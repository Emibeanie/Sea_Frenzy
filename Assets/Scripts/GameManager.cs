using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    static public GameManager Instance;
    [Header("Bonus Score")]

    [SerializeField] int bonusScore = 5;
    [SerializeField] TextMeshProUGUI teamBounsText;
    private int teamScore = 0;

    [Header("Object Spawn")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform spawnedObjectsParent;
    [SerializeField] TextMeshProUGUI nextSpawnPlaceText;
    [SerializeField] GameObject ship;
    private int nextSpawnIndex;

    [Header("Pre Gameplay Menu")]
    [SerializeField] GameObject preGamplayMenu;
    [SerializeField] TextMeshProUGUI masterRoomText;
    [SerializeField] Transform clientTextsParent;
    [SerializeField] TextMeshProUGUI clientNameTextPrefab;
    [SerializeField] Button startGameButton;

    [Header("Pick a Character")]
    [SerializeField] GameObject pickCharacterPanel;
    [SerializeField] Button[] charactersButtons;

    private const string GAME_HAS_STARTED = "gameHasStarted";
    private const string TEAM_SCORE = "teamScore";

    private bool gameHasStarted;
    private GameObject myPlayer;
    int playerViewID;
    int currentButtonID;

    private int[] charactersState = { -1, -1, -1, -1, -1, -1 };

    private void Awake()
    {
        Instance = this;
        playerViewID = PlayerPrefs.GetInt("playerViewID", -1);
    }

    private void Start()
    {
        masterRoomText.text = PhotonNetwork.MasterClient.NickName;
        preGamplayMenu.SetActive(true);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GAME_HAS_STARTED))
        {
            gameHasStarted = true;
        }

        if (gameHasStarted)
        {
            preGamplayMenu.SetActive(false);
            ship.SetActive(true);
            /*SpawnPlayers();*/
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        CheckEscapeGame();
    }


    public void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }
    public void OnCharacterPicked(int buttonID)
    {
        currentButtonID = buttonID;
        int characterIndex = buttonID - 1;

        if (charactersState[characterIndex] > -1)
        {
            Debug.Log("This player had already taken");
            return;
        }

        charactersState[characterIndex] = buttonID;



        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {"charactersState", charactersState}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);



        float H = 0;
        float S = 0;
        float V = 0;

        if (buttonID == 1)
        {
            H = 120f / 360f; // Green
            S = 1.0f;
            V = 1.0f;
        }
        else if (buttonID == 2)
        {
            H = 0f / 360f; // Red
            S = 1.0f;
            V = 1.0f;
        }
        else if (buttonID == 3)
        {
            H = 60f / 360f; // Yellow
            S = 1.0f;
            V = 1.0f;
        }
        else if (buttonID == 4)
        {
            H = 39f / 360f; // Orange
            S = 1.0f;
            V = 1.0f;
        }
        else if (buttonID == 5)
        {
            H = 240f / 360f; // Blue
            S = 1.0f;
            V = 1.0f;
        }
        else if (buttonID == 6)
        {
            H = 300f / 360f; // Purple
            S = 1.0f;
            V = 1.0f;
        }

        SpawnPlayer(H, S, V);

        pickCharacterPanel.SetActive(false);
        ship.SetActive(true);
        charactersButtons[characterIndex].gameObject.SetActive(false);
    }



    // dosen't get triggered on the first time.
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {"charactersState", charactersState}
        };

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        else
        {
            // For non-master clients, retrieve the current state
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("charactersState", out object chosenCharsObj))
            {
                charactersState = (int[])chosenCharsObj;
            }
        }

        Debug.Log(PhotonNetwork.PlayerList.Length);
        if (gameHasStarted)
        {
            StartCoroutine(Rejoin());
        }

        Debug.Log($"Joined to room named: {PhotonNetwork.CurrentRoom.Name}");
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(TEAM_SCORE))
        {
            teamScore = (int)PhotonNetwork.CurrentRoom.CustomProperties[TEAM_SCORE];
            teamBounsText.text = $"Team Score: {teamScore}";
        }

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnNewObject());
            DisplayNextSpawnIndex();
        }
    }



    private void DisplayNextSpawnIndex()
    {
        nextSpawnPlaceText.text = $"The gift spawn is: {nextSpawnIndex}";
        nextSpawnPlaceText.gameObject.SetActive(true);
    }
    public void AddTeamScore()
    {
        teamScore += bonusScore;
        teamBounsText.text = $"Team Score: {teamScore}";

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {TEAM_SCORE, teamScore}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        if (teamScore >= 30)
        {
            photonView.RPC("EndGame", RpcTarget.All);
        }
    }
    void UpdatePlayerList()
    {
        // Clear the current player list UI
        foreach (Transform child in clientTextsParent)
        {
            Destroy(child.gameObject);
        }

        // Populate the player list with all players in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.IsMasterClient)
            {
                TextMeshProUGUI playerNameObj = Instantiate(clientNameTextPrefab, clientTextsParent);
                playerNameObj.text = player.NickName;
            }
        }
    }
    void CheckEscapeGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            myPlayer.GetComponent<PlayerSetup>().DisablePlayerOwner();
            StartCoroutine(EscapeRoom());
        }
    }



    void SpawnPlayer(float H, float S, float V)
    {
        int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

        myPlayer = PhotonNetwork.Instantiate("Player", spawnPoints[randomSpawnIndex].position, Quaternion.identity);
        int playerViewID = myPlayer.GetComponent<PhotonView>().ViewID;

        PlayerPrefs.SetInt("playerViewID", playerViewID);

        myPlayer.name = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC("ChangeCharacterColor", RpcTarget.AllBuffered, H, S, V);
        myPlayer?.GetComponent<PlayerSetup>().IsPlayerOwner();
    }
    [PunRPC]
    private void ChangeCharacterColor(float H, float S, float V)
    {
        // Retrieve the viewID
         playerViewID = PlayerPrefs.GetInt("playerViewID", -1); // Default to -1 if not found

        if (playerViewID != -1)
        {
            PhotonView myPhotonView = PhotonView.Find(playerViewID);

            if (myPhotonView != null)
            {
                // Change the color using the SpriteRenderer component
                myPhotonView.gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(H, S, V);
            }
            else
            {
                Debug.LogError("PhotonView not found with viewID: " + playerViewID);
            }
        }
        else
        {
            Debug.LogError("PlayerViewID not found in PlayerPrefs");
        }

    }

    [PunRPC]
    void StartGame()
    {
        gameHasStarted = true;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnNewObject());
            DisplayNextSpawnIndex();
        }

        preGamplayMenu.SetActive(false);
        pickCharacterPanel.SetActive(true);

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {GAME_HAS_STARTED, true}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    [PunRPC]
    void EndGame()
    {
        StartCoroutine(EndGameDelay(3f));
    }



    IEnumerator SpawnNewObject()
    {
        nextSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        nextSpawnPlaceText.text = $"The gift spawn is: {nextSpawnIndex}";

        yield return new WaitForSeconds(10f);
        PhotonNetwork.Instantiate("Bonus Score", spawnPoints[nextSpawnIndex].position, Quaternion.identity);

        StartCoroutine(SpawnNewObject());
        yield return null;
    }

    IEnumerator EscapeRoom()
    {
        PlayerPrefs.SetInt("NeedsRejoin", 1);
        PhotonNetwork.LeaveRoom();

        yield return new WaitForSeconds(3f);

        PhotonNetwork.LoadLevel(0);
    }

    IEnumerator EndGameDelay(float timeUntilOut)
    {

        PlayerPrefs.SetInt("NeedsRejoin", 0);

        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(timeUntilOut);

        PhotonNetwork.LoadLevel(0);

        yield return null;
    }

    IEnumerator Rejoin()
    {
        yield return new WaitForSeconds(.5f);

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        if (PhotonNetwork.PlayerList.Length > playerObjects.Length)
        {
            ship.SetActive(false);
            pickCharacterPanel.SetActive(true);
            yield return 0;
        }

        foreach (GameObject playerObject in playerObjects)
        {
            PhotonView photonView = playerObject.GetComponent<PhotonView>();

            if (photonView != null &&
                photonView.Owner != null &&
                photonView.Owner.UserId == PhotonNetwork.LocalPlayer.UserId)
            {
                Debug.Log("Found PhotonView. ViewID: " + photonView.ViewID);

                playerObject.GetComponent<PlayerSetup>().IsPlayerOwner();
                myPlayer = playerObject;
            }

            if (photonView == null && photonView.Owner == null &&
                photonView.Owner.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                pickCharacterPanel.SetActive(true);
            }
        }


        yield return null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        PhotonView myPhotonView = PhotonView.Find(playerViewID);

        if (myPhotonView != null)
        {
            if (stream.IsWriting)
            {
                // Sending data: write the color to the stream
                stream.SendNext(myPhotonView.gameObject.GetComponent<SpriteRenderer>().color.r);
                stream.SendNext(myPhotonView.gameObject.GetComponent<SpriteRenderer>().color.g);
                stream.SendNext(myPhotonView.gameObject.GetComponent<SpriteRenderer>().color.b);
                stream.SendNext(myPhotonView.gameObject.GetComponent<SpriteRenderer>().color.a);
                // One more for 7 points
            }
            else
            {
                // Receiving data: read the color from the stream
                float r = (float)stream.ReceiveNext();
                float g = (float)stream.ReceiveNext();
                float b = (float)stream.ReceiveNext();
                float a = (float)stream.ReceiveNext();
                // One more for 7 points
                Color playerColor = new Color(r, g, b, a);

                myPhotonView.gameObject.GetComponent<SpriteRenderer>().color = playerColor;
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue("charactersState", out object chosenCharsObj))
        {
            charactersState = (int[])chosenCharsObj;
           /* charactersButtons[currentButtonID - 1].gameObject.SetActive(false);*/
        }
    }


}
