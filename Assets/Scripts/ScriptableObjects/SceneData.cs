using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "Scriptable Objects/SceneData")]
public class SceneData : ScriptableObject
{
    // Dictionary lookup used to find material
    [SerializeField] public SceneAsset[] scenes;
    
    // Simple singleton-style access with lazy load
    public const string ResourcePath = "SceneDB";
    private static SceneData _instance;

    public static SceneData Instance
    {
        get
        {
            if (_instance == null)
            {
                // Load from resources
                _instance = Resources.Load<SceneData>(ResourcePath);
            }

            // Retrieve instance
            return _instance;
        }
    }
}