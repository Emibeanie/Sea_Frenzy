using UnityEngine;
using Photon.Pun;

public class MiniGame : MonoBehaviour
{

    int miniGameCurrentStation;
    public string MiniGameID { get; private set; }

    [PunRPC]
    public void AssignMiniGameID(string miniGameID)
    {
        MiniGameID = miniGameID;
    }

    public void InisilaizeMiniGame(string miniGameID)
    {
        // Use the RPC to assign the MiniGameID across all clients
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("AssignMiniGameID", RpcTarget.AllBuffered, miniGameID);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSetup player = other.GetComponent<PlayerSetup>();

            string playerMiniID = player.PlayerMiniID;

            if(MiniGameID.Contains(playerMiniID) || playerMiniID == "Captain")
            {
                if (player.photonView.IsMine)
                {
                    MiniGameSpawner.Instance.ToggleMiniGame(MiniGameID, true);
                    player.DisablePlayerMovment();
                }
            }
        

           /* GameManager.Instance.AddTeamScore();
            GameManager.Instance._miniGameSpawner.clearMininGame(miniGameCurrentStation);
            Destroy(gameObject);*/
        }
    }

    public void SetMiniGameCurrentStation(int spawnStation)
    {
        miniGameCurrentStation = spawnStation;
    }
}
