using UnityEngine;
using UnityEngine.UI;

public class RockButtons : MonoBehaviour
{
    private Button _rockButton;

    void Start()
    {
        _rockButton = GetComponent<Button>();
        _rockButton.onClick.AddListener(OnRockClicked);
    }

    private void OnRockClicked()
    {
        gameObject.SetActive(false);
    }
}
