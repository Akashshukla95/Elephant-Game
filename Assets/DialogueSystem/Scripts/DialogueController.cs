using System;
using UnityEngine;
using ElesJourney.Managers;

namespace K12.CommonDialogueSystem
{
    public class DialogueController : MonoBehaviour
    {
        #region --------------------------------------- Private variables ---------------------------------------
        #region SerializeField
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private GameObject speakerLeft;
        [SerializeField] private GameObject speakerRight;
        [SerializeField] private DialogueSettings dialogueSettings = null;
        [SerializeField] private AudioClip dialogBoxClip;
        #endregion
        #region Non-SerializeField
        private SpeakerView speakerUILeft;
        private SpeakerView speakerUIRight;
        private SpeakerView currentActiveSpeaker = null;
        private int activeLineIndex = 0;
        private bool canStartDialogue = false;
        private Action callerScriptCallback = null;
        #endregion
        #endregion ---------------------------------------
        #region --------------------------------------- Public variables ---------------------------------------
        #endregion ---------------------------------------
        #region ---------------------------- Private Methods ----------------------------
        #region -------------- Monobehaviour --------------
        private void Awake()
        {
            speakerUILeft = speakerLeft.GetComponent<SpeakerView>();
            speakerUIRight = speakerRight.GetComponent<SpeakerView>();

            speakerUIRight.SetFont(dialogueSettings.dialogueSystemFont);
            speakerUILeft.SetFont(dialogueSettings.dialogueSystemFont);

            //if (dialogue.characterLeft != null) speakerUILeft.Speaker = dialogue.characterLeft;
            //if (dialogue.characterRight != null) speakerUIRight.Speaker = dialogue.characterRight;
        }
        // private void Update()
        // {
        //     if (Input.GetMouseButtonDown(0) && canStartDialogue)
        //     {
        //         StartDialogue(null);
        //     }
        // }
        #endregion ----------------------------
        #region -------------- Non-Monobehaviour --------------
        private void AdvanceToNextLine()
        {
            if (activeLineIndex >= 0 && activeLineIndex < dialogue.lines.Count)
            {
                HandleDialogueShow();
            }
            else
            {
                HandleDialogueEnd();
            }
        }
        private void DisplayLine()
        {
            Line line = dialogue.lines[activeLineIndex];
            Character character = line.character;

            if (character.characterSide == CharacterOnScreen.LEFT)
            {
                speakerUILeft.Speaker = character;
                SetDialogue(speakerUILeft, speakerUIRight, line.text);
            }
            else if (character.characterSide == CharacterOnScreen.RIGHT)
            {
                speakerUIRight.Speaker = character;
                SetDialogue(speakerUIRight, speakerUILeft, line.text);
            }
        }
        private void SetDialogue(
            SpeakerView activeSpeakerUI,
            SpeakerView inactiveSpeakerUI,
            string text
            )
        {
            inactiveSpeakerUI.ShowSpeaker(false);
            activeSpeakerUI.Dialogue = text;
            activeSpeakerUI.ShowSpeaker(true);
            currentActiveSpeaker = activeSpeakerUI;
        }
        private void HandleDialogueShow()
        {
            DisplayLine();
            activeLineIndex += 1;
        }
        private void HandleDialogueEnd()
        {
            canStartDialogue = false;
            speakerUILeft.ShowSpeaker(false);
            speakerUIRight.ShowSpeaker(false);
            activeLineIndex = 0;
            currentActiveSpeaker = null;
            if (callerScriptCallback != null) callerScriptCallback();
            callerScriptCallback = null;
            this.gameObject.SetActive(false);
        }
        #endregion ----------------------------
        #endregion --------------------------------------------------------
        #region ---------------------------- Public Methods ----------------------------
        public void StartDialogue(Dialogue dialogue, Action callback)
        {
            this.dialogue = dialogue;
            if (callback != null) callerScriptCallback = callback;
            if (currentActiveSpeaker != null && currentActiveSpeaker.IsSpeaking)
            {
                currentActiveSpeaker.StopSpeaking();
            }
            else
            {
                AdvanceToNextLine();
            }
            canStartDialogue = true;
        }
        public void UpdateDialogue(Dialogue dialogue)
        {
            this.dialogue = dialogue;
        }
        public void SpeakerFinished()
        {
            AdvanceToNextLine();
        }
        #endregion --------------------------------------------------------
    }
}