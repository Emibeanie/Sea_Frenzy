using UnityEngine;
using UnityEngine.UI;

public class DownAnchorManager : MonoBehaviour
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
        anchorSlider.value = 0f;

        closeButton.SetActive(false);
    }

    void Update()
    {
        DownAnchorWhileSpacePressed();

        if (anchorSlider.value == 1)
        {
            closeButton.SetActive(true);
        }
        else
        {
            closeButton.SetActive(false);
        }
    }

    private void DownAnchorWhileSpacePressed()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            anchorSlider.value += 0.1f * Time.deltaTime;

            if (anchorSlider.value > 1f)
            {
                anchorSlider.value = 1f;
            }
        }
    }

    public void CloseButton()
    {
        panel.SetActive(false);
    }
}
