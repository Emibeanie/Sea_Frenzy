using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomInfo : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI playerCountText;
    [SerializeField] TextMeshProUGUI errorMassage;

    private string roomName;
    public void SetRoomInfo(string name, int playerCount, int maxPlayers)
    {
        roomName = name;
        roomNameText.text = name;
        playerCountText.text = $"{playerCount}/{maxPlayers}";
    }

    public void OnClickEnterRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PlayerPrefs.SetString("LastRoomNickName", roomName);
            PhotonNetwork.JoinRoom(roomName);
        }  
    }
}
