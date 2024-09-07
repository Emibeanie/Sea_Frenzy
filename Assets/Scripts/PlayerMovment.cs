using UnityEngine;
using Photon.Pun;

public class PlayerMovment : MonoBehaviourPun
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Animator animator;
    /*[SerializeField] float runSpeed = 5f;*/
    private PlayerData playerData = new PlayerData();

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (photonView.IsMine)
        {
            LoadPlayerData();
        }
    }

    void MovePlayer()
    {
        if (photonView.IsMine)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 movment = new Vector2(horizontal, vertical).normalized * moveSpeed * Time.deltaTime;
            transform.position += movment;

            //update player data for saving
            playerData.positionX = transform.position.x;
            playerData.positionY = transform.position.y;
            playerData.positionZ = transform.position.z;

            photonView.RPC("UpdatePlayerDataRPC", RpcTarget.Others, playerData.positionX, playerData.positionY, playerData.positionZ, horizontal != 0 || vertical != 0);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            MovePlayer();
            UpdateAnimation();
        }
    }
    void UpdateAnimation()
    {
        bool isWalking = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        animator.SetBool("isWalking", isWalking);
    }

    [PunRPC]
    void UpdatePlayerDataRPC(float posX, float posY, float posZ, bool isWalking)
    {
        transform.position = new Vector3(posX, posY, posZ);
        animator.SetBool("isWalking", isWalking);
    }

    public void SavePlayerData()
    {
        string jsonData = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", jsonData);
        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            string jsonData = PlayerPrefs.GetString("PlayerData");
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            transform.position = new Vector3(playerData.positionX, playerData.positionY, playerData.positionZ);
        }
    }
}
