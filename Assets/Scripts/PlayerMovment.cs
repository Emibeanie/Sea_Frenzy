using Photon.Pun;
using UnityEngine;

public class PlayerMovment : MonoBehaviourPun
{
    [SerializeField] Animator animator;
    [SerializeField] float moveSpeed = 2f;

    private bool isWalking = false;

    void MovePlayer()
    {

        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        // Only allow movement if this player owns the PhotonView
        if (photonView.IsMine)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector2(horizontal, vertical).normalized * moveSpeed * Time.deltaTime;
            transform.position += movement;

            // Only send the walking animation RPC when the state changes (to avoid spamming the network)
            bool isCurrentlyWalking = movement != Vector3.zero;
            if (isCurrentlyWalking != isWalking)  // If walking state has changed
            {
                isWalking = isCurrentlyWalking;
                photonView.RPC("PlayWalkingAnimation", RpcTarget.All, isWalking);
            }
        }
    }

    [PunRPC]
    void PlayWalkingAnimation(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    private void Update()
    {
        MovePlayer();
    }
}
