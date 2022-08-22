using System;
using UnityEngine;
using System.Collections;
using ElesJourney.GlobalStrings;
using ElesJourney.Managers;

namespace ElesJourney.Components
{
    public class ItemComponent : MonoBehaviour
    {
        #region --------------------- Serialize Fields -------------------------
#pragma warning disable 649
        [SerializeField] private AudioClip itemAudioClip;
        [SerializeField] private float clipPlayDuration;
#pragma warning restore 649
        #endregion --------------------------------------------------------------

        #region ---------------------- Private Fields ----------------------------
        private Action OnUnlockComplete = null;

        #endregion -----------------------------------------------------------------

        private IEnumerator ShowUnlockAnimations(AudioClip itemUnlockClip)
        {
            yield return new WaitForSeconds(1);
            SoundManager.Instance.PlayAudio(itemUnlockClip, 0.5f, false, true);
            // if (itemAudioClip != null)
            // SoundManager.Instance.PlayUnlockItemAudio(itemAudioClip);
            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().Play(GameStrings.itemUnlockedAnimation);
            yield return new WaitForSeconds(clipPlayDuration);
            SoundManager.Instance.StopUnlockItemAudio();
            OnUnlockComplete.Invoke();
        }

        #region ------------------------- Public Methods ----------------------------
        public void UnlockItem(Action OnUnlockComplete, AudioClip itemUnlockClip)
        {
            this.OnUnlockComplete = OnUnlockComplete;
            StartCoroutine(ShowUnlockAnimations(itemUnlockClip));
        }
        #endregion -----------------------------------------------------------------------
    }
}
