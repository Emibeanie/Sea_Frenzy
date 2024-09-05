using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovment playerMovment;
    [SerializeField] SpriteRenderer spriteRenderer;

    public GameObject PlayerObject { get; private set; }

    public int PlayerViewID { get; private set; }


    private void Update()
    {
        CheckEscapeGame();
    }

    public void InitializePlayer(GameObject playerObj, string playerName)
    {
        PlayerObject = playerObj;
        gameObject.name = playerName;
        PlayerViewID = photonView.ViewID;
    }

    public void ActiveOwnerControl()
    {
        playerMovment.enabled = true;
    }

    public void DisablePlayerOwner()
    {
        playerMovment.enabled = false;
    }

    void CheckEscapeGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisablePlayerOwner();
            GameManager.Instance.EscapeRoom();
        }
    }


}
