using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    static public GameManager Instance;

    [NonSerialized]
    public const string GAME_HAS_STARTED = "gameHasStarted";
    private const string TEAM_SCORE = "teamScore";

    [SerializeField] public GameObject ship;

    [Header("Score")]
    private int teamScore = 0;
    [SerializeField] int score = 5;
    [SerializeField] TextMeshProUGUI teamBounsText;

    [Header("In Room Menu")]
    [SerializeField] public RoomMenu _roomMenu;

    [Header("Mini Game")]
    [SerializeField] public MiniGameSpawner _miniGameSpawner;

    [Header("Pick Character")]
    [SerializeField] public PickCharacter _pickCharacter;

    [Header("Rejoin")]
    [SerializeField] public RejoinManager rejoinManager;

    [Header("End Game Menu")]
    [SerializeField] GameObject endGamePanel;
    [SerializeField] TextMeshProUGUI fishCollectedText;
    [SerializeField] TextMeshProUGUI gameTimerText;
    [SerializeField] Button endGameButton;

    [Header("Captain Board")]
    [SerializeField] GameObject captainBoard;

    public bool gameHasStarted { get; private set; }

    private bool isLeavingRoom = false;
    private int currentButtonID;
    private float gameTimer;
    int tempTimer;

    private void Awake()
    {
        Instance = this;
        tempTimer = 60; // have to be greater then 0 to prevent the game from ending in case of rejoining the room.
    }

    private void Start()
    {
    
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GAME_HAS_STARTED))
        {
            gameHasStarted = true;
        }

        if (!gameHasStarted)
        {
            gameTimer = PlayerPrefs.GetFloat("GameTimeLimit", 5) * 60; // convert it to minutes.
            _roomMenu.ToggleRoomMenu(true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            _roomMenu.ToggleStartButton(true);
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 25000;
        }

        if (gameHasStarted)
        {
            StartCoroutine(rejoinManager.InitializeRejoin());
            _roomMenu.ToggleRoomMenu(false);
            ship.SetActive(true);
        }
    }

    private void Update()
    {
        if (gameHasStarted)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (gameTimer > 0)
                {
                    gameTimer -= Time.deltaTime;

                    tempTimer = (int)gameTimer;

                    if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
                    {
                        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
                         {
                            {"GameTimer", tempTimer}
                         };

                        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
                    }
                }
            }

            if (tempTimer <= 0)
            {
                ship.SetActive(false);
                fishCollectedText.text = $"Score Collected: {teamScore}";
                endGamePanel.SetActive(true);

                if (PhotonNetwork.IsMasterClient)
                {
                    endGameButton.gameObject.SetActive(true);
                }
            }
        }
    }


    // dosen't get triggered on the first time.
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined to room named: {PhotonNetwork.CurrentRoom.Name}");

        if (gameHasStarted)
        {
            StartCoroutine(rejoinManager.InitializeRejoin());
            return;
        }

        _roomMenu.UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(TEAM_SCORE))
        {
            teamScore = (int)PhotonNetwork.CurrentRoom.CustomProperties[TEAM_SCORE];
            teamBounsText.text = $"Team Score: {teamScore}";
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameTimer"))
        {
            gameTimer = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameTimer"];
        }

        _roomMenu.UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _roomMenu.UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _miniGameSpawner.SpawnNewMiniGame(); // spawning score currently
            _miniGameSpawner.DisplayNextMiniGameSpawnStation();
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue("GameTimer", out object chosenCharsObj))
        {
            tempTimer = (int)chosenCharsObj;
            gameTimerText.text = $"Time left : {tempTimer}s";
        }
    }

    public void AddTeamScore()
    {
        teamScore += score;
        teamBounsText.text = $"Team Score: {teamScore}";

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {TEAM_SCORE, teamScore}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    public void SetGameStart(bool status)
    {
        gameHasStarted = status;

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
    {
        {GAME_HAS_STARTED, gameHasStarted }
    };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    public void ToggleCaptainBoard(bool status)
    {
        captainBoard.SetActive(status);
    }


    IEnumerator EscapeRoomDelay()
    {
        if (isLeavingRoom) yield break;

        isLeavingRoom = true;
        PlayerPrefs.SetInt("NeedsRejoin", 1);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom();
        }

        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            Debug.LogError("Client disconnected unexpectedly.");
        }
        else
        {
            PhotonNetwork.LoadLevel(0);
        }

        isLeavingRoom = false;
    }
    IEnumerator EndGameDelay(float timeUntilOut)
    {

        PlayerPrefs.SetInt("NeedsRejoin", 0);

        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(timeUntilOut);

        PhotonNetwork.LoadLevel(0);

        yield return null;
    }

    public void EscapeRoom()
    {
        StartCoroutine(EscapeRoomDelay());
    }

    public void OnClickEndGameButton()
    {
        photonView.RPC("EndGame", RpcTarget.All);
    }

    [PunRPC]
    void EndGame()
    {
        StartCoroutine(EndGameDelay(3f));
    }

}
