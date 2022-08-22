using System;
using UnityEngine;
using System.Collections.Generic;

namespace ElesJourney.Models
{
    [Serializable]
    public enum MechanismType
    {
        ASSOCIATION,
        PUZZLE,
        SPIN_WHEEL,
        SHADOW
    }

    [Serializable]
    public enum VoiceOverType
    {
        INTRO,
        GUIDE,
        EXTRA,
        CORRECT,
        INCORRECT,
        HINT
    }

    [Serializable]
    public class VoiceOvers
    {
        public VoiceOverType voiceOverType;
        public List<AudioClip> clips;
    }

    [Serializable]
    public class Word
    {
        public string word;
        public bool isGuided;
        public bool autoHint;
        public bool showLetter;
        public Sprite wordSprite;
        public List<int> wordIndexes;
        [Space]
        [Space]
        public List<VoiceOvers> voiceOvers;
    }


    [Serializable]
    public class MechanismsData
    {
        public MechanismType mechanismType;
        public List<Word> words;
    }
}
