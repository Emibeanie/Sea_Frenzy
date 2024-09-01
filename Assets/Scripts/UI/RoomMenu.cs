using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviourPunCallbacks
{

    GameManager _gameManager;

    [Header("Pre Gameplay Menu")]
    [SerializeField] GameObject preGamplayMenu;
    [SerializeField] TextMeshProUGUI masterRoomText;
    [SerializeField] Transform clientTextsParent;
    [SerializeField] TextMeshProUGUI clientNameTextPrefab;
    [SerializeField] Button startGameButton;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        masterRoomText.text = PhotonNetwork.MasterClient.NickName;
  
    }
    public void OnClickStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    public void ToggleRoomMenu(bool status)
    {
        ToggleUIObject(preGamplayMenu, status);
    }

    public void ToggleStartButton(bool status)
    {
        ToggleUIObject(startGameButton.gameObject, status);
    }

    private void ToggleUIObject(GameObject toggleObject, bool status)
    {
        toggleObject.SetActive(status);
    }

    public void UpdatePlayerList()
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

    [PunRPC]
    void StartGame()
    {
        _gameManager.SetGameStart(true);

        if (PhotonNetwork.IsMasterClient)
        {
            _gameManager._miniGameSpawner.SpawnNewMiniGame(); // spawning score currently
            _gameManager._miniGameSpawner.DisplayNextMiniGameSpawnStation();
        }

        _gameManager._roomMenu.ToggleRoomMenu(false);
        _gameManager._pickCharacter.TogglePickCharacterPanel(true);

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {"gameHasStarted", true}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
