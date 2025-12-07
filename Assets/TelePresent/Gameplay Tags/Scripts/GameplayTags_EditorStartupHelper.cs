/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace TelePresent.GameplayTags
{
    [Serializable]
    public class GameplayTags_EditorStartupHelper : ScriptableObject
    {
        private static GameplayTags_EditorStartupHelper singletonInstance;

        public static GameplayTags_EditorStartupHelper Singleton
        {
            get
            {
                if (singletonInstance == null)
                {
                    singletonInstance = Resources.Load<GameplayTags_EditorStartupHelper>("GameplayTags_EditorStartupHelper");
                    if (singletonInstance == null)
                    {
                        singletonInstance = CreateInstance<GameplayTags_EditorStartupHelper>();
                    }
                }
                return singletonInstance;
            }
        }

        [SerializeField] private bool displayWelcomeMessageOnLaunch = true;
        [SerializeField] private bool firstInitialization = true;

        public static bool DisplayWelcomeOnLaunch
        {
            get => Singleton.displayWelcomeMessageOnLaunch;
            set
            {
                if (value != Singleton.displayWelcomeMessageOnLaunch)
                {
                    Singleton.displayWelcomeMessageOnLaunch = value;
                    PersistStartupPreferences();
                }
            }
        }

        public static bool FirstInitialization
        {
            get => Singleton.firstInitialization;
            set
            {
                if (value != Singleton.firstInitialization)
                {
                    Singleton.firstInitialization = value;
                    PersistStartupPreferences();
                }
            }
        }

        public static void PersistStartupPreferences()
        {
            if (!AssetDatabase.Contains(Singleton))
            {
                var temporaryCopy = CreateInstance<GameplayTags_EditorStartupHelper>();
                EditorUtility.CopySerialized(Singleton, temporaryCopy);

                string assetPath = "Assets/TelePresent/Gameplay Tags/Scripts/Resources/GameplayTags_EditorStartupHelper.asset";

                singletonInstance = Resources.Load<GameplayTags_EditorStartupHelper>("GameplayTags_EditorStartupHelper");
                if (singletonInstance == null)
                {
                    Debug.Log("Creating new GameplayTags_EditorStartupHelper asset");
                    AssetDatabase.CreateAsset(temporaryCopy, assetPath);
                    AssetDatabase.Refresh();
                    singletonInstance = temporaryCopy;
                    return;
                }
                EditorUtility.CopySerialized(temporaryCopy, singletonInstance);
            }
            EditorUtility.SetDirty(Singleton);
        }
    }
}
#endif