using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class RejoinManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PickCharacter _pickCharacter;

    GameManager _gameManager;

    int cachedViewID;
    private void Start()
    {
        _gameManager = GameManager.Instance;
       cachedViewID = PlayerPrefs.GetInt("playerViewID", 999);
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

        foreach (GameObject playerObject in playerObjects)
        {
            PlayerSetup player = playerObject.GetComponent<PlayerSetup>();

            if(player.photonView.ViewID == cachedViewID)
            {
                player.ActiveOwnerControl();
                _gameManager._roomMenu.ToggleRoomMenu(false);
                _gameManager.ship.SetActive(true);
            }  
        }
        yield return null;
    }
}
