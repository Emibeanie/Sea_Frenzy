using UnityEngine;

public class MiniGame : MonoBehaviour
{
    int miniGameCurrentStation;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddTeamScore();
            GameManager.Instance._miniGameSpawner.clearMininGame(miniGameCurrentStation);
            Destroy(gameObject);
        }
    }

    public void SetMiniGameCurrentStation(int spawnStation)
    {
        miniGameCurrentStation = spawnStation;
    }
}
