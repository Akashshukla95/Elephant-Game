using UnityEngine;
using System.Collections;

namespace ElesJourney.Animations
{
    public class WaveEffect : MonoBehaviour
    {
        #region   --------------- Serialize Fields --------------- 
#pragma warning disable 649
        [SerializeField] private float waveHeight;
        [SerializeField] private float waveFrequency;
        [SerializeField] private bool allowAutoWave;
#pragma warning restore 649
        #endregion -----------------------------------------------


        #region   --------------- Private Fields --------------- 
        private Vector3 defaultPos;

        #endregion -----------------------------------------------


        #region   --------------- Private Methods ---------------
        private void Start()
        {
            if (allowAutoWave)
                BeginWave();
        }

        private IEnumerator DoWave()
        {
            while (true)
            {
                float ySample = waveHeight * Mathf.Sin(waveFrequency * Time.time);
                transform.localPosition = new Vector3(defaultPos.x, defaultPos.y + ySample, defaultPos.z);
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion -----------------------------------------------

        public void BeginWave()
        {
            defaultPos = transform.localPosition;
            StartCoroutine(DoWave());
        }

        public void EndWave()
        {
            StopCoroutine(DoWave());
        }

    }
}

