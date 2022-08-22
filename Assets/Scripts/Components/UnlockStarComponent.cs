using System;
using UnityEngine;
using System.Collections;
using ElesJourney.Managers;

namespace ElesJourney.Components
{
    public class UnlockStarComponent : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private float introScale;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float introDuration;
        [SerializeField] private float stayDuration;
        [SerializeField] private float travelAnimDuration;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private AudioClip starUnlockClip;
#pragma warning restore 649
        private bool allowAnim = false;

        private void Update()
        {
            if (allowAnim)
            {
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }
        }

        private IEnumerator PlayUnlockAnimation(Vector2 travelPos, Action OnAnimComplete)
        {
            transform.position = Vector2.zero;
            float timeQuant = 0;
            while (timeQuant < 1)
            {
                transform.localScale = Vector2.Lerp(Vector2.zero, Vector3.one * introScale, timeQuant);
                yield return new WaitForEndOfFrame();
                timeQuant += Time.deltaTime / introDuration;
            }
            transform.localScale = Vector2.one * introScale;
            yield return new WaitForSeconds(stayDuration);
            timeQuant = 0;

            Vector2 startPoint = transform.position;
            Vector2 endPoint = travelPos;
            Vector2 direction = endPoint - startPoint;

            Vector2 midPoint = startPoint + ((direction.normalized * direction.magnitude / 2) + new Vector2(1, -1) * 1.2f);

            while (timeQuant < 1)
            {
                Vector2 pointA = Vector2.Lerp(startPoint, midPoint, timeQuant);
                Vector2 pointB = Vector2.Lerp(midPoint, endPoint, timeQuant);
                transform.position = Vector2.Lerp(pointA, pointB, animationCurve.Evaluate(timeQuant));
                transform.localScale = Vector2.Lerp(Vector2.one * introScale, Vector2.one, timeQuant);
                yield return new WaitForEndOfFrame();
                timeQuant += Time.deltaTime / travelAnimDuration;
            }
            allowAnim = false;
            OnAnimComplete.Invoke();
        }

        public void UnlockStar(Vector2 travelPosition, Action OnAnimComplete)
        {
            allowAnim = true;
            SoundManager.Instance.PlayAudio(starUnlockClip, 0.5f, false, true);
            StartCoroutine(PlayUnlockAnimation(travelPosition, OnAnimComplete));
        }
    }
}
