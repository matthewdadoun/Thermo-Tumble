using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SceneData", menuName = "Scriptable Objects/SceneData")]
public class SceneData : ScriptableObject
{
#if UNITY_EDITOR
    [Header("Editor: drag scene assets here")]
    [SerializeField] private SceneAsset[] scenes;
#endif

    [Header("Runtime: generated from scenes (do not edit)")]
    [SerializeField, HideInInspector] private string[] sceneNames; // or scenePaths
    public IReadOnlyList<string> SceneNames => sceneNames;

    private const string ResourcePath = "SceneDB";
    private static SceneData _instance;

    public static SceneData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<SceneData>(ResourcePath);
            }

            return _instance;
        }
    }
    
    public string GetSceneName(string sceneName)
    {
        return sceneNames?.FirstOrDefault(t => t == sceneName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (scenes == null)
        {
            sceneNames = null;
            return;
        }

        sceneNames = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            sceneNames[i] = scenes[i] ? scenes[i].name : string.Empty;
            // If you prefer path instead of name:
            // sceneNames[i] = scenes[i] ? AssetDatabase.GetAssetPath(scenes[i]) : string.Empty;
        }

        EditorUtility.SetDirty(this);
    }
#endif
}