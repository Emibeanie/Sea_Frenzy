using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.PlayerLoop;

public class FishManager : MonoBehaviourPun
{
    [SerializeField] private GameObject fishingPanel;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject[] fishes;
        
    public static FishManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        ResetMiniGame();
    }

    void ResetMiniGame()
    {
        foreach(GameObject fish in fishes)
        {
            fish.SetActive(true);
        }

        closeButton.SetActive(false);
    }

    private void Update()
    {
        CheckFishAndPanel();
    }

    private void CheckFishAndPanel()
    {
        if (fishingPanel.activeSelf)
        {
            bool allFishesInactive = true;
            foreach (GameObject fish in fishes)
            {
                if (fish.activeSelf)
                {
                    allFishesInactive = false;
                    break;
                }
            }

            if (allFishesInactive)
                closeButton.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerViewID"))
        {
            int playerViewID = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerViewID"];

            PlayerSetup player = PhotonNetwork.GetPhotonView(playerViewID).GetComponent<PlayerSetup>();

            player.ActiveOwnerControl();

            GameManager.Instance.AddTeamScore();

            photonView.RPC("InformMasterToClearMiniGame", RpcTarget.MasterClient, player.GetMiniGameSpawnerObject().GetComponent<MiniGame>().MiniGameCurrentStation);

            fishingPanel.SetActive(false);

            ResetMiniGame();

            PhotonNetwork.Destroy(player.GetMiniGameSpawnerObject());
        }
        else
        {
            Debug.LogError("playerViewID not found in custom properties!");
        }
    }
}
