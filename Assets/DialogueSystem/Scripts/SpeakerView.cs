using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace K12.CommonDialogueSystem
{
    public class SpeakerView : MonoBehaviour
    {
        #region --------------------------------------- Private variables ---------------------------------------
        #region SerializeField 
        [SerializeField] private Image speakerPortrait = null;
        [SerializeField] private Text dialogueText = null;
        [SerializeField] private Text nameText = null;
        #endregion
        #region Non-SerializeField
        private Character speaker;
        private string dialogueString = "";
        private bool isSpeaking = false;
        private DialogueSettings dialogueSettings = null;
        private DialogueController dialogueController = null;
        private CanvasGroup canvasGroup = null;
        #endregion
        #endregion ---------------------------------------
        #region --------------------------------------- Public variables ---------------------------------------
        public Character Speaker
        {
            get { return speaker; }
            set
            {
                speaker = value;
                speakerPortrait.sprite = speaker.characterSprite;
                nameText.text = speaker.fullName;
            }
        }
        public string Dialogue
        {
            set { dialogueString = value; }
        }
        public bool IsSpeaking
        {
            get { return isSpeaking; }
        }
        #endregion ---------------------------------------
        #region ---------------------------- Private Methods ----------------------------
        #region -------------- Monobehaviour --------------
        private void Awake()
        {
            dialogueSettings = gameObject.GetComponentInParent<DialogueSettings>();
            dialogueController = gameObject.GetComponentInParent<DialogueController>();
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
        #endregion ----------------------------
        #region -------------- Non-Monobehaviour --------------
        private IEnumerator TypeSentence(string sentence)
        {
            isSpeaking = true;
            dialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(dialogueSettings.dialogueTextDelay);
            }
            isSpeaking = false;
            yield return new WaitForSeconds(1);
            if (dialogueSettings.autoPlay) dialogueController.SpeakerFinished();
        }
        IEnumerator FadeOverTime(float startAlpha, float endAlpha, float duration)
        {
            float currentAlpha = startAlpha;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float normalizedTime = t / duration;
                currentAlpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
                canvasGroup.alpha = currentAlpha;
                yield return null;
            }
            currentAlpha = endAlpha;
            canvasGroup.alpha = currentAlpha;
        }

        #endregion ----------------------------
        #endregion --------------------------------------------------------
        #region ---------------------------- Public Methods ----------------------------
        public void ShowSpeaker(bool status)
        {
            bool wasActive = gameObject.activeSelf;
            gameObject.SetActive(status);
            StopAllCoroutines();
            if (status)
            {
                if (!wasActive && dialogueSettings.canFadeSpeakerIn) StartCoroutine(FadeOverTime(0f, 1f, dialogueSettings.fadeSpeakerDuration));
                StartCoroutine(TypeSentence(dialogueString));
            }
        }
        public bool IsCharacter(Character character)
        {
            return speaker == character;
        }
        public void StopSpeaking()
        {
            isSpeaking = false;
            StopAllCoroutines();
            canvasGroup.alpha = 1f;
            dialogueText.text = dialogueString;
        }
        public void SetFont(Font dialogFont)
        {
            dialogueText.font = dialogFont;
            nameText.font = dialogFont;
        }
        #endregion --------------------------------------------------------
    }
}