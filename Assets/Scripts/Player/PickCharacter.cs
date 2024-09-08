using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class PickCharacter : MonoBehaviourPunCallbacks
{
    [Header("Pick a Character")]
    [SerializeField] GameObject pickCharacterPanel;
    [SerializeField] Button[] charactersButtons;
    [SerializeField] MiniGameSpawner miniGameSpawner;
    private int[] charactersState = { -1, -1, -1, -1, -1, -1 };

    public void TogglePickCharacterPanel(bool IsTrue)
    {
        MiniGame[] MiniGames = FindObjectsByType<MiniGame>(FindObjectsSortMode.None);
        PlayerSetup[] players = FindObjectsByType<PlayerSetup>(FindObjectsSortMode.None);

        pickCharacterPanel.SetActive(IsTrue);

        if (IsTrue)
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

        Hashtable CharacterState = new Hashtable
        {
            {"charactersState", charactersState}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(CharacterState);


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

        GameManager.Instance.ToggleStatsPanels();

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
        PlayerSetup playerSetup = myPlayer.GetComponent<PlayerSetup>();

        if (resourcePlayerName == "Captain")
        {
            // Ensure the player (Captain) owns the PhotonView
            if (photonView != null && photonView.IsMine)
            {
                Debug.Log("Captain owns the PhotonView.");
            }
        }
        myPlayer.transform.SetParent(GameManager.Instance.ship.transform, true); 

        playerSetup.InitializePlayer(myPlayer ,PhotonNetwork.LocalPlayer.NickName);

        int playerViewID = playerSetup.photonView.ViewID;

        Hashtable SaveViewID = new Hashtable
        {
            {"playerViewID", playerViewID }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(SaveViewID);


        Hashtable SaveMiniGameID = new Hashtable
        {
            {"playerMiniID", resourcePlayerName}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(SaveMiniGameID);

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

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
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
