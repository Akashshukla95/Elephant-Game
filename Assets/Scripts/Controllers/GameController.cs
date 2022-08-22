using UnityEngine;
using System.Linq;
using ElesJourney.Views;
using ElesJourney.Models;
using System.Collections;
using ElesJourney.Managers;
using ElesJourney.Scriptables;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ElesJourney.Controllers
{
    public class GameController : MonoBehaviour
    {

        [System.Serializable]
        public class ExtraSprites
        {
            public List<Sprite> sprites;
        }

        #region --------------------- Serialize Fields --------------------------
#pragma warning disable 649
        //[SerializeField] private Image progressBar;
        [SerializeField] private List<ExtraSprites> extraSprites;
        [SerializeField] private GameView gameView;
        [SerializeField] private WordController wordController;
        [SerializeField] private MechanicController mechanicController;
        [SerializeField] private List<LevelsScriptable> levelsScriptables;
        [SerializeField] private UnlockableElementsScriptable unlockScenesData;

        [Space]
        [Header("Sound Files")]
        public AudioClip gameBackgroundClip;
        public AudioClip correctAnswerClip;
        public AudioClip wrongAnswerClip;
#pragma warning restore 649
        #endregion ---------------------------------------------------------------

        #region ----------------------- Private Fields ----------------------------
        private int movesTaken;
        private int movesAllowedForStars = 5;
        private bool hasUnlockedStar = false;
        private bool hasUnlockedItem = false;
        private List<Sprite> allSprites;
        private ItemData currentUnlockableItemData;

        #endregion ----------------------------------------------------------------

        #region ----------------------- Private Methods ----------------------------

        private IEnumerator Start()
        {
            GameManager.Instance.currentApplicationState = ApplicationState.GAMEPLAY_STATE;
            SoundManager.Instance.PlayAudio(gameBackgroundClip, 0.1f, true, false);
            currentUnlockableItemData = unlockScenesData.unlockData[GameManager.Instance.GetUnlockSceneIndex()].unlockableItems[GameManager.Instance.GetItemUnlockIndex()];
            gameView.SetItemToUnlockStatus(currentUnlockableItemData);
            movesTaken = 0;
            SetCurrentWord();
            mechanicController.ToggleMechanic(true);
            yield return null;
        }

        private void SetCurrentWord()
        {
            int levelIndex = GameManager.Instance.GetLevelIndex();
            int mechIndex = GameManager.Instance.GetMechanismIndex();
            int wordsDataIndex = GameManager.Instance.GetWordIndex();
            GameManager.Instance.SetCurrentMechanicType(
                levelsScriptables[levelIndex].mechanismScriptables[mechIndex].mechanismsData.mechanismType
            );
            Word word = levelsScriptables[levelIndex].mechanismScriptables[mechIndex].mechanismsData.words[wordsDataIndex];
            GameManager.Instance.SetCurrentWordData(word);
        }

        private void UpdateGameData()
        {
            if (movesTaken <= movesAllowedForStars)
            {
                hasUnlockedStar = true;
                GameManager.Instance.AddStars();
                int index = GameManager.Instance.GetItemUnlockIndex();
                int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
                if (GameManager.Instance.GetUserStars() == currentUnlockableItemData.totalPointsToUnlock)
                {
                    hasUnlockedItem = true;
                    GameManager.Instance.SetItemUnlockStatus(true);
                }
            }

            int levelIndex = GameManager.Instance.GetLevelIndex();
            int mechIndex = GameManager.Instance.GetMechanismIndex();
            int wordIndex = GameManager.Instance.GetWordIndex();

            if ((wordIndex + 1) >= levelsScriptables[levelIndex].mechanismScriptables[mechIndex].mechanismsData.words.Count)
            {
                wordIndex = 0;
                if (mechIndex + 1 >= levelsScriptables[levelIndex].mechanismScriptables.Count)
                {
                    Debug.LogFormat("{0} \n Message:{1}", "[GameController]", "Unlocked New Level");
                    mechIndex = 0;
                    levelIndex++;
                }
                else
                {
                    mechIndex++;
                    Debug.LogFormat("{0} \n Message:{1}", "[GameController]", "Increementing Mechanic Index");
                }

                
            }
            else
            {
                Debug.LogFormat("{0} \n Message:{1}", "[GameController]", "Increementing Mechanic Data Index");
                wordIndex++;
            }

            GameManager.Instance.SetCurrentLevelIndex(levelIndex);
            GameManager.Instance.SetCurrentMechanismIndex(mechIndex);
            GameManager.Instance.SetCurrentWordIndex(wordIndex);
        }

        #endregion -----------------------------------------------------------------


        #region ---------------------------- Public Methods --------------------------

        public void GenerateNewWord()
        {
            gameView.SetNewWordData();
            GameManager.Instance.allowUserInput = false;
            wordController.GenerateWord();
        }

        public void OnWordClosureComplete()
        {
            movesTaken = 0;
            hasUnlockedItem = false;
            hasUnlockedStar = false;
            SetCurrentWord();
            mechanicController.ToggleMechanic(true);
        }

        public void ProcessOutroStage()
        {
            mechanicController.ToggleMechanic(false);
        }

        public void RequestHintUI()
        {
            gameView.ShowHintUI();
        }

        public void ShowHint()
        {
            wordController.PlayHint();
        }

        public void OnWordComplete()
        {
            UpdateGameData();
            GameManager.Instance.allowUserInput = false;
            gameView.ShowSpeechBubble(Answer.CORRECT);
            mechanicController.ProcessMechanicOutro(true);
            if (GameManager.Instance.GetCurrentWordData().voiceOvers.Count > 0)
            {
                SoundManager.Instance.PlayAudioInSequence(
                                    wordController.GetVoiceOverClips(VoiceOverType.CORRECT),
                                    1,
                                    delegate
                                    {
                                        gameView.ShowWordComplete(Answer.CORRECT, hasUnlockedStar, hasUnlockedItem);
                                    }
                                );
            }
            else
            {
                gameView.ShowWordComplete(Answer.CORRECT, hasUnlockedStar, hasUnlockedItem);
            }
        }

        public void OnLetterInserted(Answer wordResult)
        {
            movesTaken++;
            if (wordResult == Answer.INCORRECT)
            {
                SoundManager.Instance.PlayAudio(wrongAnswerClip, 0.2f, false, true);
                gameView.ShowSpeechBubble(Answer.INCORRECT);
                gameView.ShowWordComplete(Answer.INCORRECT);
            }
            else
            {
                SoundManager.Instance.PlayAudio(correctAnswerClip, 0.5f, false, true);
            }
        }

        public List<Sprite> GetRandomWordSprites()
        {
            List<Sprite> spritesToReturn = new List<Sprite>();
            int[] indexArray = new int[4];
            switch (GameManager.Instance.GetLevelIndex())
            {
                case 0:
                    indexArray = new int[] { 1, 2, 3, 4 };
                    break;

                case 1:
                    indexArray = new int[] { 0, 2, 3, 4 };
                    break;

                case 2:
                    indexArray = new int[] { 0, 1, 3, 4 };
                    break;

                case 3:
                    indexArray = new int[] { 0, 1, 2, 3 };
                    break;
            }
            for (int i = 0; i < 9; i++)
            {
                List<Sprite> extraSp = extraSprites[indexArray[i]].sprites;
                spritesToReturn.Add(extraSp[Random.Range(0, extraSp.Count)]);
            }
            return spritesToReturn;
        }

        public void OnGameClose()
        {
            GameManager.Instance.currentApplicationState = ApplicationState.NARRATION_STATE;
            GameManager.Instance.allowUserInput = false;
            gameView.ToggleGameScene(false);
        }


        #endregion -----------------------------------------------------------------------
    }
}
