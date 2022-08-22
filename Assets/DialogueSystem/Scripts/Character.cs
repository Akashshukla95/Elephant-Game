using UnityEngine;

namespace K12.CommonDialogueSystem
{
    [CreateAssetMenu(fileName = "New Character", menuName = "DialogueSystem/Character")]

    #region ------------- Public Methods ----------------------
    public class Character : ScriptableObject
    {
        public Sprite characterSprite;
        public string fullName;
        public CharacterOnScreen characterSide;
    }
    #endregion

    #region ------------- Enums ----------------------
    public enum CharacterOnScreen
    {
        LEFT,
        RIGHT
    }
    #endregion
}