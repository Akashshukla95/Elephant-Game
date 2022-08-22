using TMPro;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using ElesJourney.Models;
using System.Collections;
using ElesJourney.Managers;
using ElesJourney.Components;
using ElesJourney.Animations;
using ElesJourney.Scriptables;
using ElesJourney.Controllers;
using ElesJourney.GlobalStrings;
using System.Collections.Generic;

namespace ElesJourney.Views
{
    public class GameView : MonoBehaviour
    {
        #region ------------------------ Serialize Fields -----------------------
#pragma warning disable 649
        [SerializeField] private Canvas canvas;
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private GameObject eleObject;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject dummyInputPanel;
        [SerializeField] private TextMeshProUGUI eleDialogueText;
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private GameObject starsParentPanel;
        [Header("Unlockable Item")]
        [SerializeField] private GameObject bigStar;
        [SerializeField] private TextMeshProUGUI starStatusText;
        [SerializeField] private GameObject itemProgressPanel;
        [SerializeField] private Image itemUnlockOutlineImage;
        [SerializeField] private Image itemUnlockProgressImage;
        [SerializeField] private GameObject itemRevealPanel;
        [SerializeField] private GameObject keypadObject;
        [SerializeField] private Image itemRevealImage;
        [SerializeField] private TextMeshProUGUI itemRevealLabel;
        [SerializeField] private GameObject unlockStarObject;
        [SerializeField] private ParticleSystem winParticleSystem;
        [SerializeField] private ParticleSystem collectStarParticle;

        [Header("Debug")]
        [SerializeField] private Text debugLevelText;
        [SerializeField] private Text debugWordText;
        [SerializeField] private Text debugMechanismText;

        [Header("Hint System")]
        [SerializeField] private GameObject hintButtonObject;
        [SerializeField] private GameObject handCursorObject;
        [SerializeField] private GameController gameController;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip characterSlideClip;
        [SerializeField] private AudioClip itemRevealClip;
#pragma warning restore 649
        #endregion ---------------------------------------------------------------------


        #region ------------------------ Private Fields ---------------------------------
        private float eleDialogueDelay = 4;
        private float incorrectWordDelay = 4;
        private float wordCompleteAnimDelay = 3;
        private ItemData currentItemData;

        #endregion ----------------------------------------------------------------------

        #region ------------------------ Private Methods -----------------------------


        private IEnumerator ShowStarsAnimation()
        {
            overlayCanvas.GetComponent<CanvasGroup>().alpha = 1;
            unlockStarObject.SetActive(true);
            unlockStarObject.GetComponent<UnlockStarComponent>().UnlockStar(
                bigStar.transform.position,
                delegate
                {
                    collectStarParticle.gameObject.transform.position = bigStar.transform.position;
                    bigStar.GetComponent<Animator>().Play(GameStrings.collectAnimation);
                    collectStarParticle.Play();
                    unlockStarObject.SetActive(false);
                    SetItemToUnlockStatus(currentItemData);
                }
            );
            yield return new WaitForSeconds(wordCompleteAnimDelay);
        }

        private IEnumerator ShowItemUnlockAnimation()
        {
            GameManager.Instance.ResetStars();
            starsParentPanel.SetActive(false);
            Animator anim = itemRevealPanel.GetComponent<Animator>();
            SoundManager.Instance.PlayAudio(itemRevealClip, 0.5f, false, true);
            itemRevealPanel.SetActive(true);
            itemRevealImage.sprite = currentItemData.itemSprite;
            itemRevealLabel.text = GameStrings.itemUnlocked + currentItemData.itemName;
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            itemRevealPanel.SetActive(false);
            ToggleGameScene(false);
        }

        // MOVE THIS CODE TO GAME CONTROLLER
        private IEnumerator ShowEleDialogue(bool hasUnlockedStar, bool hasUnlockedItem)
        {
            if (hasUnlockedStar)
                yield return StartCoroutine(ShowStarsAnimation());

            overlayCanvas.GetComponent<CanvasGroup>().alpha = 0;
            gameController.ProcessOutroStage();
            speechBubble.GetComponent<SpeechBubbleComponent>().ToggleSpeechBubble(false);

            if (hasUnlockedStar)
            {
                yield return new WaitForSeconds(0.2f);
                itemUnlockProgressImage.sprite = currentItemData.itemSprite;
                itemUnlockOutlineImage.sprite = currentItemData.itemSprite;
                itemProgressPanel.SetActive(true);
                int starsEarned = GameManager.Instance.GetUserStars();
                float res = (float)starsEarned - 1;
                float startFill = starsEarned == 0 ? 0 : (float)res / currentItemData.totalPointsToUnlock;
                itemUnlockProgressImage.fillAmount = startFill;
                overlayCanvas.GetComponent<SceneFade>().FadeOut(null);
                yield return new WaitForSeconds(0.6f);
                float timeQuant = 0;
                float progressFillTime = 2;
                itemUnlockOutlineImage.GetComponent<PulsatingEffect>().BeginPulsate();

                float endFill = (float)starsEarned / currentItemData.totalPointsToUnlock;
                float currentFill = 0;
                while (timeQuant < 1)
                {
                    currentFill = Mathf.Lerp(startFill, endFill, timeQuant);
                    itemUnlockProgressImage.fillAmount = currentFill;
                    timeQuant += Time.deltaTime / progressFillTime;
                    yield return new WaitForEndOfFrame();
                }
                itemUnlockProgressImage.fillAmount = endFill;
                itemUnlockOutlineImage.GetComponent<PulsatingEffect>().EndPulsate();
                if (hasUnlockedItem)
                {
                    itemProgressPanel.SetActive(false);
                    yield return StartCoroutine(ShowItemUnlockAnimation());
                }
                overlayCanvas.GetComponent<SceneFade>().FadeIn(null);
                yield return new WaitForSeconds(0.6f);
                itemProgressPanel.SetActive(false);

            }
            yield return new WaitForSeconds(0.5f);
            gameController.OnWordClosureComplete();
            // if (GameManager.Instance.currentApplicationState is ApplicationState.GAMEPLAY_STATE)
            // {
            //     // dialoguePanel.SetActive(true);
            //     // dialoguePanel.GetComponent<Slide>().DoSlideIn(false);
            //     // SoundManager.Instance.PlayAudio(characterSlideClip, 0.5f, false, true);
            //     // eleObject.GetComponent<Slide>().DoSlideIn(false);
            //     // string[] wordCompleteStrings = GameStrings.wordCompletedStrings;
            //     // string dialogueString = wordCompleteStrings[Random.Range(0, wordCompleteStrings.Length)];
            //     // float letterAnimationDelay = 1 / dialogueString.Length;
            //     // char[] dialogueArray = dialogueString.ToCharArray();
            //     // StringBuilder currentDialogueWorld = new StringBuilder();
            //     // for (int i = 0; i < dialogueArray.Length; i++)
            //     // {
            //     //     currentDialogueWorld.Append(dialogueArray[i]);
            //     //     eleDialogueText.text = currentDialogueWorld.ToString();
            //     //     yield return new WaitForSeconds(letterAnimationDelay);
            //     // }

            //     // yield return new WaitForSeconds(eleDialogueDelay - 1);
            //     // SoundManager.Instance.PlayAudio(characterSlideClip, 0.5f, false, true);
            //     // dialoguePanel.GetComponent<Slide>().DoSlideOut(false);
            //     // eleObject.GetComponent<Slide>().DoSlideOut(false);
            //     // dialoguePanel.SetActive(false);

            // }

        }


        private IEnumerator ShowIncorrectWordState()
        {
            yield return new WaitForSeconds(incorrectWordDelay);
            speechBubble.GetComponent<SpeechBubbleComponent>().ToggleSpeechBubble(false);
        }

        #endregion ---------------------------------------------------------------------


        #region ------------------------ Public Methods -----------------------------

        public void SetNewWordData()
        {
            // debugLevelText.text = "Level : " + (GameManager.Instance.GetLevelIndex() + 1);
            // debugMechanismText.text = "Mechanism : " + GameManager.Instance.GetCurrentMechanicType();
            // debugWordText.text = "Word : " + GameManager.Instance.GetCurrentWordData().word;

            keypadObject.SetActive(true);
            if (GameManager.Instance.GetLevelIndex() == 0)
            {
                if (GameManager.Instance.GetMechanismIndex() == 0 && GameManager.Instance.GetWordIndex() <= 2)
                {
                    hintButtonObject.SetActive(false);
                }
                else
                {
                    hintButtonObject.SetActive(true);
                }
            }
            else
            {
                hintButtonObject.SetActive(true);
            }
        }
        public void ToggleDummyInput()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            bool status = dummyInputPanel.activeInHierarchy;
            dummyInputPanel.SetActive(!status);
        }

        public void SetItemToUnlockStatus(ItemData itemData)
        {
            currentItemData = itemData;
            int starsEarned = GameManager.Instance.GetUserStars();
            starStatusText.text = $"{starsEarned}/{currentItemData.totalPointsToUnlock}";
        }

        public void ToggleGameScene(bool status)
        {
            canvas.GetComponent<SceneFade>().FadeIn(
                delegate
                {
                    GameManager.Instance.LoadScene(GameStrings.narrationScene);
                }
            );
        }

        public void ShowWordComplete(Answer wordResult, bool hasUnlockedStar = false, bool hasUnlockedItem = false)
        {
            dummyInputPanel.SetActive(false);
            if (wordResult is Answer.CORRECT)
            {
                keypadObject.SetActive(false);
                hintButtonObject.SetActive(false);
                winParticleSystem.Play();
                StopCoroutine("ShowEleDialogue");
                StartCoroutine(ShowEleDialogue(hasUnlockedStar, hasUnlockedItem));
            }
            else
            {
                StopCoroutine("ShowIncorrectWordState");
                StartCoroutine(ShowIncorrectWordState());
            }
        }

        public void ShowSpeechBubble(Answer wordResult)
        {
            dummyInputPanel.SetActive(false);
            speechBubble.SetActive(true);
            speechBubble.GetComponent<SpeechBubbleComponent>().ToggleSpeechBubble(true, wordResult);
        }

        public void ShowHintUI()
        {
            hintButtonObject.SetActive(true);
            handCursorObject.SetActive(true);
            handCursorObject.GetComponent<WaveEffect>().BeginWave();
        }

        public void OnHintButtonClick()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            if (GameManager.Instance.allowUserInput)
            {
                if (GameManager.Instance.GetCurrentWordData().autoHint)
                {
                    handCursorObject.SetActive(false);
                    handCursorObject.GetComponent<WaveEffect>().EndWave();
                }
                gameController.ShowHint();
            }
        }

        public void OnCloseClick()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            gameController.OnGameClose();
        }
        #endregion ---------------------------------------------------------------------
    }
}
