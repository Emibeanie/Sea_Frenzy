using UnityEngine;
using UnityEngine.UI;

public class AnchorManager : MonoBehaviour
{
    [SerializeField] Slider upAnchor;
    [SerializeField] Slider downAnchor;
    [SerializeField] GameObject upCloseButton;
    [SerializeField] GameObject downCloseButton;
    [SerializeField] GameObject upPanel;
    [SerializeField] GameObject downPanel;

    private void OnEnable()
    {
        ResetMiniGame();
    }
    private void ResetMiniGame()
    {
        upAnchor.interactable = false;
        downAnchor.interactable = false;
        upAnchor.value = 1;
        downAnchor.value = 0;

        upCloseButton.SetActive(false);
        downCloseButton.SetActive(false);
    }

    void Update()
    {
        DownAnchorWhileSpacePressed();
        UpAnchorOnSpacePress();

        if (downAnchor.value == 1)
            downCloseButton.SetActive(true);

        if (upAnchor.value == 0)
            upCloseButton.SetActive(true);
    }

    private void DownAnchorWhileSpacePressed()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            downAnchor.value += 0.1f * Time.deltaTime;

            if (downAnchor.value > 1f)
            {
                downAnchor.value = 1f;
            }
        }
    }
    private void UpAnchorOnSpacePress()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            upAnchor.value -= 0.1f;

            if (upAnchor.value < 0f)
            {
                upAnchor.value = 0f;
            }
        }
    }

    public void CloseButton()
    {
        upPanel.SetActive(false);
        downPanel.SetActive(false);
    }
}
