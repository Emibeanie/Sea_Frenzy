using Photon.Pun;
using UnityEngine;

public class CannonManager : MonoBehaviourPun
{
    [SerializeField] GameObject cannonPanel;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject[] rocks;

    void ResetMiniGame()
    {
        foreach (GameObject rock in rocks)
        {
            rock.SetActive(true);
        }

        closeButton.SetActive(false);
    }

    private void Update()
    {
        CheckRocksAndPanel();
    }

    private void CheckRocksAndPanel()
    {
        if (cannonPanel.activeSelf)
        {
            bool allRocksInactive = true;
            foreach (GameObject rock in rocks)
            {
                if (rock.activeSelf)
                {
                    allRocksInactive = false;
                    break;
                }
            }

            if (allRocksInactive)
                closeButton.SetActive(true);
        }
    }

    public void CloseButton()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerViewID"))
        {
            int playerViewID = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerViewID"];

            PlayerSetup player = PhotonNetwork.GetPhotonView(playerViewID).GetComponent<PlayerSetup>();

            cannonPanel.SetActive(false);

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
