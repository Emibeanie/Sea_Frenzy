using UnityEngine;
using UnityEngine.UI;

public class FishBehavior : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnFishClicked);
    }

    private void OnFishClicked()
    {
        GameManager.Instance.AddFish();
        gameObject.SetActive(false);
    }
}
