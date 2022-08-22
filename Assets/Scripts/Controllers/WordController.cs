using System;
using UnityEngine;
using ElesJourney.Views;
using ElesJourney.Models;
using System.Collections;
using ElesJourney.Managers;
using System.Collections.Generic;

namespace ElesJourney.Controllers
{
    public class WordController : MonoBehaviour
    {
        #region ---------------------- Serialize Fields -----------------------
#pragma warning disable 649
        // [SerializeField] private Bridge bridge;
        [SerializeField] private bool allowBridgeCommunication;
        [SerializeField] private WordView wordView;
        [SerializeField] private GameController gameController;
        [SerializeField] private AudioClip unguidedHintAudioClip;
#pragma warning restore 649
        #endregion -------------------------------------------------------------

        #region -------------------------- Private Fields -----------------------
        private int requiredLetterIndex;
        private string word;
        private float incorrectAnimDelay = 0.2f;
        private List<int> letterIndexes;
        private string previousArWord = "";
        private float currentTime;
        private float bridgeInterval = 1;
        private bool inBridgeState = false;
        private float maxTimeAllowed = 40;
        private float timer;

        #endregion ---------------------------------------------------------------

        #region ------------------------ Private Methods -----------------------

        private void Start()
        {
            //if (allowBridgeCommunication) bridge.Init();
        }

        private void Update()
        {
            if (GameManager.Instance.currentApplicationState is ApplicationState.GAMEPLAY_STATE)
            {
                #region Gameplay Code
                if (GameManager.Instance.allowUserInput)
                {
                    timer += Time.deltaTime;
                    if (timer >= maxTimeAllowed)
                    {
                        timer = 0;
                        Word currentWordData = GameManager.Instance.GetCurrentWordData();
                        if (currentWordData.isGuided && currentWordData.voiceOvers.Count > 0)
                        {
                            SoundManager.Instance.PlayAudioInSequence(GetVoiceOverClips(VoiceOverType.HINT), 1);
                        }
                        else
                        {
                            SoundManager.Instance.PlayAudioInSequence(new List<AudioClip>() { unguidedHintAudioClip }, 1);
                        }
                    }
                }
                #endregion

                #region Ar Plugin Code
                if (allowBridgeCommunication && inBridgeState)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= bridgeInterval)
                    {
                        // currentTime = 0;
                        // Tuple<string, bool> wordData = bridge.GetWordData();
                        // if (wordData.Item2)
                        // {
                        //     ProcessWords(wordData.Item1);
                        // }
                    }
                }
                #endregion
            }
        }

        private void IndicateLetter()
        {
            timer = 0;
            GameManager.Instance.allowUserInput = true;
            requiredLetterIndex = letterIndexes[0];
            letterIndexes.RemoveAt(0);
            wordView.IndicateLetter(requiredLetterIndex);
        }

        private IEnumerator HandleIncorrectStateAnim()
        {
            yield return new WaitForSeconds(incorrectAnimDelay);
            gameController.OnLetterInserted(Answer.INCORRECT);
            yield return new WaitForSeconds(1);
            Word currentWord = GameManager.Instance.GetCurrentWordData();
            if (currentWord.isGuided && currentWord.voiceOvers.Count > 0)
                PlayHint();

            //inBridgeState = true;
        }

        private void InitWord()
        {
            StartCoroutine(wordView.InitWords(
                word,
                letterIndexes,
                delegate
                {
                    inBridgeState = true;
                    IndicateLetter();
                }
            )
            );
        }

        #endregion ---------------------------------------------------------------

        #region ---------------------------- Public Methods --------------------------

        public void GenerateWord()
        {
            previousArWord = "";
            Word wordData = GameManager.Instance.GetCurrentWordData();
            word = wordData.word;
            List<int> tempLetters = new List<int>(wordData.wordIndexes);

            if (tempLetters.Count == 0)
            {
                letterIndexes = new List<int>();
                for (int i = 0; i < word.Length; i++)
                {
                    letterIndexes.Add(i);
                }
            }
            else
            {
                letterIndexes = new List<int>(tempLetters);
            }
            tempLetters.Clear();

            if (wordData.isGuided && wordData.voiceOvers.Count > 0)
            {
                SoundManager.Instance.PlayAudioInSequence(GetVoiceOverClips(VoiceOverType.INTRO),
                1,
                delegate
                {
                    SoundManager.Instance.PlayAudioInSequence(GetVoiceOverClips(VoiceOverType.GUIDE), 0.5f);
                    if (GameManager.Instance.GetCurrentWordData().autoHint)
                    {
                        gameController.RequestHintUI();
                    }
                    InitWord();
                });
            }
            else
                InitWord();
        }

        private void ProcessAnswer(Answer answer)
        {
            timer = 0;
            if (answer is Answer.CORRECT)
            {
                wordView.OnCorrectLetter(requiredLetterIndex);
                gameController.OnLetterInserted(Answer.CORRECT);
                if (letterIndexes.Count == 0)
                {
                    Debug.LogFormat("{0} \n Message: {1}", "[WordController]", "You Won");
                    wordView.CloseWordHolders();
                    gameController.OnWordComplete();
                }
                else
                {
                    IndicateLetter();
                    wordView.IndicateLetter(requiredLetterIndex);
                }
            }
            else
            {
                wordView.ShowIncorrect(requiredLetterIndex);
                StartCoroutine(HandleIncorrectStateAnim());
            }
        }

        public void DoAutoCorrect()
        {
            letterIndexes.Clear();
            letterIndexes = new List<int>();
            ProcessAnswer(Answer.CORRECT);
        }

        public void OnLetterEntered(int letterAsciVal)
        {
            if (GameManager.Instance.currentApplicationState is ApplicationState.GAMEPLAY_STATE &&
                GameManager.Instance.allowUserInput)
            {
                Answer answer = word[requiredLetterIndex] == (char)letterAsciVal ? Answer.CORRECT : Answer.INCORRECT;
                ProcessAnswer(answer);
            }
        }

        public void PlayHint()
        {
            SoundManager.Instance.PlayAudioInSequence(GetVoiceOverClips(VoiceOverType.HINT), 0);
        }

        public void ProcessWords(string detectedWord)
        {
            if (GameManager.Instance.allowUserInput)
            {
                if ((detectedWord.Length == requiredLetterIndex + 1) && (previousArWord != detectedWord))
                {
                    previousArWord = detectedWord;
                    Answer answer = Answer.CORRECT;
                    for (int i = 0; i <= requiredLetterIndex; i++)
                    {
                        if (detectedWord[i] != word[i])
                        {
                            answer = Answer.INCORRECT;
                            break;
                        }
                    }
                    inBridgeState = false;
                    ProcessAnswer(answer);
                }
            }
        }

        public List<AudioClip> GetVoiceOverClips(VoiceOverType voiceOverType)
        {
            Word wordData = GameManager.Instance.GetCurrentWordData();
            for (int i = 0; i < wordData.voiceOvers.Count; i++)
            {
                if (voiceOverType == wordData.voiceOvers[i].voiceOverType)
                {
                    return wordData.voiceOvers[i].clips;
                }
            }
            return null;
        }

        #endregion ---------------------------------------------------------------
    }
}
