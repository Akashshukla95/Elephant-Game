using System;
using UnityEngine;
using System.Collections;
using ElesJourney.Managers;
using ElesJourney.GlobalStrings;
using System.Collections.Generic;

namespace ElesJourney.Components
{
    public class SpinWheelComponent : MonoBehaviour
    {
        #region ------------------------ Serialize Fields ----------------------
#pragma warning disable 649
        [SerializeField] GameObject wheel;
        [SerializeField] int totalRotations;
        [SerializeField] private float animDuration;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private List<SpriteRenderer> itemRendererList;
        [SerializeField] private AudioClip spinAudioClip;
#pragma warning restore 649
        #endregion --------------------------------------------------------------

        #region ----------------------- Private Fields ----------------------------
        private float angleToRotate;
        private int index;
        private float rotationDelay = 2;

        #endregion ----------------------------------------------------------------

        private IEnumerator SpinWheel(Action OnRotationComplete)
        {
            yield return new WaitForSeconds(rotationDelay);
            SoundManager.Instance.PlayAudio(spinAudioClip, 0.5f, false, true);
            float timeQuant = 0;
            while (timeQuant < 1)
            {
                Vector3 newAngles = new Vector3(0, 0, angleToRotate * animationCurve.Evaluate(timeQuant));
                wheel.transform.localEulerAngles = newAngles;
                timeQuant += Time.deltaTime / animDuration;
                yield return new WaitForEndOfFrame();
            }
            wheel.transform.eulerAngles = new Vector3(0, 0, angleToRotate);
            OnRotationComplete.Invoke();
        }

        public void InitSpinWheel(List<Sprite> wordSprites, Action OnRotationComplete)
        {
            for (int i = 0; i < 5; i++)
            {
                itemRendererList[i].sprite = wordSprites[i];
            }
            wheel.transform.eulerAngles = Vector3.zero;
            GetComponent<Animator>().Play(GameStrings.spinWheelIntro);
            angleToRotate = 360 * totalRotations + ((0 * 90) + 45);
            StartCoroutine(SpinWheel(OnRotationComplete));
        }

        public void ExitSpinWheel()
        {
            GetComponent<Animator>().Play(GameStrings.spinWheelExit);
        }
    }
}