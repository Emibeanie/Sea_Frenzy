using UnityEngine;
using UnityEngine.UI;

public class SailsManager : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    void Start()
    {
        progressBar.interactable = false;
    }
    public void UpSails()
    {
        progressBar.value += 0.05f;
    }
    public void DownSails()
    {
        progressBar.value -= 0.05f;
    }
}
