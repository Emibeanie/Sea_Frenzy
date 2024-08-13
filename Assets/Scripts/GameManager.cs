using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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
