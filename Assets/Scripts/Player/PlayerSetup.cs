using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovment playerMovment;

    [SerializeField] private int health = 100;
    [SerializeField] private int score = 10;
    public GameObject PlayerObject { get; private set; }

    public int PlayerViewID { get; private set; }

    GameObject miniGameSpawnerAssigned;

    void SendPlayerData()
    {
        PlayerData playerData = new PlayerData(transform.position, health, score);

        string jsonData = JsonUtility.ToJson(playerData);

        photonView.RPC("ReceivePlayerData", RpcTarget.All, jsonData);
    }

    [PunRPC]
    void ReceivePlayerData(string jsonData)
    {
        // Deserialize the JSON string back to a PlayerData object
        PlayerData receivedData = JsonUtility.FromJson<PlayerData>(jsonData);

        // Use the received data (for example, update player position, health, and score)
        Debug.Log("Received position: " + receivedData.position);
        Debug.Log("Received health: " + receivedData.health);
        Debug.Log("Received score: " + receivedData.score);
    }


    private void Update()
    {
        CheckEscapeGame();

        if (Input.GetKeyDown(KeyCode.P))
        {
            SendPlayerData();
        }
    }

    public void InitializePlayer(GameObject playerObj, string playerName)
    {
        PlayerObject = playerObj;
        gameObject.name = playerName;
        PlayerViewID = photonView.ViewID;
    }

    public void ActiveOwnerControl()
    {
        playerMovment.enabled = true;
    }

    public void DisablePlayerMovment()
    {
        playerMovment.enabled = false;
    }

    void CheckEscapeGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisablePlayerMovment();
            GameManager.Instance.EscapeRoom();
        }
    }

    public void AssignMiniGameSpawner(GameObject miniSpawner)
    {
        miniGameSpawnerAssigned = miniSpawner;
    }

    public GameObject GetMiniGameSpawnerObject()
    {
        return miniGameSpawnerAssigned;
    }

   
}
