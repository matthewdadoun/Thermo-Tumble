using UnityEngine;

[DefaultExecutionOrder(-1000)] // make sure it initializes early
public class GameManager : MonoBehaviour
{
    // The single instance of this game manager
    public static GameManager Instance { get; set; }
    
    // Retrieve fade controller instance
    public FadeController FadeControllerInstance { get; private set; }

    private void Awake()
    {
        // Check if there is an old existing instance
        if (Instance != null && Instance != this)
        {
            // Destroy the old lingering game object if the instance is not null
            Destroy(gameObject); 
            return;
        }
        
        // Store instance / don't destroy on load
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Grab the UI even if it's inactive
        FadeControllerInstance = FindAnyObjectByType<FadeController>();
    }
}
