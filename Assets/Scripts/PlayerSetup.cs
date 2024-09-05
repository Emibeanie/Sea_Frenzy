using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovment playerMovment;

    public GameObject PlayerObject { get; private set; }

    public int PlayerViewID { get; private set; }
    public string PlayerMiniID;


    private void Update()
    {
        CheckEscapeGame();
    }

    public void InitializePlayer(GameObject playerObj, string playerName, string miniGameID)
    {
        PlayerObject = playerObj;
        gameObject.name = playerName;
        PlayerViewID = photonView.ViewID;
        PlayerMiniID = miniGameID;
    }

    public void ActiveOwnerControl()
    {
        playerMovment.enabled = true;
    }

    public void DisablePlayerMovment()
    {
        playerMovment.enabled = false;
    }

    void CheckEscapeGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisablePlayerMovment();
            GameManager.Instance.EscapeRoom();
        }
    }


}
