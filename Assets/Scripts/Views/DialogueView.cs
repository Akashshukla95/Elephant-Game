using TMPro;
using UnityEngine;
using ElesJourney.Animations;

namespace ElesJourney.Views
{
    public class DialogueView : MonoBehaviour
    {

        #region --------------------- Serialize Fields -----------------------
#pragma warning disable 649
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
#pragma warning restore 649
        #endregion -------------------------------------------------------------

        #region ---------------------- Public Methods ---------------------------
        public void UpdateNameText(string nameString)
        {
            nameText.text = nameString;
        }

        public void UpdateDialogueText(string dialogueString)
        {
            dialogueText.text = dialogueString;
        }

        public void ShowDialogueUI()
        {
            dialoguePanel.GetComponent<Slide>().DoSlideIn(false);
        }

        public void HideDialogueUI()
        {
            dialoguePanel.GetComponent<Slide>().DoSlideOut(false);
        }

        #endregion --------------------------------------------------------------
    }
}
