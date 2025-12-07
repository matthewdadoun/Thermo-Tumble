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
    [InitializeOnLoad]
    public static class GameplayTagStateResetter
    {
        private static Dictionary<GameplayTag, bool> originalGlobalTagStates = new Dictionary<GameplayTag, bool>();
        private static Dictionary<GameplayTagManager, Dictionary<int, bool>> originalLocalTagStates = new Dictionary<GameplayTagManager, Dictionary<int, bool>>();

        static GameplayTagStateResetter()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                CacheGlobalTagStates();
                CacheLocalTagStates();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                RestoreGlobalTagStates();
                RestoreLocalTagStates();
            }
        }

        private static void CacheGlobalTagStates()
        {
            originalGlobalTagStates.Clear();
            GameplayTag[] globalTags = Resources.FindObjectsOfTypeAll<GameplayTag>();
            foreach (GameplayTag tag in globalTags)
            {
                if (tag != null)
                {
                    originalGlobalTagStates[tag] = tag.TagActive;
                }
            }
        }

        private static void CacheLocalTagStates()
        {
            originalLocalTagStates.Clear();
            GameplayTagManager[] managers = Resources.FindObjectsOfTypeAll<GameplayTagManager>();
            foreach (GameplayTagManager manager in managers)
            {
                Dictionary<int, bool> managerStates = new Dictionary<int, bool>();
                foreach (var node in manager.allTags)
                {
                    managerStates[node.tagID] = node.localActive;
                }
                originalLocalTagStates[manager] = managerStates;
            }
        }

        private static void RestoreGlobalTagStates()
        {
            foreach (var kvp in originalGlobalTagStates)
            {
                GameplayTag tag = kvp.Key;
                if (tag != null)
                {
                    tag.TagActive = kvp.Value;
                    EditorUtility.SetDirty(tag);
                }
            }
        }

        private static void RestoreLocalTagStates()
        {
            foreach (var managerKvp in originalLocalTagStates)
            {
                GameplayTagManager manager = managerKvp.Key;
                if (manager != null)
                {
                    Dictionary<int, bool> nodeStates = managerKvp.Value;
                    foreach (var node in manager.allTags)
                    {
                        if (nodeStates.TryGetValue(node.tagID, out bool originalLocalActive))
                        {
                            node.localActive = originalLocalActive;
                        }
                    }
                    EditorUtility.SetDirty(manager);
                }
            }
        }
    }
}
#endif
