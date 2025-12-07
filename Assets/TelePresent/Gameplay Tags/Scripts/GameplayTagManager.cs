/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TelePresent.GameplayTags
{
    [CreateAssetMenu(fileName = "NewGameplayTagManager", menuName = "Gameplay Tags/Gameplay Tag Manager")]
    public class GameplayTagManager : ScriptableObject
    {
        [HideInInspector]
        public bool isExpanded = false;

#if UNITY_EDITOR

#endif
        [HideInInspector]
        public GameplayTagManager originalAsset;
        [HideInInspector]
        public bool isLocalClone = false;
        [Tooltip("List of all Gameplay Tags and Subtags.")]
        public List<TagNode> allTags = new List<TagNode>();

        // Quick lookup dictionaries for performance.
        private Dictionary<int, List<TagNode>> parentToChildrenMap = new Dictionary<int, List<TagNode>>();
        private Dictionary<int, TagNode> tagLookup = new Dictionary<int, TagNode>();

        // Events
        // Event for a specific tag’s local state change.
        public event Action<GameplayTag, bool> OnManagerTagChanged;
        // Event that fires for any local tag change in this manager.
        public event Action<GameplayTag, bool> OnAnyManagerTagChanged;


        private void OnEnable()
        {
            BuildParentChildMap();
        }

        public void BuildParentChildMap()
        {
            parentToChildrenMap.Clear();
            tagLookup.Clear();

            foreach (var tagNode in allTags)
            {
                tagLookup[tagNode.tagID] = tagNode;

                if (!parentToChildrenMap.ContainsKey(tagNode.parentTagID))
                {
                    parentToChildrenMap[tagNode.parentTagID] = new List<TagNode>();
                }
                parentToChildrenMap[tagNode.parentTagID].Add(tagNode);
            }
        }

        public bool IsTagAndParentsActive(string tagName)
        {
            var foundNode = allTags.Find(node => node.tag != null && node.tag.name == tagName);
            if (foundNode != null && foundNode.isTagEnabled)
            {
                return CheckTagAndParentsActive(foundNode.tagID);
            }
            return false;
        }


        public bool IsTagAndParentsActive(GameplayTag tag)
        {
            if (tag == null)
                return false;

            var foundNode = allTags.Find(n => n.tag == tag);
            if (foundNode == null)
                return false;

            return CheckTagAndParentsActive(foundNode.tagID);
        }

        private bool CheckTagAndParentsActive(int tagID)
        {
            if (!tagLookup.TryGetValue(tagID, out TagNode tagNode) || tagNode.tag == null)
                return false;

            if (!tagNode.isTagEnabled)
                return false;

            if (isLocalClone)
            {
                if (!tagNode.localActiveInitialized)
                {
                    tagNode.localActive = tagNode.tag.TagActive;
                    tagNode.localActiveInitialized = true;
                }

                if (!tagNode.localActive)
                    return false;
            }
            else
            {
                if (!tagNode.tag.TagActive)
                    return false;
            }

            if (tagNode.parentTagID == -1)
                return true;

            return CheckTagAndParentsActive(tagNode.parentTagID);
        }

        /// <summary>
        /// Checks if only the single tag's local state is active (ignoring parent states).
        /// </summary>
        public bool IsTagLocallyActive(GameplayTag tag)
        {
            if (tag == null)
                return false;

            var foundNode = allTags.Find(n => n.tag == tag);
            if (foundNode == null)
                return false;

            return foundNode.localActive;
        }


        /// <summary>
        /// Returns a list of all subtags for the specified parent tag (by name).
        /// </summary>
        public List<GameplayTag> GetSubtags(string tagName)
        {
            var foundNode = allTags.Find(node => node.tag != null && node.tag.name == tagName);
            if (foundNode != null)
            {
                return CollectSubtags(foundNode.tagID);
            }
            return new List<GameplayTag>();
        }

        private List<GameplayTag> CollectSubtags(int parentTagID)
        {
            List<GameplayTag> subtags = new List<GameplayTag>();

            if (parentToChildrenMap.TryGetValue(parentTagID, out List<TagNode> children))
            {
                foreach (var subtagNode in children)
                {
                    if (subtagNode.tag != null)
                    {
                        subtags.Add(subtagNode.tag);
                        subtags.AddRange(CollectSubtags(subtagNode.tagID));
                    }
                }
            }
            return subtags;
        }

        /// <summary>
        /// Sets the tag's active state locally (this method respects local clone mode).
        /// Raises local state change events.
        /// </summary>
        public void SetLocalTagState(GameplayTag tag, bool active)
        {
            if (!isLocalClone)
            {
                tag?.SetTagState(active);

                OnManagerTagChanged?.Invoke(tag, active);
                OnAnyManagerTagChanged?.Invoke(tag, active);
                return;
            }
            var foundNode = allTags.Find(n => n.tag == tag);
            if (foundNode == null) return;

            if (!foundNode.localActiveInitialized)
            {
                foundNode.localActive = foundNode.tag.TagActive;
                foundNode.localActiveInitialized = true;
            }

            if (foundNode.localActive != active)
            {
                foundNode.localActive = active;
                OnManagerTagChanged?.Invoke(tag, active);
                OnAnyManagerTagChanged?.Invoke(tag, active);
            }
        }


        public void SetLocalTagState(string tagName, bool active)
        {
            var foundNode = allTags.Find(n => n.tag != null && n.tag.name == tagName);
            if (foundNode != null)
            {
                SetLocalTagState(foundNode.tag, active);
            }
        }


        public void ToggleLocalTagState(GameplayTag tag)
        {
            if (!isLocalClone)
            {
                tag?.SetTagState(!tag.TagActive);
                OnManagerTagChanged?.Invoke(tag, tag.TagActive);
                OnAnyManagerTagChanged?.Invoke(tag, tag.TagActive);
                return;
            }
            var foundNode = allTags.Find(n => n.tag == tag);
            if (foundNode == null) return;

            if (!foundNode.localActiveInitialized)
            {
                foundNode.localActive = foundNode.tag.TagActive;
                foundNode.localActiveInitialized = true;
            }

            bool newLocalState = !foundNode.localActive;
            foundNode.localActive = newLocalState;

            // Fire events for the change.
            OnManagerTagChanged?.Invoke(tag, newLocalState);
            OnAnyManagerTagChanged?.Invoke(tag, newLocalState);
        }


        /// <summary>
        /// Finds and returns the tag node for a given tag.
        /// </summary>
        public TagNode FindTagNode(GameplayTag tag)
        {
            if (tag == null)
                return null;
            return allTags.Find(n => n.tag == tag);
        }

        /// <summary>
        /// Finds and returns the tag node for a given tag name.
        /// </summary>
        public TagNode FindTagNode(string tagName)
        {
            return allTags.Find(node => node.tag != null && node.tag.name == tagName);
        }

        [System.Serializable]
        public class TagNode
        {
            [Tooltip("The Gameplay Tag asset (used for its active state).")]
            public GameplayTag tag;

            [Tooltip("Unique identifier for this tag node.")]
            public int tagID;

            [Tooltip("Identifier for the parent tag node. -1 indicates no parent (root tag).")]
            public int parentTagID = -1;

            [Tooltip("Indicates whether this tag node is enabled. If false, the tag (and its children) are not considered active.")]
            public bool isTagEnabled = true;

            [Tooltip("Local active state (only used when changes are local).")]
            public bool localActive;

            [HideInInspector]
            public bool localActiveInitialized = false;
        }
    }
}
