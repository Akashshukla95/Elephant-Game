using TMPro;
using UnityEngine;
using ElesJourney.Models;
using ElesJourney.Managers;
using ElesJourney.Animations;
using ElesJourney.GlobalStrings;
using System.Collections.Generic;

namespace ElesJourney.Components
{

    public class SpeechBubbleComponent : MonoBehaviour
    {

        [System.Serializable]
        public struct SpeechText
        {
            public Color textColor;
            public string word;
            public AudioClip audioClip;
        }

        #region -------------------- Serialize Fields -------------------------
#pragma warning disable 649
        [SerializeField] private List<SpeechText> winTextList;
        [SerializeField] private List<SpeechText> loseTextList;
        [SerializeField] private TextMeshPro speechBubbleText;
#pragma warning restore 649
        #endregion-------------------------------------------------------------

        #region ---------------------- Private Fields ---------------------------
        private Animator animator;

        #endregion --------------------------------------------------------------

        #region ----------------------- Private Methods ---------------------------

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        #endregion ------------------------------------------------------------------

        #region ------------------------ Public Methods ------------------------------

        public void ToggleSpeechBubble(bool show, Answer wordResult = Answer.INCORRECT)
        {
            if (show)
            {
                if (wordResult is Answer.CORRECT)
                {
                    GetComponent<Oscillation>().BeginOscillation();
                    int index = Random.Range(0, winTextList.Count);
                    speechBubbleText.text = winTextList[index].word;
                    speechBubbleText.color = winTextList[index].textColor;
                    SoundManager.Instance.PlayAudio(winTextList[index].audioClip, 1, false, true);
                }
                else
                {
                    GetComponent<WaveEffect>().BeginWave();
                    int index = Random.Range(0, loseTextList.Count);
                    speechBubbleText.text = loseTextList[index].word;
                    speechBubbleText.color = loseTextList[index].textColor;
                    SoundManager.Instance.PlayAudio(loseTextList[index].audioClip, 1, false, true);
                }

                animator.Play(GameStrings.scaleInAnimation);
            }
            else
            {
                GetComponent<WaveEffect>().EndWave();
                GetComponent<Oscillation>().EndOscillation();
                animator.Play(GameStrings.scaleOutAnimation);
            }
        }

        #endregion --------------------------------------------------------------------------
    }
}
