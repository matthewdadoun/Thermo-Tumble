using System;
using UnityEngine;

public class DeathVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Retrieve player status component
        var playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        
        // If player status is valid, kill the player
        playerStatus?.KillPlayer(true);
    }
}
