using UnityEngine;
using UnityEngine.UI;

public class SailsManager : MonoBehaviour
{
    [SerializeField] Slider upProgressBar;
    [SerializeField] Slider downProgressBar;
    [SerializeField] GameObject upCloseButton;
    [SerializeField] GameObject downCloseButton;
    [SerializeField] GameObject upPanel;
    [SerializeField] GameObject downPanel;
    void Start()
    {
        upProgressBar.interactable = false;
        downProgressBar.interactable = false;

        upProgressBar.value = 0f;
        downProgressBar.value = 1f;
    }

    private void Update()
    {
        if(upProgressBar.value == 1)
            upCloseButton.SetActive(true);

        if (downProgressBar.value == 0)
            downCloseButton.SetActive(true);

    }
    public void UpSails()
    {
        upProgressBar.value += 0.05f;
    }
    public void DownSails()
    {
        downProgressBar.value -= 0.05f;
    }

    public void closeButton()
    {
        upPanel.SetActive(false);
        downPanel.SetActive(false);
    }
}
