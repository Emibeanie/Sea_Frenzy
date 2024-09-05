using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishPool fishPool;
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private float spawnInterval = 2.0f;

    public int FishSpawnLimit { get => _fishSpawnLimit; }

    private int _fishSpawnLimit;
    private int _fishCount;
    private const float _gridCellSize = 100f;
    private Dictionary<Vector2Int, List<RectTransform>> _gridCells;
    private int iterationLimit = 100;
    private int iterationCount = 0;

    void Start()
    {
        _gridCells = new Dictionary<Vector2Int, List<RectTransform>>();
        _fishSpawnLimit = Random.Range(18, 28);
        InvokeRepeating("SpawnFish", 0, spawnInterval);
    }

    void SpawnFish()
    {
        while (_fishCount < _fishSpawnLimit)
        {
            GameObject fish = fishPool.GetFish();

            if (fish == null)
            {
                Debug.LogError("FishPool returned null even after attempting to instantiate new fish. Check pool size and prefab assignment.");
                break;
            }

            fish.transform.SetParent(panelRectTransform, false);

            RectTransform fishRectTransform = fish.GetComponent<RectTransform>();

            float width = panelRectTransform.rect.width - fishRectTransform.rect.width;
            float height = panelRectTransform.rect.height - fishRectTransform.rect.height;

            Vector2 position;
            bool positionValid;

            do
            {
                float x = Random.Range(-width / 2, width / 2);
                float y = Random.Range(-height / 2, height / 2);
                position = new Vector2(x, y);

                positionValid = true;
                Vector2Int gridPos = GetGridPosition(position);

                foreach (var offset in GetNeighborOffsets())
                {
                    Vector2Int neighborCell = gridPos + offset;
                    if (_gridCells.ContainsKey(neighborCell))
                    {
                        foreach (var otherRect in _gridCells[neighborCell])
                        {
                            if (RectOverlaps(otherRect, position, fishRectTransform.rect.size))
                            {
                                positionValid = false;
                                break;
                            }
                        }
                    }

                    if (!positionValid) break;
                }

                iterationCount++;
                if (iterationCount > iterationLimit)
                {
                    Debug.LogError("Positioning loop exceeded iteration limit, breaking.");
                    break;
                }

            } while (!positionValid);

            fishRectTransform.anchoredPosition = position;
            RegisterFishPosition(fishRectTransform);

            Debug.Log("Fish spawned at: " + fishRectTransform.anchoredPosition);
            _fishCount++;
        }
    }


    Vector2Int GetGridPosition(Vector2 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / _gridCellSize),
            Mathf.FloorToInt(position.y / _gridCellSize)
        );
    }

    IEnumerable<Vector2Int> GetNeighborOffsets()
    {
        yield return new Vector2Int(0, 0);
        yield return new Vector2Int(-1, 0);
        yield return new Vector2Int(1, 0);
        yield return new Vector2Int(0, -1);
        yield return new Vector2Int(0, 1);
        yield return new Vector2Int(-1, -1);
        yield return new Vector2Int(-1, 1);
        yield return new Vector2Int(1, -1);
        yield return new Vector2Int(1, 1);
    }

    void RegisterFishPosition(RectTransform fishRect)
    {
        Vector2Int gridPos = GetGridPosition(fishRect.anchoredPosition);

        if (!_gridCells.ContainsKey(gridPos))
        {
            _gridCells[gridPos] = new List<RectTransform>();
        }

        _gridCells[gridPos].Add(fishRect);
    }

    bool RectOverlaps(RectTransform otherRect, Vector2 newPos, Vector2 size)
    {
        Vector2 minNew = newPos - size / 2;
        Vector2 maxNew = newPos + size / 2;

        Vector3[] otherCorners = new Vector3[4];
        otherRect.GetWorldCorners(otherCorners);

        for (int i = 0; i < 4; i++)
        {
            otherCorners[i] = panelRectTransform.InverseTransformPoint(otherCorners[i]);
        }

        Vector2 minOther = new Vector2(otherCorners[0].x, otherCorners[0].y);
        Vector2 maxOther = new Vector2(otherCorners[2].x, otherCorners[2].y);

        return minNew.x < maxOther.x && maxNew.x > minOther.x && minNew.y < maxOther.y && maxNew.y > minOther.y;
    }
}
