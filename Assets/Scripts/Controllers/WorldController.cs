using System;
using UnityEngine;
using ElesJourney.Views;
using System.Collections;
using ElesJourney.Managers;
using ElesJourney.Scriptables;
using System.Collections.Generic;

namespace ElesJourney.Controllers
{
    public class WorldController : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private NarrationController narrationController;
        [SerializeField] private List<WorldView> worldViews;
        [SerializeField] private UnlockableElementsScriptable unlockElementsData;
#pragma warning restore 649
        private int currentWorldIndex = 0;

        private void Start()
        {
            ShowWorld();
        }

        private void ShowWorld()
        {
            worldViews[currentWorldIndex].ToggleInputUI(false);
            worldViews[currentWorldIndex].ShowWorld(
                delegate
                {
                    narrationController.InitNarration();
                }
            );
        }

        private IEnumerator ShowNextNarrationState(int itemIndex)
        {
            int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
            yield return StartCoroutine(narrationController.PlayDialogue(unlockElementsData.unlockData[sceneIndex].unlockableItems[itemIndex - 1].dialogue));
            yield return new WaitForSeconds(1);

            bool hasUnlockedNewScene = itemIndex >= unlockElementsData.unlockData[sceneIndex].unlockableItems.Count ? true : false;
            sceneIndex = hasUnlockedNewScene ? sceneIndex + 1 : sceneIndex;
            itemIndex = hasUnlockedNewScene ? 0 : itemIndex;
            GameManager.Instance.SetUnlockSceneIndex(sceneIndex);
            GameManager.Instance.SetItemToUnlockIndex(itemIndex);
            if (hasUnlockedNewScene)
                worldViews[currentWorldIndex].ShowNewScene();
            else
            {
                GameManager.Instance.allowUserInput = true;
                worldViews[currentWorldIndex].ToggleInputUI(true);
            }
        }

        public void SetWorldItems()
        {
            worldViews[currentWorldIndex].SetupItems();
            if (GameManager.Instance.HasUnlockedItemInLastGame())
            {
                GameManager.Instance.allowUserInput = false;
                GameManager.Instance.SetItemUnlockStatus(false);
                int sceneIndex = GameManager.Instance.GetUnlockSceneIndex();
                int itemIndex = GameManager.Instance.GetItemUnlockIndex();
                itemIndex++;
                worldViews[currentWorldIndex].UnlockItem(
                    itemIndex - 1,
                    delegate
                    {
                        StartCoroutine(ShowNextNarrationState(itemIndex));
                    }
                );
            }
            else
            {
                worldViews[currentWorldIndex].ToggleInputUI(true);
            }
            GameManager.Instance.allowUserInput = true;
        }
    }
}
