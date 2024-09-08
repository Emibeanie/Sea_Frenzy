using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MiniGame : MonoBehaviourPunCallbacks
{
    public string MiniGameID { get; private set; }

    public int MiniGameCurrentStation;

    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);  
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == photonView)
        {
            Debug.Log("Ownership transferred to: " + photonView.Owner.NickName);
           
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player requestingPlayer)
    {
        Debug.LogWarning("Ownership transfer failed for " + targetView.name);
    }

    [PunRPC]
    public void AssignMiniGameID(string miniGameID, int currentStation)
    {
        MiniGameID = miniGameID;
        MiniGameCurrentStation = currentStation;
    }

    public void InisilaizeMiniGame(string miniGameID, int currentStation)
    {
        photonView.RPC("AssignMiniGameID", RpcTarget.AllBuffered, miniGameID, currentStation);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSetup player = other.GetComponent<PlayerSetup>();
            string playerMiniID = "";
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerMiniID"))
            {
                 playerMiniID = PhotonNetwork.LocalPlayer.CustomProperties["playerMiniID"].ToString();  
            }

            Debug.Log(playerMiniID);

            if (MiniGameID.Contains(playerMiniID) || playerMiniID == "Captain")
            {
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(player.photonView.Owner);
                }

                if (player.photonView.IsMine)
                {
                    player.AssignMiniGameSpawner(gameObject);
                    MiniGameSpawner.Instance.ToggleMiniGame(MiniGameID, true);
                    player.DisablePlayerMovment();
                }    
            }
        }
    }
}
