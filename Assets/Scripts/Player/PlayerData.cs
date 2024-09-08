using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public int health;
    public int score;

    public PlayerData(Vector3 pos, int hp, int sc)
    {
        position = pos;
        health = hp;
        score = sc;
    }
}
