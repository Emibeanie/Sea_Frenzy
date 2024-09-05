using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PickCharacter : MonoBehaviourPunCallbacks
{
    [Header("Pick a Character")]
    [SerializeField] GameObject pickCharacterPanel;
    [SerializeField] Button[] charactersButtons;
    [SerializeField] MiniGameSpawner miniGameSpawner;
    private int[] charactersState = { -1, -1, -1, -1, -1, -1 };

    public void TogglePickCharacterPanel(bool status)
    {
        MiniGame[] MiniGames = FindObjectsByType<MiniGame>(FindObjectsSortMode.None);
        PlayerSetup[] players = FindObjectsByType<PlayerSetup>(FindObjectsSortMode.None);

        pickCharacterPanel.SetActive(status);

        if (status)
        {  
            foreach (var mini in MiniGames)
            {
                mini.gameObject.SetActive(false);
            }

            foreach (var player in players)
            {
                player.gameObject.SetActive(false);
            }
        }
       
       
    }

    public void OnCharacterPicked(int buttonID)
    {
       
        int characterIndex = buttonID - 1;

        if (charactersState[characterIndex] > -1)
        {
            Debug.Log("This player had already taken");
            return;
        }

        charactersState[characterIndex] = buttonID;

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {"charactersState", charactersState}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);


        SpawnPlayer(PickResourcePlayerName(buttonID));

        pickCharacterPanel.SetActive(false);
        GameManager.Instance.ship.SetActive(true);
        charactersButtons[characterIndex].gameObject.SetActive(false);

        if(buttonID == 1)
        {
            GameManager.Instance.ToggleCaptainBoard(true);
        }
        else
        {
            GameManager.Instance.ToggleCaptainBoard(false);
        }

        MiniGame[] MiniGames = FindObjectsByType<MiniGame>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        PlayerSetup[] players = FindObjectsByType<PlayerSetup>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var mini in MiniGames)
        {
            mini.gameObject.SetActive(true);
        }

        foreach (var player in players)
        {
            player.gameObject.SetActive(true);
        }

    }


    string PickResourcePlayerName(int characterIndex)
    {
        string resourceName = "";

        switch (characterIndex)
        {
            case 1: resourceName = "Captain"; break;

            case 2: resourceName = "Fisher"; break;

            case 3: case 4: resourceName = "Canon"; break;

            case 5: resourceName = "Sails"; break;

            case 6: resourceName = "Anchor"; break;
        }

        Debug.Log("Resource Name : " + resourceName);
        return resourceName;
    }
    void SpawnPlayer(string resourcePlayerName)
    {
        int randomSpawnIndex = UnityEngine.Random.Range(0, miniGameSpawner.spawnPoints.Length);

        GameObject myPlayer = PhotonNetwork.Instantiate(resourcePlayerName, miniGameSpawner.spawnPoints[randomSpawnIndex].position, Quaternion.identity);
        myPlayer.transform.SetParent(GameManager.Instance.ship.transform, true); 
        PlayerSetup playerSetup = myPlayer.GetComponent<PlayerSetup>();

        playerSetup.InitializePlayer(myPlayer ,PhotonNetwork.LocalPlayer.NickName, resourcePlayerName);

        PlayerPrefs.SetInt("playerViewID", playerSetup.PlayerViewID);

        playerSetup.ActiveOwnerControl();

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.ToggleCaptainBoard(true);
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable
        {
            {"charactersState", charactersState}
        };

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        else
        {
            // For non-master clients, retrieve the current state
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("charactersState", out object chosenCharsObj))
            {
                charactersState = (int[])chosenCharsObj;
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue("charactersState", out object chosenCharsObj))
        {
            charactersState = (int[])chosenCharsObj;
            /* charactersButtons[currentButtonID - 1].gameObject.SetActive(false);*/
        }
    }

    public bool IsPickCharacterActive()
    {
       return pickCharacterPanel.activeSelf;
    }
}
