using System;
using UnityEngine;

namespace ElesJourney.Models
{

    [Serializable]
    public enum Answer
    {
        CORRECT,
        INCORRECT
    }

    [Serializable]
    public class WordModel
    {
        public int worldId;
        public int wordIndex;
        public string word;
        public Sprite wordSprite;

        public WordModel(int _worldId, string _word, int _wordIndex, Sprite _wordSprite)
        {
            worldId = _worldId;
            wordIndex = _wordIndex;
            word = _word;
            wordSprite = _wordSprite;
        }
    }
}


