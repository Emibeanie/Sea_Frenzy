using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AnchorManager : MonoBehaviourPun
{
    [SerializeField] Slider upAnchor;
    [SerializeField] Slider downAnchor;
    [SerializeField] GameObject upCloseButton;
    [SerializeField] GameObject downCloseButton;
    [SerializeField] GameObject upPanel;
    [SerializeField] GameObject downPanel;

    private void Start()
    {
        upAnchor.interactable = false;
        downAnchor.interactable = false;
    }
    void Update()
    {
        DownAnchorWhileSpacePressed();
        UpAnchorOnSpacePress();

        if (downAnchor.value == 1)
            downCloseButton.SetActive(true);

        if (upAnchor.value == 0)
            upCloseButton.SetActive(true);
    }

    private void DownAnchorWhileSpacePressed()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            downAnchor.value += 0.1f * Time.deltaTime;

            if (downAnchor.value > 1f)
            {
                downAnchor.value = 1f;
            }
        }
    }
    private void UpAnchorOnSpacePress()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            upAnchor.value -= 0.1f;

            if (upAnchor.value < 0f)
            {
                upAnchor.value = 0f;
            }
        }
    }

    public void CloseButton()
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
