using UnityEngine;
using UnityEngine.UI;

public class UpSailsManager : MonoBehaviour
{
    [SerializeField] Slider sailSlider;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject panel;

    private void OnEnable()
    {
        ResetMiniGame();
    }

    private void ResetMiniGame()
    {
        sailSlider.interactable = false;
        sailSlider.value = 0f;
        closeButton.SetActive(false);
    }

    private void Update()
    {
        if (sailSlider.value == 1f)
            closeButton.SetActive(true);
    }

    public void UpSailsButton()
    {
        sailSlider.value += 0.05f;
    }

    public void CloseButton()
    {
        panel.SetActive(false);
    }
}
