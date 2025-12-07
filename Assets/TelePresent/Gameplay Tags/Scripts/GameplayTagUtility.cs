/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

using UnityEngine;

namespace TelePresent.GameplayTags
{

    public static class GameplayTagUtility
    {
        /// <summary>
        /// Checks if a standalone tag is active.
        /// </summary>
        public static bool IsTagActive(GameplayTag tag)
        {
            if (tag == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTag is null!");
                return false;
            }
            return tag.IsActive();
        }

        /// <summary>
        /// Checks if a tag is active, including all its parent tags.
        /// </summary>
        public static bool IsTagAndParentsActive(GameplayTagManager manager, GameplayTag tag)
        {
            if (manager == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTagManager is null!");
                return false;
            }
            return manager.IsTagAndParentsActive(tag);
        }

        /// <summary>
        /// Checks if a tag (by name) is active, including all its parent tags.
        /// </summary>
        public static bool IsTagAndParentsActive(GameplayTagManager manager, string tagName)
        {
            if (manager == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTagManager is null!");
                return false;
            }
            return manager.IsTagAndParentsActive(tagName);
        }

        /// <summary>
        /// Checks if only the tag's own (local) active state is set, ignoring parent tags.
        /// </summary>
        public static bool IsTagLocallyActive(GameplayTagManager manager, GameplayTag tag)
        {
            if (manager == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTagManager is null!");
                return false;
            }
            return manager.IsTagLocallyActive(tag);
        }


        /// <summary>
        /// Toggles the active state of a standalone tag.
        /// </summary>
        public static void SetTagState(GameplayTag tag, bool active)
        {
            if (tag == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTag is null!");
                return;
            }
            tag.SetTagState(active);
        }

        /// <summary>
        /// Toggles the active state of a tag within a manager context (uses local mode if applicable).
        /// </summary>
        public static void SetLocalTagState(GameplayTagManager manager, GameplayTag tag, bool active)
        {
            if (manager == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTagManager is null!");
                return;
            }
            manager.SetLocalTagState(tag, active);
        }

        public static void ToggleTagState(GameplayTag tag)
        {
            if (tag == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTag is null!");
                return;
            }
            tag.ToggleTagState();
        }

        public static void ToggleLocalTagState(GameplayTagManager manager, GameplayTag tag)
        {
            if (manager == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTagManager is null!");
                return;
            }
            manager.ToggleLocalTagState(tag);
        }



        public static System.Collections.Generic.List<GameplayTag> GetSubtags(GameplayTagManager manager, string tagName)
        {
            if (manager == null)
            {
                Debug.LogError("GameplayTagUtility: Provided GameplayTagManager is null!");
                return new System.Collections.Generic.List<GameplayTag>();
            }
            return manager.GetSubtags(tagName);
        }
    }
}
