using UnityEngine;
using UnityEngine.UI;

public class UpAnchorManager : MonoBehaviour
{
    [SerializeField] Slider anchorSlider;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject panel;

    private void OnEnable()
    {
        ResetMiniGame();
    }

    private void ResetMiniGame()
    {
        anchorSlider.interactable = false;
        anchorSlider.value = 1f;

        closeButton.SetActive(false);
    }

    void Update()
    {
        UpAnchorOnSpacePress();

        if (anchorSlider.value == 0)
        {
            closeButton.SetActive(true);
        }
        else
        {
            closeButton.SetActive(false);
        }
    }

    private void UpAnchorOnSpacePress()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anchorSlider.value -= 0.1f;

            if (anchorSlider.value < 0f)
            {
                anchorSlider.value = 0f;
            }
        }
    }

    public void CloseButton()
    {
        panel.SetActive(false);
    }
}
