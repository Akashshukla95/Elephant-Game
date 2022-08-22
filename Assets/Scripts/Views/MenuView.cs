using UnityEngine;
using UnityEngine.UI;
using ElesJourney.Controllers;
using ElesJourney.Animations;
using ElesJourney.Managers;
using ElesJourney.GlobalStrings;

namespace ElesJourney.Views
{
    public class MenuView : MonoBehaviour
    {

        #region ------------------------ Serialize Fields -----------------------------
#pragma warning disable 649
        [SerializeField] private Button playButton;
        [SerializeField] private GameObject canvas;
        [SerializeField] private MenuController menuController;
        [SerializeField] private AudioClip buttonClick;
#pragma warning restore 649
        #endregion ---------------------------------------------------------------------

        #region --------------------------- Private Methods ------------------------------

        private void Start()
        {
            playButton.onClick.AddListener(OnPlayClick);
        }
        private void OnPlayClick()
        {
            SoundManager.Instance.PlayUISound(buttonClick);
            canvas.GetComponent<SceneFade>().FadeIn(
              delegate
              {
                  menuController.PlayGame();
              }
            );
        }

        #endregion ---------------------------------------------------------------------
    }
}

