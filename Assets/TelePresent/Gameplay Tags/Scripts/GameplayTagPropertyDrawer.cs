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

namespace TelePresent.GameplayTags
{
    [CustomPropertyDrawer(typeof(GameplayTag))]
    public class GameplayTagPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float objectFieldWidth = position.width * 0.95f;
            float spacing = 5f;
            float toggleWidth = position.width - objectFieldWidth - spacing;
            Rect objectFieldRect = new Rect(position.x, position.y, objectFieldWidth, EditorGUIUtility.singleLineHeight);
            Rect toggleRect = new Rect(position.x + objectFieldWidth + spacing, position.y, toggleWidth, EditorGUIUtility.singleLineHeight);

            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, label, property.objectReferenceValue, typeof(GameplayTag), false);

            GameplayTag tag = property.objectReferenceValue as GameplayTag;
            if (tag != null)
            {
                EditorGUI.BeginChangeCheck();
                bool newState = EditorGUI.Toggle(toggleRect, tag.TagActive);
                if (EditorGUI.EndChangeCheck())
                {
                    tag.SetTagState(newState);
                    EditorUtility.SetDirty(tag);
                }
            }
            else
            {
                EditorGUI.Toggle(toggleRect, false);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
