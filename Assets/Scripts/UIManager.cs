using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    NetworkManager _networkManager;

    [Header("Connecting Text")]
    [SerializeField] TextMeshProUGUI connectingText;
    [SerializeField] TextMeshProUGUI errorMassageMain;

    [Header("Main Manu")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject rejoinRoomButton;

    [Header("Lobby")]
    [SerializeField] GameObject lobbyPannel;
    [SerializeField] Transform roomListParentObject;
    [SerializeField] RoomInfo roomInfoPrefab;

    [Header("Create Room Panel")]
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNickname;
    [SerializeField] TMP_InputField maxPlayers;
    [SerializeField] TMP_InputField miniGameTimer;
    [SerializeField] TMP_InputField gameTimeLimitField;
    [SerializeField] TextMeshProUGUI errorMassage;

    [Header("Choose Nickname panel")]
    [SerializeField] GameObject chooseNicknamePanel;
    [SerializeField] TMP_InputField nickNameInput;

    private int maxPlayersInt;
    private int rejoinFlag;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _networkManager = NetworkManager.Instance;
        rejoinFlag = PlayerPrefs.GetInt("NeedsRejoin", 0);

        if(rejoinFlag == 0)
        {
            rejoinRoomButton.SetActive(false);
        }

        if(rejoinFlag == 1)
        {
            rejoinRoomButton.SetActive(true);
        }
    }

    public void OnClickEnterLobby()
    {
        ToggleMainMenuPannl(false);
        ToggleMainLobbyPannel(true);
    }

    public void OnClickCreateRoomPanel()
    {
        ToggleMainMenuPannl(false);
        ToggleCreateRoomPanel(true);
    }

    public void OnClickCreateRoom()
    {
        if (!maxPlayers.text.IsNullOrEmpty())
        {
            maxPlayersInt = Int32.Parse(maxPlayers.text);
        }

        if (roomNickname.text.Length < 2 || roomNickname.text.IsNullOrEmpty())
        {
            ToggleErrorMassageRoomCreation("Room name must be 3 character or more", true);
            return;
        }

        if (maxPlayersInt < 2 || maxPlayers.text.IsNullOrEmpty())
        {
            ToggleErrorMassageRoomCreation("Minimum players is 2", true);
            return;
        }

        if(maxPlayersInt > 6 || maxPlayers.text.IsNullOrEmpty())
        {
            ToggleErrorMassageRoomCreation("Max Players is 6", true);
            return;
        }

        if(float.Parse(miniGameTimer.text) < 5 || miniGameTimer.text.IsNullOrEmpty())
        {
            ToggleErrorMassageRoomCreation("Minimum 5 seconds for spawn", true);
            return;
        }
        
        if(float.Parse(gameTimeLimitField.text) < 5 || miniGameTimer.text.IsNullOrEmpty())
        {
            ToggleErrorMassageRoomCreation("Minimum 5 minutes of game time", true);
            return;
        }

        if (float.Parse(gameTimeLimitField.text) > 15 || miniGameTimer.text.IsNullOrEmpty())
        {
            ToggleErrorMassageRoomCreation("Maximum of 15 minutes of game time", true);
            return;
        }

        ToggleErrorMassageRoomCreation("", false);

        float spawnTimer = float.Parse(miniGameTimer.text);
        float gameTimeLimit = float.Parse(gameTimeLimitField.text);

        _networkManager.CreateRoom(roomNickname.text, maxPlayersInt, spawnTimer, gameTimeLimit);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickBackToMenu()
    {
        if (createRoomPanel.gameObject.activeSelf)
        {
            ToggleCreateRoomPanel(false);
            ToggleMainMenuPannl(true);
        }

        if (lobbyPannel.gameObject.activeSelf)
        {
            ToggleMainLobbyPannel(false);
            ToggleMainMenuPannl(true);
        }
    }

    public void OnConfirmNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        ToggleNickNamePanel(false);
        ToggleMainMenuPannl(true);
    }
    public void ToggleConnectingMassage(bool state)
    {
        connectingText.gameObject.SetActive(state);
    }

    public void ToggleMainMenuPannl(bool state)
    {
        mainMenuPanel.gameObject.SetActive(state);
    }

    public void ToggleMainLobbyPannel(bool state)
    {
        lobbyPannel.gameObject.SetActive(state);
    }

    public void ToggleCreateRoomPanel(bool state)
    {
        createRoomPanel.SetActive(state);
    }

    public void ToggleErrorMassageMainManu(string massage, bool state)
    {
        errorMassageMain.text = massage;
        errorMassageMain.gameObject.SetActive(state);
    }
    public void ToggleErrorMassageRoomCreation(string massage, bool state)
    {
        errorMassage.text = massage;
        errorMassage.gameObject.SetActive(state);
    }

    public void ToggleNickNamePanel(bool state)
    {
        chooseNicknamePanel.SetActive(state);
    }

    public void UpdateLobbyRoomListUI(Dictionary<string, Photon.Realtime.RoomInfo> cachedRoomList)
    {
        foreach (Transform roomItem in roomListParentObject)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            RoomInfo roomInfo = Instantiate(roomInfoPrefab, roomListParentObject);
            roomInfo.SetRoomInfo(room.Value.Name, room.Value.PlayerCount, room.Value.MaxPlayers);
        }
    }
   
    public void OnJoinRandomRoom()
    {
        NetworkManager.Instance.JoinRandomRoom(true);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("NeedsRejoin", 0);
    }
}
