using System;
using UnityEngine;
using System.Collections.Generic;

namespace K12.CommonDialogueSystem
{
    [Serializable]
    public struct Line
    {
        public Character character;
        [TextArea(3, 5)]
        public string text;
    }

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "DialogueSystem/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public List<Line> lines = new List<Line>();
    }
}