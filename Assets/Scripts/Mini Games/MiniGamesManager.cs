using UnityEngine;
using UnityEngine.UI;

public class MiniGamesManager : MonoBehaviour
{
    [SerializeField] GameObject cannonCrosshair;
    [SerializeField] GameObject fishingHook;

    void Update()
    {
        if (cannonCrosshair.activeInHierarchy)
        {
            Cursor.visible = false;
            UpdateCrosshairPosition();
        }
        else if (fishingHook.activeInHierarchy)
        {
            Cursor.visible = false;
            UpdateHookPosition();
        }
        else
        {
            Cursor.visible = true;
        }
    }

    private void UpdateCrosshairPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        cannonCrosshair.transform.position = mousePosition;
    }

    private void UpdateHookPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        fishingHook.transform.position = mousePosition;
    }
}