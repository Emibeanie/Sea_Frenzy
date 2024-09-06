using UnityEngine;
using Photon.Pun;

public class PlayerMovment : MonoBehaviourPun
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Animator animator;
    /*[SerializeField] float runSpeed = 5f;*/

    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movment = new Vector2(horizontal, vertical).normalized * moveSpeed * Time.deltaTime;
        transform.position += movment;
    }

    private void Update()
    {
       MovePlayer();
       UpdateAnimation();
    }
    void UpdateAnimation()
    {
        bool isWalking = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        animator.SetBool("IsWalking", isWalking);
    }
}
