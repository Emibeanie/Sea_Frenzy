using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UpAnchorManager : MonoBehaviourPun
{
    [SerializeField] Slider anchorSlider;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject panel;

    
    private void ResetMiniGame()
    {
        anchorSlider.interactable = false;
        anchorSlider.value = 1f;

        closeButton.SetActive(false);
    }

    void Update()
    {
        UpAnchorOnSpacePress();

        if (anchorSlider.value == 0)
        {
            closeButton.SetActive(true);
        }
        else
        {
            closeButton.SetActive(false);
        }
    }

    private void UpAnchorOnSpacePress()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anchorSlider.value -= 0.1f;

            if (anchorSlider.value < 0f)
            {
                anchorSlider.value = 0f;
            }
        }
    }

    public void CloseButton()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerViewID"))
        {
            int playerViewID = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerViewID"];

            PlayerSetup player = PhotonNetwork.GetPhotonView(playerViewID).GetComponent<PlayerSetup>();

            panel.SetActive(false);

            player.ActiveOwnerControl();

            GameManager.Instance.AddTeamScore();

            photonView.RPC("InformMasterToClearMiniGame", RpcTarget.MasterClient, player.GetMiniGameSpawnerObject().GetComponent<MiniGame>().MiniGameCurrentStation);

            ResetMiniGame();

            PhotonNetwork.Destroy(player.GetMiniGameSpawnerObject());
        }
        else
        {
            Debug.LogError("playerViewID not found in custom properties!");
        }
    }
}

