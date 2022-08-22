using UnityEngine;
using System.Collections.Generic;

namespace ElesJourney.Utils
{
    public enum SplitAxis
    {
        HORIZONTAL_SPLIT,
        VERTICAL_SPLIT
    }

    public static class SpriteSplitter
    {
        public static List<Sprite> SplitSprites(Texture2D texture, int numberOfSplits, SplitAxis splitDirection)
        {
            Debug.LogFormat(
                "{0} \n Message: {1} \n Number of Splits: {2} \n Split Direction: {3} ", "SpriteSplitter", "Sprite Split Data",
                numberOfSplits,
                splitDirection
            );

            List<Sprite> slicedSprites = new List<Sprite>();
            int height = texture.height;
            int width = texture.width;
            int cutFactor = splitDirection == SplitAxis.HORIZONTAL_SPLIT ? Mathf.RoundToInt(height / numberOfSplits) :
                                                                            Mathf.RoundToInt(width / numberOfSplits);
            float x = 0;
            float y = 0;
            float w = 0;
            float h = 0;

            for (int i = 0; i < numberOfSplits; i++)
            {
                if (splitDirection is SplitAxis.HORIZONTAL_SPLIT)
                {
                    x = 0;
                    y = cutFactor * i;
                    w = width;
                    h = cutFactor * (i + 1) - cutFactor * i;
                }
                else
                {
                    x = cutFactor * i;
                    y = 0;
                    w = cutFactor * (i + 1) - cutFactor * i;
                    h = height;
                }
                Sprite currentSprite = Sprite.Create(texture, new Rect(x, y, w, h), Vector2.one * 0.5f, 100, 1, SpriteMeshType.Tight, Vector4.zero);
                slicedSprites.Add(currentSprite);
            }
            return slicedSprites;
        }
    }
}