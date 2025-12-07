/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TelePresent.GameplayTags
{
    [CustomEditor(typeof(GameplayTagManager))]
    public class GameplayTagManagerEditor : Editor
    {
        private SerializedProperty allTagsProperty;
        private Dictionary<int, bool> foldoutStates = new Dictionary<int, bool>();
        private Dictionary<int, bool> createTagStates = new Dictionary<int, bool>();
        private Dictionary<int, string> newTagNames = new Dictionary<int, string>();
        private Dictionary<int, bool> renameTagStates = new Dictionary<int, bool>();
        private const string IconsFolderPath = "Assets/TelePresent/Gameplay Tags/Editor/";
        private const string TelePresentLogoPath = IconsFolderPath + "telepresentsmallgrey.png";
        private Texture2D TelePresentIcon;
        private Texture2D docsIcon;
        private const string DocsIconPath = IconsFolderPath + "docs.png";

        private string searchFilter = "";
        private bool showSearch = false;
        private Vector2 scrollPos = Vector2.zero;

        private void OnEnable()
        {
            if (target == null) return;
            allTagsProperty = serializedObject.FindProperty("allTags");
            RestoreFoldoutStates();
            TelePresentIcon = LoadIcon(TelePresentLogoPath);
            docsIcon = LoadIcon(DocsIconPath);

            // Subscribe to the manager's event for any local tag changes.
            GameplayTagManager manager = (GameplayTagManager)target;
            manager.OnAnyManagerTagChanged += OnAnyLocalTagChanged;
        }
        private Texture2D LoadIcon(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private void OnDisable()
        {
            GameplayTagManager manager = target as GameplayTagManager;
            if (manager != null)
            {
                manager.OnAnyManagerTagChanged -= OnAnyLocalTagChanged;
            }
        }

        private void OnAnyLocalTagChanged(GameplayTag changedTag, bool newLocalState)
        {
            HandleLocalTagStateChanged();
        }

        private void HandleLocalTagStateChanged()
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GameplayTagManager manager = (GameplayTagManager)target;
            bool isLocal = !EditorUtility.IsPersistent(manager);

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gameplay Tag Hierarchy", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            DrawIconButton(docsIcon, 5, () =>
            {
                Application.OpenURL("https://telepresentgames.dk/Unity%20Asset/Gameplay%20Tags/Gameplay%20Tags%20Documentation.pdf");
            }, buttonWidth: 25f, buttonHeight: 25f, tooltip: "View Documentation");
            EditorGUILayout.Space();
            DrawIconButton(TelePresentIcon, 2, () =>
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/72893");
            }, buttonWidth: 25f, buttonHeight: 25f, tooltip: "Visit Publisher Page");
            EditorGUILayout.Space();

            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            float inspectorWidth = EditorGUIUtility.currentViewWidth;
            bool useAbbreviated = inspectorWidth < 400;

            GUILayoutOption buttonLayout = useAbbreviated ? GUILayout.MinWidth(1) : GUILayout.ExpandWidth(true);

            GUIContent addTagContent = new GUIContent(useAbbreviated ? "Add Tag" : "Add Root Tag", "Add a new root-level gameplay tag.");
            GUIContent expandAllContent = new GUIContent(useAbbreviated ? "Expand" : "Expand All", "Expand all gameplay tag nodes.");
            GUIContent collapseAllContent = new GUIContent(useAbbreviated ? "Collapse" : "Collapse All", "Collapse all gameplay tag nodes.");
            GUIContent toggleAllContent = new GUIContent(useAbbreviated ? "Toggle" : "Toggle All", "Toggle the active state of all gameplay tags.");
            GUIContent searchContent = new GUIContent(useAbbreviated ? "Search" : "Search Tags", "Show or hide the search field.");

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(7);
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.1f, 1.7f, 1.1f, 1f);

            if (GUILayout.Button(addTagContent, EditorStyles.toolbarButton, buttonLayout))
            {
                AddNewTag(null, -1);
            }
            GUI.backgroundColor = new Color(1.3f, 1.3f, 1.3f, 1.3f);

            if (GUILayout.Button(expandAllContent, EditorStyles.toolbarButton, buttonLayout))
            {
                SetAllFoldouts(true);
            }
            if (GUILayout.Button(collapseAllContent, EditorStyles.toolbarButton, buttonLayout))
            {
                SetAllFoldouts(false);
            }
            if (GUILayout.Button(toggleAllContent, EditorStyles.toolbarButton, buttonLayout))
            {
                ToggleAllTags(manager, isLocal);
            }
            showSearch = GUILayout.Toggle(showSearch, searchContent, EditorStyles.toolbarButton, buttonLayout);
            GUILayout.Space(7);
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = originalColor;

            if (showSearch)
            {
                searchFilter = EditorGUILayout.TextField("Search", searchFilter);
                EditorGUILayout.Space();
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                SerializedProperty parentTagIDProperty = tagNodeProperty.FindPropertyRelative("parentTagID");
                if (parentTagIDProperty.intValue == -1)
                {
                    DrawTagWithSubtags(manager, i, 0, isLocal);
                }
            }
            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }
        private void DrawIconButton(Texture2D icon, int padding, System.Action onClick, float buttonWidth = 80f, float buttonHeight = 30f, string tooltip = "")
        {
            // Define the button style
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 0, // Smaller font size
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(padding, padding, padding, padding),
                margin = new RectOffset(0, 0, 0, 0),
                fixedWidth = buttonWidth,
                fixedHeight = buttonHeight
            };

            // Create a GUIContent with icon and tooltip
            GUIContent content = new GUIContent(icon, tooltip);

            // Draw the button using the content with tooltip
            if (GUILayout.Button(content, buttonStyle))
            {
                onClick();
            }
        }

        private void DrawTagWithSubtags(GameplayTagManager manager, int index, int indentLevel, bool isLocal)
        {
            SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(index);
            SerializedProperty tagProperty = tagNodeProperty.FindPropertyRelative("tag");
            SerializedProperty tagIDProperty = tagNodeProperty.FindPropertyRelative("tagID");
            int tagID = tagIDProperty.intValue;
            GameplayTag tag = tagProperty.objectReferenceValue as GameplayTag;

            if (!ShouldDisplayTag(tagNodeProperty))
                return;

            if (!createTagStates.ContainsKey(tagID))
            {
                createTagStates[tagID] = false;
                newTagNames[tagID] = "";
            }
            if (!renameTagStates.ContainsKey(tagID))
            {
                renameTagStates[tagID] = false;
            }
            if (!foldoutStates.ContainsKey(tagID))
            {
                foldoutStates[tagID] = EditorPrefs.GetBool($"Foldout_{tagID}", false);
            }

            Color originalColor = GUI.backgroundColor;
            Color evenRowColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.85f, 0.85f, 0.85f);
            Color oddRowColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f) : new Color(0.75f, 0.75f, 0.75f);
            GUI.backgroundColor = (index % 2 == 0) ? evenRowColor : oddRowColor;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel * 15);

            bool hasSubtags = HasSubtags(tagID);
            if (hasSubtags)
            {
                string arrow = foldoutStates[tagID] ? " \u25BC" : " \u25BA";
                if (GUILayout.Button(arrow, EditorStyles.toolbarButton, GUILayout.Width(25)))
                {
                    foldoutStates[tagID] = !foldoutStates[tagID];
                    EditorPrefs.SetBool($"Foldout_{tagID}", foldoutStates[tagID]);
                }
            }
            else
            {
                GUILayout.Space(25);
            }
            GUILayout.Space(-7);

            if (isLocal)
            {
                SerializedProperty localActiveProp = tagNodeProperty.FindPropertyRelative("localActive");
                bool currentVal = localActiveProp.boolValue;
                bool newVal = EditorGUILayout.Toggle(currentVal, GUILayout.Width(30));
                if (newVal != currentVal)
                {
                    GameplayTag gameplayTag = tagProperty.objectReferenceValue as GameplayTag;
                    if (gameplayTag != null)
                    {
                        manager.SetLocalTagState(gameplayTag, newVal);
                    }
                    else
                    {
                        localActiveProp.boolValue = newVal;
                    }
                }
            }
            else
            {
                if (tag != null)
                {
                    bool newVal = EditorGUILayout.Toggle(tag.TagActive, GUILayout.Width(30));
                    if (newVal != tag.TagActive)
                    {
                        Undo.RecordObject(tag, "Toggle Tag State");
                        manager.SetLocalTagState(tag, newVal);
                        tag.SetTagState(newVal);
                        EditorUtility.SetDirty(tag);
                    }
                }
                else
                {
                    GUILayout.Space(20);
                }
            }
            GUILayout.Space(-10);

            tagProperty.objectReferenceValue = (GameplayTag)EditorGUILayout.ObjectField(
                tagProperty.objectReferenceValue,
                typeof(GameplayTag),
                false
            );

            if (isLocal)
            {
                SerializedProperty localActiveInitializedProp = tagNodeProperty.FindPropertyRelative("localActiveInitialized");
                if (!localActiveInitializedProp.boolValue)
                {
                    GameplayTag assignedTag = tagProperty.objectReferenceValue as GameplayTag;
                    if (assignedTag != null)
                    {
                        SerializedProperty localActiveProp = tagNodeProperty.FindPropertyRelative("localActive");
                        localActiveProp.boolValue = assignedTag.TagActive;
                        localActiveInitializedProp.boolValue = true;
                    }
                }
            }

            if (tagProperty.objectReferenceValue == null)
            {
                if (!createTagStates[tagID])
                {
                    if (GUILayout.Button("New Tag", EditorStyles.miniButton, GUILayout.Width(80)))
                    {
                        createTagStates[tagID] = true;
                    }
                }
                else
                {
                    newTagNames[tagID] = EditorGUILayout.TextField(newTagNames[tagID], GUILayout.Width(150));
                    if (GUILayout.Button("Save", EditorStyles.miniButton, GUILayout.Width(100)))
                    {
                        SaveNewTag(tagID, isLocal);
                    }
                }
            }

            if (GUILayout.Button("\u25BE", EditorStyles.miniButton, GUILayout.Width(25)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Subtag"), false, () => AddNewTag(tag, tagID));
                menu.AddItem(new GUIContent("Remove Tag"), false, () => RemoveTag(tagID));
                menu.AddItem(new GUIContent("Rename Tag"), false, () => StartRenameTag(tagID));
                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = originalColor;

            if (renameTagStates[tagID])
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space((indentLevel + 1) * 15 + 25);
                newTagNames[tagID] = EditorGUILayout.TextField(newTagNames[tagID]);
                if (GUILayout.Button("Rename", EditorStyles.miniButton, GUILayout.Width(100)))
                {
                    RenameTag(tagID, newTagNames[tagID]);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (foldoutStates[tagID])
            {
                for (int i = 0; i < allTagsProperty.arraySize; i++)
                {
                    SerializedProperty subTagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                    SerializedProperty parentTagIDProp = subTagNodeProperty.FindPropertyRelative("parentTagID");
                    if (parentTagIDProp.intValue == tagID)
                    {
                        DrawTagWithSubtags(manager, i, indentLevel + 1, isLocal);
                    }
                }
            }
        }

        private bool ShouldDisplayTag(SerializedProperty tagNodeProperty)
        {
            if (string.IsNullOrEmpty(searchFilter))
                return true;

            SerializedProperty tagProperty = tagNodeProperty.FindPropertyRelative("tag");
            GameplayTag tag = tagProperty.objectReferenceValue as GameplayTag;
            string tagName = tag != null ? tag.name : "";
            if (tagName.ToLower().Contains(searchFilter.ToLower()))
                return true;

            SerializedProperty tagIDProp = tagNodeProperty.FindPropertyRelative("tagID");
            int tagID = tagIDProp.intValue;
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty childNodeProp = allTagsProperty.GetArrayElementAtIndex(i);
                SerializedProperty parentTagIDProp = childNodeProp.FindPropertyRelative("parentTagID");
                if (parentTagIDProp.intValue == tagID)
                {
                    if (ShouldDisplayTag(childNodeProp))
                        return true;
                }
            }
            return false;
        }

        private bool HasSubtags(int parentTagID)
        {
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                SerializedProperty parentTagIDProperty = tagNodeProperty.FindPropertyRelative("parentTagID");
                if (parentTagIDProperty.intValue == parentTagID)
                    return true;
            }
            return false;
        }

        private void StartRenameTag(int tagID)
        {
            renameTagStates[tagID] = true;
        }

        private void RenameTag(int tagID, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                EditorUtility.DisplayDialog("Error", "New tag name cannot be empty!", "OK");
                return;
            }

            string sanitizedNewName = newName.Replace("æ", "ae").Replace("ø", "o").Replace(" ", "_");
            string invalidCharsPattern = @"[<>:""/\\|?*]";
            if (Regex.IsMatch(sanitizedNewName, invalidCharsPattern))
            {
                EditorUtility.DisplayDialog("Invalid Characters",
                    "The tag name contains invalid characters. Remove: <>:\"/\\|?*",
                    "OK");
                return;
            }

            SerializedProperty tagNodeProperty = FindTagNodeByID(tagID);
            if (tagNodeProperty == null)
                return;

            SerializedProperty tagProperty = tagNodeProperty.FindPropertyRelative("tag");
            GameplayTag tag = tagProperty.objectReferenceValue as GameplayTag;
            if (tag == null)
            {
                EditorUtility.DisplayDialog("Error", "Tag reference is missing.", "OK");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(tag);
            if (string.IsNullOrEmpty(assetPath) || !System.IO.File.Exists(assetPath))
            {
                EditorUtility.DisplayDialog("Error", "Asset not found for the selected tag.", "OK");
                return;
            }

            string newAssetPath = "Assets/Tags/" + sanitizedNewName + ".asset";
            if (AssetDatabase.LoadAssetAtPath<GameplayTag>(newAssetPath) != null)
            {
                EditorUtility.DisplayDialog("Error", "A tag with this name already exists.", "OK");
                return;
            }

            tag.name = sanitizedNewName;
            EditorUtility.SetDirty(tag);

            string error = AssetDatabase.RenameAsset(assetPath, sanitizedNewName);
            if (!string.IsNullOrEmpty(error))
            {
                EditorUtility.DisplayDialog("Error", "Failed to rename asset: " + error, "OK");
                return;
            }

            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            renameTagStates[tagID] = false;
        }

        private void SaveNewTag(int tagID, bool isLocal)
        {
            string newTagName = newTagNames[tagID];
            if (string.IsNullOrEmpty(newTagName))
            {
                EditorUtility.DisplayDialog("Error", "Tag name cannot be empty!", "OK");
                return;
            }

            string directory = "Assets/Tags";
            if (!AssetDatabase.IsValidFolder(directory))
            {
                AssetDatabase.CreateFolder("Assets", "Tags");
            }

            string assetPath = directory + "/" + newTagName + ".asset";
            if (AssetDatabase.LoadAssetAtPath<GameplayTag>(assetPath) != null)
            {
                EditorUtility.DisplayDialog("Error", "A tag with that name already exists!", "OK");
                return;
            }

            GameplayTag newTag = ScriptableObject.CreateInstance<GameplayTag>();
            newTag.name = newTagName;
            AssetDatabase.CreateAsset(newTag, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            serializedObject.Update();
            allTagsProperty = serializedObject.FindProperty("allTags");
            SerializedProperty tagNodeProperty = FindTagNodeByID(tagID);
            if (tagNodeProperty != null)
            {
                tagNodeProperty.FindPropertyRelative("tag").objectReferenceValue = newTag;
                if (isLocal)
                {
                    SerializedProperty localActiveProp = tagNodeProperty.FindPropertyRelative("localActive");
                    localActiveProp.boolValue = newTag.TagActive;
                    SerializedProperty localActiveInitializedProp = tagNodeProperty.FindPropertyRelative("localActiveInitialized");
                    localActiveInitializedProp.boolValue = true;
                }
            }
            else
            {
                Debug.LogError("Failed to find tag node with ID: " + tagID);
            }
            serializedObject.ApplyModifiedProperties();

            createTagStates[tagID] = false;
            newTagNames[tagID] = "";
            EditorUtility.SetDirty(target);
        }

        private SerializedProperty FindTagNodeByID(int tagID)
        {
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                SerializedProperty tagIDProperty = tagNodeProperty.FindPropertyRelative("tagID");
                if (tagIDProperty.intValue == tagID)
                    return tagNodeProperty;
            }
            return null;
        }

        private void RestoreFoldoutStates()
        {
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                SerializedProperty tagIDProperty = tagNodeProperty.FindPropertyRelative("tagID");
                int tagID = tagIDProperty.intValue;
                foldoutStates[tagID] = EditorPrefs.GetBool($"Foldout_{tagID}", false);
            }
        }

        private void SetAllFoldouts(bool expanded)
        {
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                int tagID = tagNodeProperty.FindPropertyRelative("tagID").intValue;
                foldoutStates[tagID] = expanded;
                EditorPrefs.SetBool($"Foldout_{tagID}", expanded);
            }
        }

        private void ToggleAllTags(GameplayTagManager manager, bool isLocal)
        {
            bool allEnabled = true;
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                if (isLocal)
                {
                    if (!tagNodeProperty.FindPropertyRelative("localActive").boolValue)
                    {
                        allEnabled = false;
                        break;
                    }
                }
                else
                {
                    GameplayTag tag = tagNodeProperty.FindPropertyRelative("tag").objectReferenceValue as GameplayTag;
                    if (tag != null && !tag.TagActive)
                    {
                        allEnabled = false;
                        break;
                    }
                }
            }

            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty tagNodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                if (isLocal)
                {
                    GameplayTag tag = tagNodeProperty.FindPropertyRelative("tag").objectReferenceValue as GameplayTag;
                    if (tag != null)
                    {
                        manager.SetLocalTagState(tag, !allEnabled);
                    }
                    else
                    {
                        tagNodeProperty.FindPropertyRelative("localActive").boolValue = !allEnabled;
                    }
                }
                else
                {
                    GameplayTag tag = tagNodeProperty.FindPropertyRelative("tag").objectReferenceValue as GameplayTag;
                    if (tag != null)
                    {
                        tag.SetTagState(!allEnabled);
                        EditorUtility.SetDirty(tag);
                    }
                }
            }
            EditorUtility.SetDirty(target);
        }

        private void AddNewTag(GameplayTag parentTag, int parentTagID)
        {
            allTagsProperty.arraySize++;
            SerializedProperty newTagNodeProperty = allTagsProperty.GetArrayElementAtIndex(allTagsProperty.arraySize - 1);
            int newID = GetNewTagID();
            newTagNodeProperty.FindPropertyRelative("tagID").intValue = newID;
            newTagNodeProperty.FindPropertyRelative("parentTagID").intValue = parentTagID;
            newTagNodeProperty.FindPropertyRelative("tag").objectReferenceValue = null;
            newTagNodeProperty.FindPropertyRelative("isTagEnabled").boolValue = true;
            newTagNodeProperty.FindPropertyRelative("localActive").boolValue = false;
            newTagNodeProperty.FindPropertyRelative("localActiveInitialized").boolValue = false;

            if (parentTagID != -1)
            {
                foldoutStates[parentTagID] = true;
                EditorPrefs.SetBool($"Foldout_{parentTagID}", true);
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private int GetNewTagID()
        {
            int maxID = -1;
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                int id = allTagsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("tagID").intValue;
                if (id > maxID)
                    maxID = id;
            }
            return maxID + 1;
        }

        private void RemoveTag(int tagID)
        {
            if (HasSubtags(tagID))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Warning: Subtags Detected",
                    "Deleting this tag will also remove its subtags. Proceed?",
                    "Delete",
                    "Cancel"
                );
                if (!confirmed)
                    return;
                RemoveSubtags(tagID);
            }

            serializedObject.Update();

            int indexToRemove = -1;
            for (int i = 0; i < allTagsProperty.arraySize; i++)
            {
                SerializedProperty nodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                int currentID = nodeProperty.FindPropertyRelative("tagID").intValue;
                if (currentID == tagID)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove >= 0)
            {
                int initialSize = allTagsProperty.arraySize;
                allTagsProperty.DeleteArrayElementAtIndex(indexToRemove);
                if (allTagsProperty.arraySize == initialSize)
                {
                    allTagsProperty.DeleteArrayElementAtIndex(indexToRemove);
                }
                EditorPrefs.DeleteKey($"Foldout_{tagID}");
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void RemoveSubtags(int parentTagID)
        {
            serializedObject.Update();
            for (int i = allTagsProperty.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty nodeProperty = allTagsProperty.GetArrayElementAtIndex(i);
                int currentParent = nodeProperty.FindPropertyRelative("parentTagID").intValue;
                if (currentParent == parentTagID)
                {
                    int childTagID = nodeProperty.FindPropertyRelative("tagID").intValue;
                    RemoveSubtags(childTagID);

                    int initialSize = allTagsProperty.arraySize;
                    allTagsProperty.DeleteArrayElementAtIndex(i);
                    if (allTagsProperty.arraySize == initialSize)
                    {
                        allTagsProperty.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
