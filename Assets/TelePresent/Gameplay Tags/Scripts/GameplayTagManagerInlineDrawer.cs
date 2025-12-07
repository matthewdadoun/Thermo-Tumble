/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TelePresent.GameplayTags
{
    [CustomPropertyDrawer(typeof(GameplayTagManager))]
    public class GameplayTagManagerInlineDrawer : PropertyDrawer
    {
        private Editor cachedEditor;
        private static Dictionary<string, bool> modeSelections = new Dictionary<string, bool>();
        private const float PADDING = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float y = position.y + PADDING;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float paddedWidth = position.width - PADDING * 2;

            // Foldout button style
            GUIStyle foldoutButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.95f, 0.95f, 0.95f) },
                hover = { textColor = Color.white },
                active = { textColor = new Color(0.85f, 0.85f, 0.85f) },
                padding = new RectOffset(8, 8, 0, 0),
                fixedHeight = lineHeight + 2f
            };

            bool foldout = property.objectReferenceValue is GameplayTagManager tagManager && tagManager.isExpanded;
            Color foldoutColor = foldout ? new Color(0.15f, 0.4f, 0.55f, 0.95f) : new Color(0.2f, 0.2f, 0.2f, 0.9f);
            Rect foldoutRect = new Rect(position.x + PADDING, y, paddedWidth * 0.5f, lineHeight + 2f);
            EditorGUI.DrawRect(foldoutRect, foldoutColor);

            string foldoutLabel = $"{(foldout ? "▼" : "►")} {label.text}";
            bool newFoldout = GUI.Toggle(foldoutRect, foldout, foldoutLabel, foldoutButtonStyle);
            if (property.objectReferenceValue is GameplayTagManager tagMgr)
                tagMgr.isExpanded = newFoldout;

            Rect objectFieldRect = new Rect(position.x + PADDING + paddedWidth * 0.5f, y, paddedWidth * 0.5f, lineHeight);
            EditorGUI.DrawRect(objectFieldRect, new Color(0.15f, 0.15f, 0.15f, 0.85f));
            EditorGUI.PropertyField(objectFieldRect, property, GUIContent.none, false);

            y += lineHeight + 4f;

            if (newFoldout)
            {
                Rect expandedBgRect = new Rect(position.x + PADDING, y - 2, paddedWidth, position.height - (y - position.y) + PADDING);
                EditorGUI.DrawRect(expandedBgRect, new Color(0.18f, 0.18f, 0.18f, 0.95f));

                bool isLocal = false;
                if (property.objectReferenceValue is GameplayTagManager mgr)
                    isLocal = mgr.isLocalClone;
                else if (modeSelections.ContainsKey(property.propertyPath))
                    isLocal = modeSelections[property.propertyPath];
                else
                    modeSelections[property.propertyPath] = false;

                GUIStyle toggleStyle = new GUIStyle(EditorStyles.miniButton)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = isLocal ? new Color(0.95f, 0.95f, 0.95f) : new Color(0.85f, 0.85f, 0.85f) },
                    hover = { textColor = Color.white },
                    active = { textColor = Color.white },
                    padding = new RectOffset(8, 8, 0, 0),
                    fixedHeight = lineHeight + 2f
                };

                Color toggleColor = isLocal ? new Color(0.2f, 0.3f, 0.2f, 0.95f) : new Color(0.25f, 0.25f, 0.25f, 0.95f);
                Rect toggleRect = new Rect(position.x + PADDING, y, paddedWidth, lineHeight + 2f);
                EditorGUI.DrawRect(toggleRect, toggleColor);

                GUIContent toggleContent = new GUIContent(
                    isLocal ? "Local Mode" : "Global Mode",
                    "Toggle between Local and Global modes"
                );
                bool newToggle = GUI.Toggle(toggleRect, isLocal, toggleContent, toggleStyle);
                if (newToggle != isLocal)
                {
                    modeSelections[property.propertyPath] = newToggle;
                    isLocal = newToggle;
                    if (!isLocal && property.objectReferenceValue is GameplayTagManager currentManager &&
                        !EditorUtility.IsPersistent(currentManager) && currentManager.originalAsset != null)
                    {
                        // Reverting to global: push any changes into the original asset,
                        // then revert the property back to that asset.
                        currentManager.originalAsset.allTags = currentManager.allTags;
                        property.objectReferenceValue = currentManager.originalAsset;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                y += lineHeight + 6f;

                // Clone or revert the manager if needed
                if (property.objectReferenceValue is GameplayTagManager manager)
                {
                    if (isLocal)
                    {
                        // Switching to local mode and the assigned manager is a persistent asset:
                        // clone it so we can modify local states without affecting global
                        if (EditorUtility.IsPersistent(manager))
                        {
                            GameplayTagManager localInstance = ScriptableObject.CreateInstance<GameplayTagManager>();
                            EditorUtility.CopySerialized(manager, localInstance);
                            localInstance.hideFlags = HideFlags.None;
#if UNITY_EDITOR
                            localInstance.originalAsset = manager;
                            localInstance.isLocalClone = true;
#endif
                            // Make sure we rebuild the lookup dictionaries on the clone
                            localInstance.BuildParentChildMap();

                            property.objectReferenceValue = localInstance;
                            property.serializedObject.ApplyModifiedProperties();
                            manager = localInstance;
                        }
                    }
                    else
                    {
                        // Switching back to global mode
                        if (!EditorUtility.IsPersistent(manager) && manager.originalAsset != null)
                        {
                            property.objectReferenceValue = manager.originalAsset;
                            property.serializedObject.ApplyModifiedProperties();
                            manager = property.objectReferenceValue as GameplayTagManager;
                        }
                    }
                }

                // Draw inline inspector for whichever instance we’re using
                if (cachedEditor == null || cachedEditor.target != property.objectReferenceValue)
                    cachedEditor = Editor.CreateEditor(property.objectReferenceValue);

                EditorGUI.indentLevel++;
                Rect inspectorRect = new Rect(position.x + PADDING + 4, y, paddedWidth - 4, position.height - (y - position.y));
                cachedEditor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // We keep the drawer collapsed height a bit bigger so there's room for two lines
            return EditorGUIUtility.singleLineHeight * 2f + PADDING * 2;
        }
    }
}
#endif
