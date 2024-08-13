using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public FishPool fishPool;
    public RectTransform panelRectTransform;
    public float spawnInterval = 2.0f;

    void Start()
    {
        InvokeRepeating("SpawnFish", 0, spawnInterval);
    }

    void SpawnFish()
    {
        GameObject fish = fishPool.GetFish();
        fish.transform.SetParent(panelRectTransform, false);

        RectTransform fishRectTransform = fish.GetComponent<RectTransform>();
        Debug.Log(panelRectTransform.rect.width);
        Debug.Log(panelRectTransform.rect.height);

        float width = panelRectTransform.rect.width - fishRectTransform.rect.width;
        float height = panelRectTransform.rect.height - fishRectTransform.rect.height;
        float x = Random.Range(-width / 2, width / 2);
        float y = Random.Range(-height / 2, height / 2);
        fishRectTransform.anchoredPosition = new Vector2(x, y);

        Debug.Log("Fish spawned at: " + fishRectTransform.anchoredPosition);
    }
}



