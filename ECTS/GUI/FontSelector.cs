using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.GUI
{
    /// <summary>
    /// selects the correct font size depending on the display width
    /// </summary>
    internal sealed class FontSelector
    {
        private sealed class FontItem
        {
            public int ScreenWidth { get; set; }
            public SpriteFont Font { get; set; }
            public string FontName { get; set; }
        }

        private readonly List<FontItem> mFontItems;
        private readonly ContentManager mContentManager;

        /// <summary>
        /// different font sizes
        /// </summary>
        internal enum FontSize
        {
            Small,
            Medium,
            Large
        }

        /// <summary>
        /// contains a table of screen width and their font size name
        /// </summary>
        /// <param name="content"></param>
        internal FontSelector(ContentManager content)
        {
            mContentManager = content;
            mFontItems = new List<FontItem>
            {
                new FontItem {ScreenWidth =    1, FontName = "Arial10"},
                new FontItem {ScreenWidth = 1000, FontName = "Arial20"},
                new FontItem {ScreenWidth = 2000, FontName = "Arial30"},
                new FontItem {ScreenWidth = 3000, FontName = "Arial40"},
                new FontItem {ScreenWidth = 4000, FontName = "Arial50"},
                new FontItem {ScreenWidth = 5000, FontName = "Arial60"}
            };
        }

        /// <summary>
        /// selects the font depending on the display width
        /// </summary> 
        /// <param name="screenWidth">positive int</param>
        /// <param name="fontSize">fontSize enum: small, medium, large</param>
        /// <returns>SpriteFont</returns>
        internal SpriteFont GetFont(int screenWidth, FontSize fontSize)
        {
            var closestFontIndex = 0;

            // checks which entry is the most similar
            for (var fontIndex = 0; fontIndex < 6; fontIndex++)
            {
                if (Math.Abs(mFontItems[fontIndex].ScreenWidth - screenWidth) < Math.Abs(mFontItems[closestFontIndex].ScreenWidth - screenWidth))
                {
                    closestFontIndex = fontIndex;
                }
            }

            if (fontSize == FontSize.Small && closestFontIndex > 0)
            {
                closestFontIndex -= 1;
            }
            if (fontSize == FontSize.Large && closestFontIndex < 5)
            {
                closestFontIndex += 1;
            }

            return mFontItems[closestFontIndex].Font ??
                   (mFontItems[closestFontIndex].Font =
                       mContentManager.Load<SpriteFont>("Font/" + mFontItems[closestFontIndex].FontName));
        }
    }
}
