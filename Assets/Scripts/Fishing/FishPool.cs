using System.Collections.Generic;
using UnityEngine;

public class FishPool : MonoBehaviour
{
    public GameObject fishPrefab;
    public int poolSize = 6;
    private List<GameObject> fishPool;

    void Start()
    {
        fishPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject fish = Instantiate(fishPrefab,transform);
            fish.SetActive(false);
            fishPool.Add(fish);
        }
    }
    public GameObject GetFish()
    {
        foreach (var fish in fishPool)
        {
            if (!fish.activeInHierarchy)
            {
                fish.SetActive(true);
                return fish;
            }
        }
        return null;
    }
}
