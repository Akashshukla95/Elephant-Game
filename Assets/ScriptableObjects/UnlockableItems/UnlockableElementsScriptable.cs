using System;
using UnityEngine;
using System.Collections.Generic;
using K12.CommonDialogueSystem;

namespace ElesJourney.Scriptables
{
    [Serializable]
    public class ItemData
    {
        public string itemName;
        public Sprite itemSprite;
        public Dialogue dialogue;
        public int totalPointsToUnlock;
    }

    [Serializable]
    public class ItemUnlockScenes
    {
        public List<ItemData> unlockableItems;
    }

    [CreateAssetMenu(fileName = "New Ele's Items", menuName = "Items/ UnlockableItems")]
    public class UnlockableElementsScriptable : ScriptableObject
    {
        public List<ItemUnlockScenes> unlockData;
    }
}
