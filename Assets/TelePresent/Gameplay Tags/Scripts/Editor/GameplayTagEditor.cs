#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TelePresent.GameplayTags
{
    [CustomEditor(typeof(GameplayTag))]
    public class GameplayTagEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GameplayTag tag = (GameplayTag)target;

            EditorGUILayout.LabelField("Gameplay Tag Editor", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Use this asset to define a custom Gameplay Tag. " +
                "Changing the tag state here will affect anything subscribing to OnTagChanged.",
                MessageType.Info
            );

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Tag Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            bool newState = EditorGUILayout.Toggle(
                new GUIContent("Tag Active", "Whether this tag is currently active."),
                tag.TagActive
            );

            if (EditorGUI.EndChangeCheck())
            {
                tag.SetTagState(newState);
                EditorUtility.SetDirty(tag);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();


        }
    }
}
#endif
