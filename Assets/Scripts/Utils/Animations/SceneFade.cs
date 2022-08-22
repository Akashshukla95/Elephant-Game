using System;
using UnityEngine;
using System.Collections;

namespace ElesJourney.Animations
{
    public class SceneFade : MonoBehaviour
    {
        #region -------------------- Serialize Fields -----------------------
#pragma warning disable 649
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float fadeOutDuration;
#pragma warning restore 649
        #endregion ----------------------------------------------------------

        #region --------------------- Private Fields --------------------------
        private CanvasGroup canvasGroup;
        private float desiredDuration;
        private float startAlpha;
        private float endAlpha;

        #endregion ------------------------------------------------------------

        #region --------------------- Private Methods --------------------------

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private IEnumerator DoFade(bool isFadeIn, Action funcRef)
        {

            if (isFadeIn)
            {
                startAlpha = 1;
                endAlpha = 0;
                desiredDuration = fadeInDuration;
            }
            else
            {
                startAlpha = 0f;
                endAlpha = 1;
                desiredDuration = fadeOutDuration;
            }


            float timeQuant = 0;

            while (timeQuant <= 1)
            {
                timeQuant += Time.deltaTime / desiredDuration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timeQuant);
                yield return new WaitForEndOfFrame();
            }
            canvasGroup.alpha = endAlpha;
            funcRef?.Invoke();
        }

        #endregion ------------------------------------------------------------


        public void FadeIn(Action funcRef)
        {
            StartCoroutine(DoFade(true, funcRef));
        }

        public void FadeOut(Action funcRef)
        {
            StartCoroutine(DoFade(false, funcRef));
        }
    }
}

