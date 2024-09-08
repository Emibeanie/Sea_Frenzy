using UnityEngine;
using UnityEngine.UI;

public class FishBehaviorEmily : MonoBehaviour
{
    private Button fishButton;

    void Start()
    {
        fishButton = GetComponent<Button>();
        fishButton.onClick.AddListener(OnFishClicked);
    }

    private void OnFishClicked()
    {
        FishManagerEmily.Instance.AddFish();
        gameObject.SetActive(false);
    }
}
