using System;
using UnityEngine;
using System.Collections;

namespace ElesJourney.Animations
{
    public class Slide : MonoBehaviour
    {

        [Serializable]
        struct SlideParams
        {
            public float startY;
            public float endY;
        }


        #region ----------------------- Serialize Fields -----------------------------

        [Header("Only Support Vertical Slide..")]
#pragma warning disable 649
        [SerializeField] private AnimationCurve translationCurve;
        [SerializeField] private float animationDuration;

        [Header("UI should be in pixel coordinates")]
        [SerializeField] private bool IsUI;
        [SerializeField] private SlideParams sliderParamsIn;
        [SerializeField] private SlideParams sliderParamsOut;
#pragma warning restore 649
        #endregion -----------------------------------------------------------------------
        private RectTransform myRectTransform;
        #region --------------------------- Private Methods --------------------------------
        private IEnumerator DoSlide(bool isSlideIn)
        {
            float timeQuant = 0;
            Vector2 startPos;

            if (IsUI)
            {
                myRectTransform = GetComponent<RectTransform>();
                startPos = myRectTransform.anchoredPosition;
                myRectTransform.anchoredPosition = startPos;
            }
            else
            {
                startPos = (Vector2)transform.position;
            }

            float startY = isSlideIn ? sliderParamsIn.startY : sliderParamsOut.startY;
            float endY = isSlideIn ? sliderParamsIn.endY : sliderParamsOut.endY;
            startPos.y = startY;
            transform.position = startPos;

            while (timeQuant < 1)
            {
                timeQuant += Time.deltaTime / animationDuration;
                Vector2 samplePos = new Vector2(startPos.x,
                                                  startY - ((startY - endY) * translationCurve.Evaluate(timeQuant))
                                                );
                if (IsUI)
                    myRectTransform.anchoredPosition = samplePos;
                else
                    transform.position = samplePos;

                yield return new WaitForEndOfFrame();
            }

            if (!IsUI)
                transform.position = new Vector2(transform.position.x, endY);
            else
                myRectTransform.anchoredPosition = new Vector2(myRectTransform.anchoredPosition.x, endY);
        }

        private void SnapToTarget(bool isSlideIn)
        {
            float endY = isSlideIn ? sliderParamsIn.endY : sliderParamsOut.endY;
            if (IsUI)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, endY);
            }
            else
                transform.position = new Vector3(transform.position.x, endY);
        }

        #endregion -------------------------------------------------------------------------


        public void DoSlideIn(bool instant)
        {
            if (instant)
            {
                SnapToTarget(true);
                return;
            }
            StartCoroutine(DoSlide(true));
        }

        public void DoSlideOut(bool instant)
        {
            if (instant)
            {
                SnapToTarget(false);
                return;
            }
            StartCoroutine(DoSlide(false));
        }
    }
}

