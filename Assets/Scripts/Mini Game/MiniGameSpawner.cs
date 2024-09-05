using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniGameSpawner : MonoBehaviour
{
    static public MiniGameSpawner Instance;

    [Header("Object Spawn")]
    public List<MiniGamePanel> miniGamesPanelsList = new List<MiniGamePanel>();
    private Dictionary<string, GameObject> miniGamesPanels;

    [SerializeField] public Transform[] spawnPoints;
    [SerializeField] TextMeshProUGUI nextSpawnPlaceText;
    private int nextSpawnIndex;
    float spawnTime;

    int[] currentSpawns = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }; // -1 means there is no spawn at the station, 1 means mini game has spawned at that location.

    private void Awake()
    {
        Instance = this;
        spawnTime = PlayerPrefs.GetFloat("MiniGameSpawnTime", 10f);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        ConvertToDictionary();
        yield return null;
    }

    public void DisplayNextMiniGameSpawnStation()
    {
        nextSpawnPlaceText.text = $"The gift spawn is: {nextSpawnIndex}";
        nextSpawnPlaceText.gameObject.SetActive(true);
    }

    public void SpawnNewMiniGame()
    {
        StartCoroutine(SpawnNewMiniGame(spawnTime));
    }

    public void clearMininGame(int miniGameIndex)
    {
        currentSpawns[miniGameIndex] = -1;
    }

    IEnumerator SpawnNewMiniGame(float time)
    {
        nextSpawnIndex = UnityEngine.Random.Range(0, currentSpawns.Length);

        if (currentSpawns[nextSpawnIndex] != -1)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SpawnNewMiniGame(time));
            yield break;
        }

        nextSpawnPlaceText.text = $"The next spawn is : {nextSpawnIndex}";

        yield return new WaitForSeconds(time);
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
        {
            switch (nextSpawnIndex)
            {
                case 0:
                case 2:
                case 3:
                case 4:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame("Fisher");
                        Debug.Log("Fishing Mini Game has been spawned");
                    }
                    break;

                case 5:
                case 6:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame("Canon ");
                        Debug.Log("Canon Mini Game has been spawned");
                    }
                    break;

                case 1:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame("Anchor Up");
                        Debug.Log("Anchor Up Mini Game has been spawned");
                    }
                    break;

                case 9:
                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame("Anchor Down");
                        Debug.Log("Anchor Down Mini Game has been spawned");
                    }
                    break;

                case 7:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame("Sails Up");
                        Debug.Log("Sails Up Mini Game has been spawned");
                    }
                    break;

                case 8:
                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame("Sails Down");
                        Debug.Log("Sails Down Mini Game has been spawned");
                    }
                    break;
            }
        }

        StartCoroutine(SpawnNewMiniGame(time));
        yield return null;
    }


    private void SpawnMiniGame(string miniGameID)
    {

        GameObject triangleScoreClone = PhotonNetwork.InstantiateRoomObject(
            "Bonus Score",
            spawnPoints[nextSpawnIndex].position,
            Quaternion.identity
        );


        MiniGame miniGameComponent = triangleScoreClone.GetComponent<MiniGame>();

        miniGameComponent.InisilaizeMiniGame(miniGameID);

        triangleScoreClone.transform.SetParent(GameManager.Instance.ship.transform, true);

        miniGameComponent.SetMiniGameCurrentStation(nextSpawnIndex);

        currentSpawns[nextSpawnIndex] = 1;
    }

    private void ConvertToDictionary()
    {
        miniGamesPanels = new Dictionary<string, GameObject>();
        foreach (var item in miniGamesPanelsList)
        {
            miniGamesPanels.Add(item.miniGameId, item.panel);
        }
    }

    public void ToggleMiniGame(string miniGameID, bool status)
    {

        if (miniGamesPanels.TryGetValue(miniGameID, out GameObject panel))
        {
            Debug.Log(gameObject.name);
            if (panel != null)
            {
                panel.SetActive(status);
            }
            else
            {
                Debug.LogError("Panel for MiniGameID " + miniGameID + " is null!");
            }
        }
    }
}
