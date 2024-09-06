using UnityEngine;

public class CannonManager : MonoBehaviour
{
    [SerializeField] GameObject cannonPanel;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject[] rocks;
    private void OnEnable()
    {
        ResetMiniGame();
    }
    private void ResetMiniGame()
    {
        CheckRocksAndPanel();
        closeButton.SetActive(false);
    }
    private void CheckRocksAndPanel()
    {
        if (cannonPanel.activeSelf)
        {
            bool allRocksInactive = true;
            foreach (GameObject rock in rocks)
            {
                if (rock.activeSelf)
                {
                    allRocksInactive = false;
                    break;
                }
            }

            if (allRocksInactive)
                closeButton.SetActive(true);
        }
    }

    public void CloseButton()
    {
        cannonPanel.SetActive(false);
    }
}
