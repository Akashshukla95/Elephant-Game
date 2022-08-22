using UnityEngine;
using ElesJourney.Utils;
using System.Collections;
using ElesJourney.Animations;
using ElesJourney.Components;
using ElesJourney.Managers;
using ElesJourney.Models;
using ElesJourney.Controllers;
using ElesJourney.GlobalStrings;
using System.Collections.Generic;

namespace ElesJourney.Views
{
    public class MechanicView : MonoBehaviour
    {
        [SerializeField] private MechanicController mechanicController;
        [SerializeField] private AudioClip containerToggleClip;
        [SerializeField] private AudioClip puzzleFormClip;

        #region --------------------- Association Mechanic ----------------------
        [Header("Association Gameobjects")]
        [SerializeField] private SpriteRenderer associationWordContainer;

        #endregion ---------------------------------------------------------------

        #region ----------------------- Puzzle Mechanic --------------------------

        [Header("Puzzle Gameobjects")]
        [SerializeField] private GameObject puzzleWordContainer;
        [SerializeField] private GameObject splitSpritePrefab;
        private Sprite puzzleSprite;
        private List<GameObject> splitSprites;
        private float puzzleSpritesSpacing = 0.5f;
        private SplitAxis currentSplitAxis;
        private Vector2[] puzzleSpriteActualPosition;
        private float puzzleOutroAnimationDuration = 0.2f;

        #endregion -------------------------------------------------------------------

        #region ---------------------- SpinWheel Mechanic ----------------------------
        [Header("Wheel Gameobjects")]
        [SerializeField] private GameObject wheelWordContainer;
        [SerializeField] private GameObject spinWheel;

        #endregion -------------------------------------------------------------------

        #region ---------------------- Shadow Mechanic --------------------------------
        [Header("Shadow Gameobjects")]
        [SerializeField] private SpriteRenderer shadowWordContainer;
        [SerializeField] private Material shadowSpriteMaterial;
        [SerializeField] private Material spriteDefaultMaterial;

        #endregion---------------------------------------------------------------------

        private IEnumerator PlayPuzzleOutroAnimation(Vector2 zeroPos, Vector2 onePos, Vector2 twoPos)
        {
            float timeQuant = 0;
            Vector2 startPos0 = Vector2.zero;
            Vector2 startPos1 = Vector2.zero;
            Vector2 startPos2 = Vector2.zero;

            startPos0 = splitSprites[0].transform.position;
            startPos1 = splitSprites[1].transform.position;
            if (splitSprites.Count == 3)
                startPos2 = splitSprites[2].transform.position;
            SoundManager.Instance.PlayAudio(puzzleFormClip, 0.3f, false, true);
            while (timeQuant < 1)
            {
                splitSprites[0].transform.position = Vector2.Lerp(startPos0, zeroPos, timeQuant);
                splitSprites[1].transform.position = Vector2.Lerp(startPos1, onePos, timeQuant);
                if (splitSprites.Count == 3)
                {
                    splitSprites[2].transform.position = Vector2.Lerp(startPos2, twoPos, timeQuant);
                }
                timeQuant += Time.deltaTime / puzzleOutroAnimationDuration;
                yield return new WaitForEndOfFrame();
            }
            splitSprites[0].transform.position = zeroPos;
            splitSprites[1].transform.position = onePos;
            if (splitSprites.Count == 3)
            {
                splitSprites[2].transform.position = twoPos;
            }
            puzzleWordContainer.GetComponent<WaveEffect>().BeginWave();
            puzzleWordContainer.GetComponent<Animator>().Play(GameStrings.wordOutroAnimation);
        }

        #region ---------------------------- Public Methods --------------------------

        public void OnAssociationIntro(Sprite wordSprite)
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            associationWordContainer.sprite = wordSprite;
            associationWordContainer.GetComponent<Animator>().Play(GameStrings.wordIntroAnimation);
            mechanicController.OnIntroComplete();
        }

        public void OnPuzzleIntro(Sprite originalSprite, List<Sprite> puzzledSprites, SplitAxis currentSplitAxis)
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            puzzleWordContainer.GetComponent<Animator>().Play(GameStrings.defaultStateAnim);
            puzzleWordContainer.transform.localScale = Vector2.one;
            this.currentSplitAxis = currentSplitAxis;
            puzzleSprite = originalSprite;
            Vector2[] colliderSize = new Vector2[puzzledSprites.Count];

            if (splitSprites != null)
            {
                foreach (GameObject gSprite in splitSprites)
                {
                    Destroy(gSprite.gameObject);
                }
            }

            splitSprites = new List<GameObject>();
            for (int i = 0; i < puzzledSprites.Count; i++)
            {
                GameObject gSprite = Instantiate(splitSpritePrefab);
                gSprite.transform.parent = puzzleWordContainer.transform;
                gSprite.transform.localPosition = Vector2.zero;
                gSprite.transform.localScale = Vector2.one;
                gSprite.GetComponent<SpriteRenderer>().sprite = puzzledSprites[i];
                BoxCollider2D box2D = gSprite.AddComponent<BoxCollider2D>();
                colliderSize[i] = box2D.size;
                splitSprites.Add(gSprite);
            }

            if (currentSplitAxis is SplitAxis.HORIZONTAL_SPLIT) // Will only work for 2 and 3
            {
                if (splitSprites.Count == 2)
                {

                    splitSprites[0].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.down * colliderSize[0].y * 0.5f;
                    splitSprites[1].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.up * colliderSize[1].y * 0.5f;
                }
                else
                {
                    splitSprites[1].transform.position = puzzleWordContainer.transform.position;
                    splitSprites[0].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.down * colliderSize[0].y;
                    splitSprites[2].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.up * colliderSize[2].y;
                }
            }
            else
            {
                if (splitSprites.Count == 2)
                {
                    splitSprites[0].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.left * colliderSize[0].x * 0.5f;
                    splitSprites[1].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.right * colliderSize[1].x * 0.5f;
                }
                else
                {
                    splitSprites[1].transform.position = puzzleWordContainer.transform.position;
                    splitSprites[0].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.left * colliderSize[0].x;
                    splitSprites[2].transform.position = (Vector2)puzzleWordContainer.transform.position + Vector2.right * colliderSize[2].x;
                }
            }

            for (int i = 0; i < splitSprites.Count; i++)
            {
                splitSprites[i].GetComponent<Animator>().Play(GameStrings.scaleInAnimation);
            }

            int length = splitSprites.Count;
            puzzleSpriteActualPosition = new Vector2[length];
            for (int i = 0; i < length; i++)
            {
                puzzleSpriteActualPosition[i] = splitSprites[i].transform.position;
            }

            if (length == 3)
            {
                splitSprites[2].transform.position = puzzleSpriteActualPosition[0];
                splitSprites[0].transform.position = puzzleSpriteActualPosition[2];
                splitSprites[1].transform.position = puzzleSpriteActualPosition[1];
            }
            else
            {
                splitSprites[0].transform.position = puzzleSpriteActualPosition[1];
                splitSprites[1].transform.position = puzzleSpriteActualPosition[0];
            }

            puzzleWordContainer.GetComponent<WaveEffect>().BeginWave();
            mechanicController.OnIntroComplete();
        }

        public void ShowWheelWordSprite()
        {
            spinWheel.GetComponent<SpinWheelComponent>().ExitSpinWheel();
            mechanicController.OnIntroComplete();
        }

        public void OnSpinWheelIntro(List<Sprite> wordSprites, List<AudioClip> wheelClips)
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            Debug.LogFormat("{0} Message: {1}", "[MechanicView]", "In Spin Wheel Intro");
            Sprite wordSprite = wordSprites[0];
            spinWheel.GetComponent<SpinWheelComponent>().InitSpinWheel(
                wordSprites,
                delegate
                {
                    Word word = GameManager.Instance.GetCurrentWordData();
                    if (word.isGuided && word.voiceOvers.Count > 0)
                    {
                        SoundManager.Instance.PlayAudioInSequence(wheelClips, 0.5f,
                                            delegate
                                            {
                                                wheelWordContainer.GetComponent<Animator>().Play(GameStrings.wordIntroAnimation);
                                                wheelWordContainer.GetComponent<SpriteRenderer>().sprite = wordSprite;
                                                ShowWheelWordSprite();
                                            }
                                        );
                    }
                    else
                    {
                        wheelWordContainer.GetComponent<Animator>().Play(GameStrings.wordIntroAnimation);
                        wheelWordContainer.GetComponent<SpriteRenderer>().sprite = wordSprite;
                        ShowWheelWordSprite();
                    }
                }
            );
        }

        public void OnShadowIntro(Sprite wordSprite)
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            shadowWordContainer.material = shadowSpriteMaterial;
            shadowWordContainer.sprite = wordSprite;
            shadowWordContainer.GetComponent<Animator>().Play(GameStrings.wordIntroAnimation);
            mechanicController.OnIntroComplete();
        }

        public void OnAssociationOutro(bool hasWon)
        {
            if (hasWon)
            {
                SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
                associationWordContainer.GetComponent<Animator>().Play(GameStrings.wordOutroAnimation);
            }
        }

        public void OnPuzzleOutro(bool hasWon)
        {
            if (hasWon)
            {
                puzzleWordContainer.GetComponent<WaveEffect>().EndWave();
                Vector2 newPos0;
                Vector2 newPos1;
                Vector2 newPos2 = Vector2.zero;

                if (splitSprites.Count == 3)
                {
                    newPos2 = puzzleSpriteActualPosition[2];
                }
                newPos0 = puzzleSpriteActualPosition[0];
                newPos1 = puzzleSpriteActualPosition[1];
                StartCoroutine(PlayPuzzleOutroAnimation(newPos0, newPos1, newPos2));
            }
        }

        public void OnSpinWheelOutro(bool hasWon)
        {
            if (hasWon)
                wheelWordContainer.GetComponent<Animator>().Play(GameStrings.wordOutroAnimation);
        }

        public void OnShadowOutro(bool hasWon)
        {
            if (hasWon)
            {
                SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
                shadowWordContainer.material = spriteDefaultMaterial;
                shadowWordContainer.GetComponent<Animator>().Play(GameStrings.wordOutroAnimation);
            }
        }

        public void OnAssociationClosure()
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            associationWordContainer.GetComponent<Animator>().Play(GameStrings.wordClosureAnimation);
        }

        public void OnPuzzleClosure()
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            puzzleWordContainer.GetComponent<WaveEffect>().EndWave();
            puzzleWordContainer.GetComponent<Animator>().Play(GameStrings.wordClosureAnimation);
        }

        public void OnSpinWheelClosure()
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            wheelWordContainer.GetComponent<Animator>().Play(GameStrings.wordClosureAnimation);
        }

        public void OnShadowClosure()
        {
            SoundManager.Instance.PlayAudio(containerToggleClip, 0.5f, false, true);
            shadowWordContainer.GetComponent<Animator>().Play(GameStrings.wordClosureAnimation);
        }

        #endregion -----------------------------------------------------------------------------------
    }
}
