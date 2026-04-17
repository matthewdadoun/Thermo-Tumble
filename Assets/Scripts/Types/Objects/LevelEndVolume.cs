using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelEndVolume : MonoBehaviour
{
    public string mainMenuScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        var fadeInstance = GameManager.Instance.FadeControllerInstance;

        // Retrieve game manager instance / set up binding
        fadeInstance.OnFadeComplete += OnLevelFadeComplete;

        // Fade out to black
        fadeInstance.FadeOutToBlack(3f);

        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfxWin);

        // OnDestroyFadeComplete
        void OnLevelFadeComplete()
        {
            // Unbind event
            GameManager.Instance.FadeControllerInstance.OnFadeComplete -= OnLevelFadeComplete;

            // Reload the active scene
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}