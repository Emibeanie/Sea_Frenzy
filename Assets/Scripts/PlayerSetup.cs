using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField] PlayerMovment playerMovment;
    public void IsPlayerOwner()
    {
        playerMovment.enabled = true;
    }

    public void DisablePlayerOwner()
    {
        playerMovment.enabled = false;
    }

   
}
