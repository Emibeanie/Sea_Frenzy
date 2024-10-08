using Photon.Pun;
using System.Collections;
using UnityEngine;

public class RejoinManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PickCharacter _pickCharacter;

    GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public IEnumerator InitializeRejoin()
    {
        // dealy to ensure that the player does spawn before searching for it
        yield return new WaitForSeconds(1f);

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        // Check if the number of players in the room is greater the number of player objects in the room.
        if (PhotonNetwork.PlayerList.Length > playerObjects.Length)
        {
            _gameManager.ship.SetActive(false);
            _pickCharacter.TogglePickCharacterPanel(true);
            yield break;
        }

        foreach (GameObject myPlayer in playerObjects)
        {
            int cachedViewID = -1;
            PlayerSetup player = myPlayer.GetComponent<PlayerSetup>();

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerViewID"))
            {
                int playerViewID = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerViewID"];

                PlayerSetup playerSetup = PhotonNetwork.GetPhotonView(playerViewID).GetComponent<PlayerSetup>();
                cachedViewID = playerSetup.photonView.ViewID;
            }

            if (player.photonView.ViewID == cachedViewID)
            {
                player.ActiveOwnerControl();
                _gameManager._roomMenu.ToggleRoomMenu(false);
                _gameManager.ship.SetActive(true);
                GameManager.Instance.ToggleStatsPanels(true);

                if (PhotonNetwork.IsMasterClient)
                {
                    GameManager.Instance.ToggleStationPanels(true);
                }
            }
        }
        yield return null;
    }
}
