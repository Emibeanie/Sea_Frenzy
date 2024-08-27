using UnityEngine;
using UnityEngine.UI;

public class FishBehavior : MonoBehaviour
{
    private Button fishButton;

    void Start()
    {
        fishButton = GetComponent<Button>();
        fishButton.onClick.AddListener(OnFishClicked);
    }

    private void OnFishClicked()
    {
        FishManager.Instance.AddFish();
        gameObject.SetActive(false);
    }
}
