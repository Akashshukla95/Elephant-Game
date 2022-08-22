using UnityEngine;
using System.Collections;

namespace ElesJourney.Animations
{
    public class PulsatingEffect : MonoBehaviour
    {
        #region   --------------- Serialize Fields --------------- 
#pragma warning disable 649
        [SerializeField] private float pulsatingSpeed;
        [SerializeField] private float scaleFactor;
        [SerializeField] private bool allowAutoPulsate;
#pragma warning restore 649
        #endregion -----------------------------------------------


        #region   --------------- Private Fields --------------- 
        private Vector3 initialScale;

        #endregion -----------------------------------------------


        #region   --------------- Private Methods --------------- 
        private void Start()
        {
            initialScale = transform.localScale;
            if (allowAutoPulsate)
                StartCoroutine(Pulsate());
        }

        private IEnumerator Pulsate()
        {
            while (true)
            {
                float sampleVal = Mathf.Abs(Mathf.Sin(Time.time * pulsatingSpeed));
                transform.localScale = initialScale + new Vector3(sampleVal, sampleVal, sampleVal) * scaleFactor;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion -----------------------------------------------

        public void BeginPulsate()
        {
            StartCoroutine(Pulsate());
        }

        public void EndPulsate()
        {
            StopCoroutine(Pulsate());
        }
    }
}

