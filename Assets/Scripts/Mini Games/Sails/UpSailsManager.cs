using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UpSailsManager : MonoBehaviourPun
{
    [SerializeField] Slider sailSlider;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject panel;

    private void ResetMiniGame()
    {
        sailSlider.interactable = false;
        sailSlider.value = 0f;
        closeButton.SetActive(false);
    }

    private void Update()
    {
        if (sailSlider.value == 1f)
            closeButton.SetActive(true);
    }

    public void UpSailsButton()
    {
        sailSlider.value += 0.05f;
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

