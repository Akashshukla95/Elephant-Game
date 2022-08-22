using UnityEngine;
using System.Collections;


namespace ElesJourney.Animations
{
    public class Oscillation : MonoBehaviour
    {
        #region   --------------- Serialize Fields --------------- 
#pragma warning disable 649
        [SerializeField] private bool allowAutoOscillation;
#pragma warning restore 649
        #endregion -----------------------------------------------


        #region   --------------- Public Fields ------------------
        public float oscillationSpeed;
        public float scaleFactor;

        #endregion ----------------------------------------------- 


        #region   --------------- Private Methods ----------------

        private void Start()
        {
            if (allowAutoOscillation)
                StartCoroutine(Oscillate());
        }

        private IEnumerator Oscillate()
        {
            while (true)
            {
                transform.eulerAngles = new Vector3(
                                                    transform.eulerAngles.x,
                                                    transform.eulerAngles.y,
                                                    scaleFactor * Mathf.Sin(Time.time * oscillationSpeed)
                                                    );

                yield return new WaitForEndOfFrame();
            }

        }

        #endregion -----------------------------------------------

        public void BeginOscillation()
        {
            StartCoroutine(Oscillate());
        }

        public void EndOscillation()
        {
            StopCoroutine(Oscillate());
        }

    }
}

