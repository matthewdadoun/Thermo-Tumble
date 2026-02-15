using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)] // make sure it initializes early
public class GameManager : MonoBehaviour
{
    // The single instance of this game manager
    public static GameManager Instance { get; private set; }
    
    // Retrieve fade controller instance
    public FadeController FadeControllerInstance { get; set; }
    
    // Store the scene index between loading scenes
    private int _sceneIndex = 0;

    private void Awake()
    {
        // Check if there is an old existing instance
        if (Instance != null && Instance != this)
        {
            // Destroy the duplicate game object if it exists
            Destroy(gameObject);

            // Store scene
            var scene = SceneData.Instance?.SceneNames[_sceneIndex];

            // If the scene manager already has more than 1 scene, don't load the new scene
            if (SceneManager.sceneCount > 1)
            {
                return;
            }
            
            // Load the sub-scene
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            return;
        }
        
        // Store instance / don't destroy on load
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
