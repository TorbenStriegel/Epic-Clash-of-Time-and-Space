using System;
using System.Collections.Generic;
using ECTS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.States
{
    /// <summary>
    /// Renders Tutorial Button and Tutorial image in game
    /// </summary>
    internal sealed class TutorialState : State
    {
        private GameLoop GameLoop { get; }
        private State PrevState { get; }
        private Button mInvisibleButton;
        private Rectangle mTutorialPositionRectangle;
        private Texture2D mTutorialTexture;
        private Texture2D mInvisibleTexture;
        private Texture2D mBlackBackground;
        private readonly bool mSecondClick;
        private bool mAreButtonsInit;
        private List<Component> mComponents;

        public TutorialState(GameLoop gameLoop, State prevState) : base(gameLoop)
        {
            GameLoop = gameLoop;
            PrevState = prevState;
            GameLoop.RenderManager.ScreenManager.Tutorial.mIsTutorialOpen = false;
            mSecondClick = false;
        }

        /// <summary>
        /// opens tutorial picture and closes it
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (mAreButtonsInit)
            {
                foreach (var component in mComponents)
                {
                    component.Update();
                }
            }


            if (mSecondClick && GameLoop.InputManager.mLeftMouseButtonJustReleased)
            {
                GameLoop.RenderManager.ScreenManager.Tutorial.mIsTutorialOpen = false;
                mGameLoop.ChangeState(PrevState);
            }
        }

        public override void Draw()
        {
            mGameLoop.RenderManager.DrawVisibleElements();
        }

        public override void DrawGui(GameTime gameTime)
        {
            if (mBlackBackground is null)
            {
                LoadContent();
            }

            GameLoop.SpriteBatch.Draw(mBlackBackground, new Rectangle(0, 0, GameLoop.DisplayWidth, GameLoop.DisplayWidth), Color.White);
            GameLoop.SpriteBatch.Draw(mTutorialTexture, mTutorialPositionRectangle, Color.White);

            mInvisibleButton.Draw(GameLoop.SpriteBatch);

            // click event handler
            mComponents = new List<Component>
            {
                mInvisibleButton
            };
        }

        private void LoadContent()
        {
            mTutorialTexture = GameLoop.Content.Load<Texture2D>("Menu/Tutorial");
            mBlackBackground = GameLoop.Content.Load<Texture2D>("Menu/BlackBackground");
            mInvisibleTexture = GameLoop.Content.Load<Texture2D>("Button/Invisible");

            if (!mAreButtonsInit)
            {
                InitButtons();
                mAreButtonsInit = true;
            }
        }

        /// <summary>
        /// initializes buttons only once after loading content
        /// </summary>
        private void InitButtons()
        {
            const int imgWidth = 1525;
            const int imgHeight = 856;

            int x;
            int y;
            int width;
            int height;

            if (GameLoop.DisplayWidth / GameLoop.DisplayHeight > imgWidth / imgHeight)
            {
                height = GameLoop.DisplayHeight;
                y = 0;
                width = (int)(1f * imgWidth / imgHeight * height);
                x = (GameLoop.DisplayWidth - width) / 2;
            }
            else
            {
                width = GameLoop.DisplayWidth;
                x = 0;
                height = (int)(1f * imgHeight / imgWidth * width);
                y = (GameLoop.DisplayHeight - height) / 2;
            }
            mTutorialPositionRectangle = new Rectangle(x, y, width, height);
            mInvisibleButton = new Button(new Rectangle(0, 0, GameLoop.DisplayWidth, GameLoop.DisplayHeight), 
                mInvisibleTexture, mTutorialPositionRectangle, -1);
            mInvisibleButton.Click += InvisibleButton_Click;
        }
        private void InvisibleButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            GameLoop.ChangeState(PrevState);
        }
    }
}
