using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovment playerMovment;
    private Vector3 spawnPosition;

    public GameObject PlayerObject { get; private set; }
    public int PlayerViewID { get; private set; }
    public string PlayerMiniID;

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PlayerPrefs.HasKey("playerPosition"))
            {
                string positionString = PlayerPrefs.GetString("playerPosition");
                string[] positionValues = positionString.Split(',');
                float x = float.Parse(positionValues[0]);
                float y = float.Parse(positionValues[1]);
                float z = float.Parse(positionValues[2]);
                spawnPosition = new Vector3(x, y, z);
            }
            else
            {
                spawnPosition = transform.position;
            }
        }
    }

    private void Start()
    {
        transform.position = spawnPosition;
    }

    private void Update()
    {
        CheckEscapeGame();
    }

    public void InitializePlayer(GameObject playerObj, string playerName, string miniGameID)
    {
        PlayerObject = playerObj;
        gameObject.name = playerName;
        PlayerViewID = photonView.ViewID;
        PlayerMiniID = miniGameID;
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
            PlayerPrefs.SetString("playerPosition", $"{transform.position.x},{transform.position.y},{transform.position.z}");
            PlayerPrefs.Save();

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "IsInGame", false }
        };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            DisablePlayerMovment();
            GameManager.Instance.EscapeRoom();
        }
    }

    public override void OnLeftRoom()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    {
        { "IsInGame", false }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
