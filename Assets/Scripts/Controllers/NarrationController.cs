using UnityEngine;
using ElesJourney.Views;
using ElesJourney.Models;
using ElesJourney.Managers;
using K12.CommonDialogueSystem;
using System.Collections;

namespace ElesJourney.Controllers
{
    public class NarrationController : MonoBehaviour
    {

        #region --------------------- Serialize Fields -----------------------
#pragma warning disable 649
        [SerializeField] private NarrationView narrationView;
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private WorldController worldController;
        [SerializeField] private Dialogue introDialogue;
        [SerializeField] private AudioClip dialogBoxClip;
        [SerializeField] private AudioClip narrationBackgroundClip;
        #endregion
#pragma warning restore 649

        #region ---------------------- Private Methods -------------------------

        private void Start()
        {
            SoundManager.Instance.PlayAudio(narrationBackgroundClip, 0.5f, true, false);
        }

        private IEnumerator BeginNarration()
        {
            yield return new WaitForSeconds(0.5f);
            narrationView.EnableNarration();
            SoundManager.Instance.PlayAudio(dialogBoxClip, 1, false, true);
            dialogueController.StartDialogue(introDialogue, delegate { EndNarration(); });
        }

        private void EndNarration()
        {
            SoundManager.Instance.PlayAudio(dialogBoxClip, 1, false, true);
            narrationView.DisableNarration();
            worldController.SetWorldItems();
        }
        #endregion -------------------------------------------------------------

        #region ---------------------- Public Methods -------------------------

        public void InitNarration()
        {
            GameManager.Instance.currentApplicationState = ApplicationState.NARRATION_STATE;
            if (GameManager.Instance.IsNewUser())
            {
                narrationView.ShowUserRegistrationPanel();
            }
            else
            {
                int sampleCount = GameManager.Instance.GetLevelIndex() +
                                  GameManager.Instance.GetMechanismIndex() +
                                  GameManager.Instance.GetWordIndex();
                if (sampleCount == 0)
                    StartCoroutine(BeginNarration());

                worldController.SetWorldItems();
            }
        }

        public IEnumerator PlayDialogue(Dialogue dialogue)
        {
            narrationView.EnableNarration();
            bool dialogComplete = false;
            SoundManager.Instance.PlayAudio(dialogBoxClip, 1, false, true);
            dialogueController.StartDialogue(dialogue,
            delegate
            {
                dialogComplete = true;
                SoundManager.Instance.PlayAudio(dialogBoxClip, 1, false, true);
            });
            while (!dialogComplete)
            {
                yield return new WaitForEndOfFrame();
            }
            narrationView.DisableNarration();
            yield return 0;
        }

        public void OnRegistrationComplete(int age)
        {
            int levelIndex = 0;
            switch (age)
            {
                case 2:
                    levelIndex = 0;
                    break;

                case 3:
                    levelIndex = 1;
                    break;

                case 4:
                case 5:
                case 6:
                    levelIndex = 2;
                    break;
            }
            GameManager.Instance.SetIsNewUser(false);
            GameManager.Instance.SetCurrentLevelIndex(levelIndex);
            worldController.SetWorldItems();
            StartCoroutine(BeginNarration());
        }

        #endregion -------------------------------------------------------------
    }
}
