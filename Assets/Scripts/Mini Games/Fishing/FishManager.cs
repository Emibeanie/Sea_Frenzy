using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FishManager : MonoBehaviour
{
    [SerializeField] private TMP_Text fishCounterText;
    [SerializeField] private FishSpawner fishSpawner;
    [SerializeField] private GameObject fishingPanel;
    [SerializeField] GameObject closeButton;

    public static FishManager Instance { get; private set; }

    private int _fishCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_fishCount == fishSpawner.FishSpawnLimit)
            closeButton.SetActive(true);
    }

    public void AddFish()
    {
        _fishCount++;
        fishCounterText.text = $"Fish Caught: {_fishCount}/{fishSpawner.FishSpawnLimit}";
    }

    public void ClosePanel()
    {
        fishingPanel.SetActive(false);
    }
}
