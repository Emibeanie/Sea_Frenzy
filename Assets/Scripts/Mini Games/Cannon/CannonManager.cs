using UnityEngine;
using UnityEngine.UI;

public class CannonManager : MonoBehaviour
{
    private Button rockButton;

    void Start()
    {
        rockButton = GetComponent<Button>();
        rockButton.onClick.AddListener(OnRockClicked);
    }

    private void OnRockClicked()
    {
        gameObject.SetActive(false);
    }
}
