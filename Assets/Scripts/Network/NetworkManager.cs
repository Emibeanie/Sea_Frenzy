using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    [SerializeField] TextMeshProUGUI lobbyErrorMessage;

    private Dictionary<string, Photon.Realtime.RoomInfo> _cachedRoomList = new();

    UIManager _uiManager;
    TypedLobby mainLobby = new TypedLobby("Test", LobbyType.Default);
    string lastRoomName;

    private void Awake()
    {
        Instance = this;
        lastRoomName = PlayerPrefs.GetString("LastRoomNickName", "");
    }

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _uiManager.ToggleConnectingMassage(true);

        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby(mainLobby); // Join the lobby if connected but not in the lobby
            _uiManager.ToggleNickNamePanel(false);
            _uiManager.ToggleConnectingMassage(false);
            _uiManager.ToggleMainMenuPannl(true);
        }
        else if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _uiManager.ToggleConnectingMassage(false);
        _uiManager.ToggleCreateRoomPanel(false);
        _uiManager.ToggleMainLobbyPannel(false);
        _uiManager.ToggleErrorMassageMainManu("", false);
        _uiManager.ToggleMainMenuPannl(false);
        _uiManager.ToggleConnectingMassage(false);
        _uiManager.ToggleNickNamePanel(true);

        Debug.Log("You are connected to the master server");
        PhotonNetwork.JoinLobby(mainLobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"You are now in {PhotonNetwork.CurrentLobby.Name} Lobby");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"A room named {PhotonNetwork.CurrentRoom.Name} has been created");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message + "Error Code:" + returnCode);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message + "Error Code:" + returnCode);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("You have disconnected");
        _uiManager.ToggleErrorMassageMainManu("You are disconected, trying to connect again...", true);
        PlayerPrefs.SetInt("NeedsRejoin", 1);
        StartCoroutine(TryReconnecting());
    }

    private IEnumerator TryReconnecting()
    {
        PhotonNetwork.ReconnectAndRejoin();
        yield return new WaitForSeconds(.5f);

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("You reconnected to the master server");
        }
        else
        {
            Debug.Log("Trying to reconnect in 1 second");
            yield return new WaitForSeconds(1f);
            StartCoroutine(TryReconnecting());
        }
    }

    public override void OnRoomListUpdate(List<Photon.Realtime.RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                _cachedRoomList.Remove(room.Name);
            }
            else
            {

                _cachedRoomList[room.Name] = room;
            }
        }

        _uiManager.UpdateLobbyRoomListUI(_cachedRoomList);
    }

    public void CreateRoom(string roomNickname, int maxPlayers, float miniGameSpawnTime, float gameTimeLimit)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions roomOptions = new RoomOptions
            {
                PublishUserId = true,
                MaxPlayers = (byte)maxPlayers,
                PlayerTtl = -1,
                EmptyRoomTtl = 30000,
                IsVisible = true,
                CleanupCacheOnLeave = false
            };

            PlayerPrefs.SetString("LastRoomNickName", roomNickname);
            PlayerPrefs.SetFloat("MiniGameSpawnTime", miniGameSpawnTime);
            PlayerPrefs.SetFloat("GameTimeLimit", gameTimeLimit);
            PhotonNetwork.CreateRoom(roomNickname, roomOptions, mainLobby);
        }
    }

    public void RejoinRoom()
    {
        PhotonNetwork.RejoinRoom(lastRoomName);
    }

    public void JoinRandomRoom(bool joinOngoingGame)
    {
        List<Photon.Realtime.RoomInfo> suitableRooms = new List<Photon.Realtime.RoomInfo>();

        foreach (var roomInfo in _cachedRoomList.Values)
        {
            if (roomInfo.PlayerCount < roomInfo.MaxPlayers)
            {
                suitableRooms.Add(roomInfo);
            }
        }

        if (suitableRooms.Count > 0)
        {
            Photon.Realtime.RoomInfo randomRoom = suitableRooms[Random.Range(0, suitableRooms.Count)];
            PhotonNetwork.JoinRoom(randomRoom.Name);
        }
        else
        {
            lobbyErrorMessage.gameObject.SetActive(true);
            lobbyErrorMessage.text = "No suitable rooms found";
        }
    }
}