using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ECTS.GUI.InGameMenu
{
    /// <summary>
    /// shows current resources on the top left of the game
    /// </summary>
    [DataContract]
    public sealed class ResourceMenu
    {
        private Texture2D mBackgroundTexture;
        private Texture2D mFoodTexture2D;
        internal Texture2D mMetalTexture2D;
        internal Texture2D mWoodTexture2D;
        internal Texture2D mStoneTexture2D;
        private GameLoop GameLoop { get; }
        private const float CountDuration = 200f; //every  2s.
        private float mCurrentTime;
        public Rectangle mResourceMenuRectangle;

        public ResourceMenu(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }

        public void LoadContent()
        {
            mBackgroundTexture = GameLoop.Content.Load<Texture2D>("Menu/ResourcesHud");
            mFoodTexture2D = GameLoop.Content.Load<Texture2D>("Player/Food_Icon");
            mMetalTexture2D = GameLoop.Content.Load<Texture2D>("Player/Metall_Icon");
            mWoodTexture2D = GameLoop.Content.Load<Texture2D>("Player/Wood_Icon");
            mStoneTexture2D = GameLoop.Content.Load<Texture2D>("Player/Stone_Icon");
        }

        /// <summary>
        /// Resources shown on map
        /// </summary>
        public void Render()
        {
            // edge length of the squared icon
            var iconSize = (int)Math.Min(GameLoop.DisplayWidth * 0.036 * 0.6, GameLoop.DisplayHeight * 0.065 * 0.6);
            // draws the amount
            var font = GameLoop.RenderManager.FontSelector.GetFont(GameLoop.DisplayWidth, FontSelector.FontSize.Small);


            mResourceMenuRectangle = new Rectangle(0, 0, (int)(GameLoop.DisplayWidth * 0.37),
                (int)(GameLoop.DisplayHeight * 0.06));
            GameLoop.SpriteBatch.Draw(mBackgroundTexture, mResourceMenuRectangle, Color.White);

            GameLoop.SpriteBatch.Draw(mFoodTexture2D,
                new Rectangle((int)(GameLoop.DisplayWidth * 0.05),  (int)(GameLoop.DisplayHeight * 0.009),
                    iconSize, iconSize), Color.White);
            GameLoop.SpriteBatch.DrawString(font, "" + (int)GameLoop.ObjectManager.DataManager.ResourceList[3],
                new Vector2((int)(GameLoop.DisplayWidth * 0.08), (int)(GameLoop.DisplayHeight * 0.026 * 0.6)),
                Color.White);

            GameLoop.SpriteBatch.Draw(mMetalTexture2D,
                new Rectangle((int)(GameLoop.DisplayWidth * 0.12), (int)(GameLoop.DisplayHeight * 0.011),
                    iconSize, iconSize), Color.White);
            GameLoop.SpriteBatch.DrawString(font, "" + (int)GameLoop.ObjectManager.DataManager.ResourceList[0],
                new Vector2((int)(GameLoop.DisplayWidth * 0.15), (int)(GameLoop.DisplayHeight * 0.026 * 0.6)),
                Color.White);

            GameLoop.SpriteBatch.Draw(mWoodTexture2D,
                new Rectangle((int)(GameLoop.DisplayWidth * 0.19), (int)(GameLoop.DisplayHeight * 0.011),
                    iconSize, iconSize), Color.White);
            GameLoop.SpriteBatch.DrawString(font, "" + (int)GameLoop.ObjectManager.DataManager.ResourceList[2],
                new Vector2((int)(GameLoop.DisplayWidth * 0.22), (int)(GameLoop.DisplayHeight * 0.026 * 0.6)),
                Color.White);

            GameLoop.SpriteBatch.Draw(mStoneTexture2D,
                new Rectangle((int)(GameLoop.DisplayWidth * 0.26), (int)(GameLoop.DisplayHeight * 0.009),
                    iconSize, iconSize), Color.White);
            GameLoop.SpriteBatch.DrawString(font, "" + (int)GameLoop.ObjectManager.DataManager.ResourceList[1],
                new Vector2((int)(GameLoop.DisplayWidth * 0.29), (int)(GameLoop.DisplayHeight * 0.026 * 0.6)),
                Color.White);
        }
 
        public void Update(GameTime gameTime)
        {
            mCurrentTime += (float)gameTime.TotalGameTime.TotalSeconds; //Time passed 

            if (mCurrentTime >= CountDuration)
            {
                mCurrentTime -= CountDuration; // "use up" the time
                //any actions to perform
            }
        }
    }
}