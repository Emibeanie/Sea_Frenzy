using UnityEngine;
using TMPro;
public class FishGameManager : MonoBehaviour
{
    public static FishGameManager Instance { get; private set; }
    public TMP_Text fishCounterText;
    private int fishCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddFish()
    {
        fishCount++;
        fishCounterText.text = "Fish Caught: " + fishCount;
    }
}
