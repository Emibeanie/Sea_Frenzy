using System.Collections.Generic;
using UnityEngine;

public class FishPoolEmily : MonoBehaviour
{
    public GameObject fishPrefab;
    public int poolSize = 6;
    private List<GameObject> fishPool;

    private void OnEnable()
    {
        InitializePool();
    }
    void InitializePool()
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

        Debug.LogError("No available fish in the pool, limit reached.");
        return null;
    }
    public void ResetPool()
    {
        foreach (var fish in fishPool)
        {
            fish.SetActive(false);
        }
    }
}
