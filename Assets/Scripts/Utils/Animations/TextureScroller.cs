using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace ElesJourney.Animations
{
    public class TextureScroller : MonoBehaviour
    {
        #region   --------------- Serialize Fields --------------- 
#pragma warning disable 649
        [SerializeField] private Vector2 scrollDirection;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private bool allowAutoScroll;
#pragma warning restore 649
        #endregion -----------------------------------------------


        #region   --------------- Private Fields --------------- 
        private Material imageMaterial;
        private float currentX;
        private float currentY;

        #endregion -----------------------------------------------


        #region   --------------- Private Methods --------------- 
        private void Start()
        {
            imageMaterial = GetComponent<Image>().material;
            if (allowAutoScroll)
                StartCoroutine(Scroll());
        }

        private IEnumerator Scroll()
        {
            while (true)
            {
                currentX += scrollDirection.x * (Time.deltaTime * scrollSpeed);
                currentY += scrollDirection.y * (Time.deltaTime * scrollSpeed);
                imageMaterial.SetTextureOffset("_MainTex", new Vector2(currentX, currentY));
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion -----------------------------------------------
    }
}


