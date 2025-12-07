/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

using UnityEngine;
using System;

namespace TelePresent.GameplayTags
{
    [CreateAssetMenu(fileName = "NewGameplayTag", menuName = "Gameplay Tags/Gameplay Tag")]
    public class GameplayTag : ScriptableObject
    {
        [Tooltip("Indicates whether the player has this tag.")]
        public bool TagActive;

        // Event raised when the global (asset) state changes.
        public event Action<bool> OnTagChanged;

        public bool IsActive() => TagActive;

        public void SetTagState(bool state)
        {
            if (TagActive != state)
            {
                TagActive = state;
                OnTagChanged?.Invoke(state);
            }
        }

        public void ToggleTagState()
        {
            SetTagState(!TagActive);
        }
    }
}
