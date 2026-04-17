using System;
using UnityEngine;

public class DeathVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Retrieve player status component
        var playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        
        // Stop music
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfxFail);
        
        // If player status is valid, kill the player
        playerStatus?.KillPlayer(true);
    }
}
