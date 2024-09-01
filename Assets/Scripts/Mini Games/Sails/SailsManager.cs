using UnityEngine;
using UnityEngine.UI;

public class SailsManager : MonoBehaviour
{
    [SerializeField] Slider upProgressBar;
    [SerializeField] Slider downProgressBar;
    void Start()
    {
        upProgressBar.interactable = false;
        downProgressBar.interactable = false;

        upProgressBar.value = 0f;
        downProgressBar.value = 1f;
    }
    public void UpSails()
    {
        upProgressBar.value += 0.05f;
    }
    public void DownSails()
    {
        downProgressBar.value -= 0.05f;
    }
}
