using System;
using UnityEngine;
using UnityEngine.UI;
using ElesJourney.Managers;
using ElesJourney.Animations;
using ElesJourney.Components;
using ElesJourney.Controllers;
using System.Collections.Generic;
using System.Collections;


namespace ElesJourney.Views
{

    public class WorldView : MonoBehaviour
    {

        [Serializable]
        private class SceneItems
        {
            public GameObject scenePanel;
            public List<ItemComponent> unlockableItems;
        }

        #region -------------------- Serialize Fields ---------------------
#pragma warning disable 649
        [SerializeField] private WorldController worldController;
        [SerializeField] private AnimationCurve scrollAnimCurve;
        [SerializeField] private Canvas myCanvas;
        [SerializeField] private List<GameObject> scenePanels;
        [SerializeField] private GameObject rectContent;
        [SerializeField] private Button playButton;
        [SerializeField] private List<SceneItems> sceneItems;
        [SerializeField] private ParticleSystem unlockParticle;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip itemUnlockClip;
        [SerializeField] private AudioClip worldAudioClip;

#pragma warning restore 649
        #endregion ---------------------------------------------------------

        #region -------------------- Private Fields ---------------------

        private float scrollAnimationDuration = 1.5f;

        #endregion ---------------------------------------------------------

        #region -------------------- Private Methods ---------------------

        private void Start()
        {
            playButton.onClick.AddListener(OnPlayClick);
            GetComponent<CanvasGroup>().alpha = 0;
            SoundManager.Instance.PlayAudio(worldAudioClip, 0.2f, true, true);
        }

        private void OnPlayClick()
        {
            if (GameManager.Instance.allowUserInput)
            {
                SoundManager.Instance.PlayUISound(buttonClickClip);
                GameManager.Instance.LoadScene(ElesJourney.GlobalStrings.GameStrings.gameScene);
            }
            // CloseWorld();
        }

        private IEnumerator UnlockScene()
        {
            float timeQuant = 0;
            int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
            sceneItems[sceneIndex].scenePanel.SetActive(true);
            Vector2 startPos = rectContent.GetComponent<RectTransform>().anchoredPosition;
            Vector2 endPos = new Vector2(-(Screen.width * sceneIndex), 0);
            while (timeQuant < 1)
            {
                rectContent.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(
                                                                                        startPos,
                                                                                        endPos,
                                                                                        scrollAnimCurve.Evaluate(timeQuant)
                                                                                    );
                yield return new WaitForEndOfFrame();
                timeQuant += Time.deltaTime / scrollAnimationDuration;
            }
            rectContent.GetComponent<RectTransform>().anchoredPosition = endPos;
            GameManager.Instance.allowUserInput = true;
            ToggleInputUI(true);
        }

        private IEnumerator ScrollScene()
        {
            yield return new WaitForEndOfFrame();
            int index = GameManager.Instance.GetUnlockSceneIndex();
            rectContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                -(Screen.width * index),
                0
            );
        }


        #endregion ---------------------------------------------------------

        #region -------------------- Public Methods ------------------------

        public void ShowWorld(Action OnWorldShowComplete)
        {
            myCanvas.GetComponent<SceneFade>().FadeOut(
               delegate
               {
                   OnWorldShowComplete.Invoke();
               }
            );
        }

        public void ShowNewScene()
        {
            StartCoroutine(UnlockScene());
        }

        public void UnlockItem(int index, Action OnUnlockComplete)
        {
            int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
            ItemComponent itemComponent = sceneItems[sceneIndex].unlockableItems[index];
            itemComponent.gameObject.SetActive(true);
            itemComponent.UnlockItem(OnUnlockComplete, itemUnlockClip);
        }

        public void ToggleInputUI(bool toggleOn)
        {
            playButton.gameObject.SetActive(toggleOn);
        }

        public void SetupItems()
        {
            int itemIndex = GameManager.Instance.GetItemUnlockIndex();
            int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
            for (int i = 0; i < sceneItems.Count; i++)
            {
                if (i <= sceneIndex)
                {
                    int limit = i == sceneIndex ? itemIndex : sceneItems[i].unlockableItems.Count;
                    sceneItems[i].scenePanel.SetActive(true);
                    for (int j = 0; j < limit; j++)
                    {
                        ItemComponent unlockedObject = sceneItems[i].unlockableItems[j];
                        unlockedObject.gameObject.SetActive(true);
                        unlockedObject.transform.localScale = Vector3.one;
                    }
                }
                else break;
            }
            StartCoroutine(ScrollScene());
        }
        #endregion ---------------------------------------------------------

    }

}

