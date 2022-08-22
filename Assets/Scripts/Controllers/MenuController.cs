using UnityEngine;
using ElesJourney.Views;
using ElesJourney.Models;
using ElesJourney.Managers;
using ElesJourney.GlobalStrings;

namespace ElesJourney.Controllers
{
    public class MenuController : MonoBehaviour
    {

        #region ------------------- Serialize Fields --------------------------
#pragma warning disable 649
        [SerializeField] private MenuView menuView;
        [SerializeField] private AudioClip menuBackgroundClip;

#pragma warning restore 649
        #endregion --------------------------------------------------------------

        #region -------------------- Private Methods ----------------------------

        private void Start()
        {
            GameManager.Instance.currentApplicationState = ApplicationState.MENU_STATE;
            SoundManager.Instance.InitManager();
            SoundManager.Instance.PlayAudio(menuBackgroundClip, 1, true, false);
        }

        #endregion ----------------------------------------------------------------

        #region ------------------- Public Methods ------------------------------

        public void PlayGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameStrings.narrationScene);
        }

        #endregion ----------------------------------------------------------------
    }

}
