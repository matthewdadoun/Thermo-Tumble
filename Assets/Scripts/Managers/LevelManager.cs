using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string[] additiveScenes; // Additive levels

    private void Start()
    {
        // Iterate through additive scenes
        foreach (var scene in additiveScenes)
        {
            // Check to see if level name is null
            if (!string.IsNullOrWhiteSpace(scene))
            {
                // Load scene
                StartCoroutine(LoadAdditiveIfNeeded(scene));
            }
        }
    }

    private static IEnumerator LoadAdditiveIfNeeded(string sceneName)
    {
        // First check to see if scene is loaded
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield break;
        }

        // Load the additive scene
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (op is { isDone: false })
        {
            yield return null;
        }
    }
}