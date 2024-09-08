using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SailsManager : MonoBehaviourPun
{
    [SerializeField] Slider upProgressBar;
    [SerializeField] Slider downProgressBar;
    [SerializeField] GameObject upCloseButton;
    [SerializeField] GameObject downCloseButton;
    [SerializeField] GameObject upPanel;
    [SerializeField] GameObject downPanel;
    void Start()
    {
        upProgressBar.interactable = false;
        downProgressBar.interactable = false;

        upProgressBar.value = 0f;
        downProgressBar.value = 1f;
    }

    private void Update()
    {
        if(upProgressBar.value == 1)
            upCloseButton.SetActive(true);

        if (downProgressBar.value == 0)
            downCloseButton.SetActive(true);

    }
    public void UpSails()
    {
        upProgressBar.value += 0.05f;
    }
    public void DownSails()
    {
        downProgressBar.value -= 0.05f;
    }

    public void closeButton()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerViewID"))
        {
            int playerViewID = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerViewID"];

            PlayerSetup player = PhotonNetwork.GetPhotonView(playerViewID).GetComponent<PlayerSetup>();


            upPanel.SetActive(false);
            downPanel.SetActive(false);

            player.ActiveOwnerControl();

            GameManager.Instance.AddTeamScore();

            photonView.RPC("InformMasterToClearMiniGame", RpcTarget.MasterClient, player.GetMiniGameSpawnerObject().GetComponent<MiniGame>().MiniGameCurrentStation);

            PhotonNetwork.Destroy(player.GetMiniGameSpawnerObject());
        }
        else
        {
            Debug.LogError("playerViewID not found in custom properties!");
        }
    }
}
