using System;
using UnityEngine;
using K12Singleton;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace ElesJourney.Managers
{
    public class SoundManager : Singleton<SoundManager>
    {
        #region ----------------------- Serialize Fields -----------------------------

        #endregion --------------------------------------------------------------------

        #region ----------------------- Private Fields --------------------------------
        private int maxPoolSize = 5;
        private List<AudioSource> sourcePools;
        private AudioSource uiAudioSource;
        private AudioSource mainAudioSource;
        private AudioSource sequenceAudioSource;
        private AudioSource unlockItemAudioSource;

        #endregion ---------------------------------------------------------------------

        #region ------------------------ Private Methods -------------------------------


        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (sourcePools != null)
            {
                foreach (AudioSource source in sourcePools)
                {
                    Destroy(source);
                }
                sourcePools.Clear();
            }
            StopCoroutine("PlayInSequence");
            sequenceAudioSource.Stop();
            sourcePools = new List<AudioSource>();
            sequenceAudioSource.Stop();
            mainAudioSource.Stop();
        }

        private IEnumerator PlayInSequence(Queue<AudioClip> clipsInSequence, float delay, Action OnSequenceComplete = null)
        {
            yield return new WaitForSeconds(delay);
            while (clipsInSequence.Count > 0)
            {
                sequenceAudioSource.Stop();
                sequenceAudioSource.clip = clipsInSequence.Dequeue();
                sequenceAudioSource.Play();
                while (sequenceAudioSource.isPlaying)
                {
                    yield return null;
                }
                yield return null;
            }
            OnSequenceComplete?.Invoke();
        }

        #endregion -------------------------------------------------------------------------


        #region --------------------------------- Public Methods ---------------------------

        public void InitManager()
        {
            sourcePools = new List<AudioSource>();
            mainAudioSource = gameObject.AddComponent<AudioSource>();
            sequenceAudioSource = gameObject.AddComponent<AudioSource>();
            unlockItemAudioSource = gameObject.AddComponent<AudioSource>();
            uiAudioSource = gameObject.AddComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void PlayAudio(AudioClip clip, float volume, bool isLoop, bool addToPool)
        {
            if (addToPool)
            {
                if (sourcePools.Count >= maxPoolSize)
                {
                    bool clipAdded = false;
                    for (int i = 0; i < sourcePools.Count; i++)
                    {
                        if (!sourcePools[i].isPlaying)
                        {
                            sourcePools[i].clip = clip;
                            sourcePools[i].volume = volume;
                            sourcePools[i].loop = isLoop;
                            sourcePools[i].Play();
                            clipAdded = true;
                            break;
                        }
                    }
                    if (!clipAdded)
                    {
                        sourcePools[sourcePools.Count - 1].clip = clip;
                        sourcePools[sourcePools.Count - 1].volume = volume;
                        sourcePools[sourcePools.Count - 1].loop = isLoop;
                        sourcePools[sourcePools.Count - 1].Play();
                    }
                }
                else
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.volume = volume;
                    source.clip = clip;
                    source.loop = isLoop;
                    source.Play();
                    sourcePools.Add(source);
                }
            }
            else
            {
                mainAudioSource.Stop();
                mainAudioSource.clip = clip;
                mainAudioSource.volume = volume;
                mainAudioSource.loop = isLoop;
                mainAudioSource.Play();
            }
        }

        public void PlayUnlockItemAudio(AudioClip clip)
        {
            unlockItemAudioSource.clip = clip;
            unlockItemAudioSource.Play();
        }

        public void StopUnlockItemAudio()
        {
            unlockItemAudioSource.Stop();
        }

        public void PlayUISound(AudioClip clip)
        {
            uiAudioSource.clip = clip;
            uiAudioSource.volume = 0.5f;
            uiAudioSource.loop = false;
            uiAudioSource.Play();
        }

        public void PlayAudioInSequence(List<AudioClip> clips, float delay, Action OnSequenceComplete = null)
        {
            StopCoroutine("PlayInSequence");
            sequenceAudioSource.Stop();
            StartCoroutine(PlayInSequence(new Queue<AudioClip>(clips), delay, OnSequenceComplete));
        }
        #endregion ----------------------------------------------------------------------------
    }
}