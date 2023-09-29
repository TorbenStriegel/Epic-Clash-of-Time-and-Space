using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.GUI.InGameMenu
{
    public sealed class PopUp
    {
        private readonly GameLoop mGameLoop;
        private readonly Texture2D mTexture;
        private readonly string mMessage;
        private readonly Texture2D mImage;
        public Vector2 mPosition;
        public TimeSpan mLifeSpan;

        public PopUp(GameLoop gameLoop, Texture2D image, string message = "You are the winner!")
        {
            mGameLoop = gameLoop;
            mTexture = mGameLoop.Content.Load<Texture2D>("Menu/blueHud");
            mMessage = message;
            mImage = image;
            mPosition = new Vector2(mGameLoop.DisplayWidth, mGameLoop.DisplayHeight * 1 / 4f);
            mLifeSpan = TimeSpan.Zero;
    }

    public void Draw()
        {
            mGameLoop.SpriteBatch.Draw(mTexture, mPosition, Color.White);
            mGameLoop.SpriteBatch.Draw(mImage, mPosition, null, Color.White, 0f, Vector2.Zero, mTexture.Height / (float) mImage.Height, SpriteEffects.None, 0);
            mGameLoop.SpriteBatch.DrawString(
                mGameLoop.RenderManager.FontSelector.GetFont(mGameLoop.DisplayWidth,
                    FontSelector.FontSize.Small),
                mMessage,
                mPosition +
                new Vector2(mImage.Width * mTexture.Height / (float)mImage.Height + 20,
                    50),
                Color.White);
        }

    }
}
