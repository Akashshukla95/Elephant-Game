using UnityEngine;
using ElesJourney.Views;
using ElesJourney.Utils;
using ElesJourney.Models;
using ElesJourney.Managers;
using System.Collections.Generic;

namespace ElesJourney.Controllers
{
    public class MechanicController : MonoBehaviour
    {

        #region ------------------ Serialize Fields --------------------------
#pragma warning disable 649
        [SerializeField] private MechanicView mechanicView;
        [SerializeField] private GameController gameController;
        [SerializeField] private WordController wordController;

#pragma warning restore 649
        #endregion -------------------------------------------------------------

        private bool hasPlayedWheel = false;

        #region -------------------- Public Methods -----------------------------

        public void ToggleMechanic(bool isInit)
        {
            switch (GameManager.Instance.GetCurrentMechanicType())
            {
                case MechanismType.ASSOCIATION:

                    Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "In Association Mechanic");

                    if (isInit)
                    {
                        mechanicView.OnAssociationIntro(GameManager.Instance.GetCurrentWordData().wordSprite);
                    }
                    else
                    {
                        Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "Close Association Mechanic");
                        mechanicView.OnAssociationClosure();
                    }
                    break;

                case MechanismType.PUZZLE:

                    Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "In Puzzle Mechanic");

                    if (isInit)
                    {
                        Sprite wordSprite = GameManager.Instance.GetCurrentWordData().wordSprite;
                        if (wordSprite is null)
                        {
                            Debug.LogFormat("{0} \n Message : {1}", "[MechanicController]", "Sprite Data missing in Resources");
                            return;
                        }
                        SplitAxis currentSplitAxis = (SplitAxis)Mathf.RoundToInt(Random.value);
                        int totalSplits = 2 + Mathf.RoundToInt(Random.value);
                        List<Sprite> puzzledSprites = SpriteSplitter.SplitSprites(wordSprite.texture, totalSplits, currentSplitAxis);
                        mechanicView.OnPuzzleIntro(wordSprite, puzzledSprites, currentSplitAxis);
                    }
                    else
                    {
                        Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "Close Puzzle Mechanic");
                        mechanicView.OnPuzzleClosure();
                    }
                    break;

                case MechanismType.SPIN_WHEEL:

                    Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "In Spin Wheel Mechanic");

                    if (isInit)
                    {
                        if (!hasPlayedWheel)
                        {
                            hasPlayedWheel = true;
                            string wordString = string.Empty;
                            List<Sprite> wordSprites = new List<Sprite>();
                            wordSprites.Add(GameManager.Instance.GetCurrentWordData().wordSprite);
                            List<Sprite> randomSprites = gameController.GetRandomWordSprites();
                            foreach (Sprite s in randomSprites)
                            {
                                wordSprites.Add(s);
                            }
                            mechanicView.OnSpinWheelIntro(wordSprites, wordController.GetVoiceOverClips(VoiceOverType.EXTRA));
                        }
                        else
                        {
                            mechanicView.ShowWheelWordSprite();
                        }
                    }
                    else
                    {
                        Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "Close Spin Wheel Mechanic");
                        mechanicView.OnSpinWheelClosure();
                    }
                    break;

                case MechanismType.SHADOW:
                    if (isInit)
                    {
                        mechanicView.OnShadowIntro(GameManager.Instance.GetCurrentWordData().wordSprite);
                    }
                    else
                    {
                        Debug.LogFormat("{0} \n Message:{1}", "[MechanicController]", "Close Shadow Mechanic");
                        mechanicView.OnShadowClosure();
                    }
                    break;
            }
        }

        public void OnIntroComplete()
        {
            gameController.GenerateNewWord();
        }

        public void ProcessMechanicOutro(bool hasWon)
        {
            if (hasWon)
            {
                switch (GameManager.Instance.GetCurrentMechanicType())
                {
                    case MechanismType.ASSOCIATION:
                        mechanicView.OnAssociationOutro(hasWon);
                        break;

                    case MechanismType.PUZZLE:
                        mechanicView.OnPuzzleOutro(hasWon);
                        break;

                    case MechanismType.SPIN_WHEEL:
                        hasPlayedWheel = hasWon ? false : true;
                        mechanicView.OnSpinWheelOutro(hasWon);
                        break;

                    case MechanismType.SHADOW:
                        mechanicView.OnShadowOutro(hasWon);
                        break;
                }
            }
        }

        #endregion -----------------------------------------------------------------
    }
}
