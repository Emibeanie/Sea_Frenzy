using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class MiniGameSpawner : MonoBehaviour
{ 
    [Header("Object Spawn")]
    [SerializeField] public Transform[] spawnPoints;
    [SerializeField] TextMeshProUGUI nextSpawnPlaceText;
    private int nextSpawnIndex;
    float spawnTime;

    int[] currentSpawns = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }; // -1 means there is no spawn at the station, 1 means mini game has spawned at that location.

    private void Awake()
    {
        spawnTime = PlayerPrefs.GetFloat("MiniGameSpawnTime", 10f);
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
                case 0: case 2: case 3: case 4:

                    if(currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame();
                        Debug.Log("Fishing Mini Game has been spawned");
                    }
                    break;

                case 5: case 6:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame();
                        Debug.Log("Canon Mini Game has been spawned");
                    }
                    break;

                case 1: case 9:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame();
                        Debug.Log("Anchor Mini Game has been spawned");
                    }
                    break;

                case 7: case 8:

                    if (currentSpawns[nextSpawnIndex] == -1)
                    {
                        SpawnMiniGame();
                        Debug.Log("Sails Mini Game has been spawned");
                    }
                    break;
            }
        }

        StartCoroutine(SpawnNewMiniGame(time));
        yield return null;
    }

    private void SpawnMiniGame()
    {
        GameObject triangleScoreClone = PhotonNetwork.Instantiate
         ("Bonus Score",
         spawnPoints[nextSpawnIndex].position,
         Quaternion.identity);
        triangleScoreClone.transform.SetParent(GameManager.Instance.ship.transform, true);

        triangleScoreClone.GetComponent<MiniGame>().SetMiniGameCurrentStation(nextSpawnIndex);//to not use get component!! howww

        currentSpawns[nextSpawnIndex] = 1;
    }
}
