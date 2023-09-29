using System;
using System.Collections.Generic;
using ECTS.Components;
using ECTS.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.States
{
    internal sealed class SaveGameState : State
    {
        private GameLoop GameLoop { get; }
        private State PrevState { get; }
        private List<Component> mComponents;

        private Texture2D mSaveBackgroundTexture;
        private Texture2D mSaveTexture;
        private Texture2D mNotSaveTexture;
        private Texture2D mCancelTexture;

        private Button mSaveButton;
        private Button mNotSaveButton;
        private Button mCancelButton;

        public SaveGameState(GameLoop gameLoop, State prevState) : base(gameLoop)
        {
            GameLoop = gameLoop;
            PrevState = prevState;
            LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (mComponents == null)
            {
                return;
            }

            foreach (var component in mComponents)
            {
                component.Update();
            }
        }

        public override void Draw()
        {
            mGameLoop.RenderManager.DrawVisibleElements();
        }

        public override void DrawGui(GameTime gameTime)
        {
            // Draws window
            GameLoop.SpriteBatch.Draw(mSaveBackgroundTexture,
                new Rectangle(GameLoop.DisplayWidth / 4, GameLoop.DisplayHeight / 6,
                    GameLoop.DisplayWidth / 2, GameLoop.DisplayHeight / 2), Color.White);

            var largeFont =
                GameLoop.RenderManager.FontSelector.GetFont(GameLoop.DisplayWidth, FontSelector.FontSize.Large);

            GameLoop.SpriteBatch.DrawString(largeFont,
                "Would you like\nto save the game?",
                new Vector2(GameLoop.DisplayWidth * 0.38f, GameLoop.DisplayHeight * 0.31f), Color.Black);
            
            mSaveButton.Draw(GameLoop.SpriteBatch);
            mNotSaveButton.Draw(GameLoop.SpriteBatch);
            mCancelButton.Draw(GameLoop.SpriteBatch);
            
            mComponents = new List<Component>
            {
                mSaveButton,
                mNotSaveButton,
                mCancelButton
            };
        }

        private void LoadContent()
        {
            mSaveBackgroundTexture = GameLoop.Content.Load<Texture2D>("Menu/GameFinished");
            mSaveTexture = GameLoop.Content.Load<Texture2D>("Button/Yes");
            mNotSaveTexture = GameLoop.Content.Load<Texture2D>("Button/No");
            mCancelTexture = GameLoop.Content.Load<Texture2D>("Button/Cancel");
            InitButtons();
        }

        /// <summary>
        /// initializes buttons only once after loading content
        /// </summary>
        private void InitButtons()
        {
            mSaveButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.31),
                (int)(GameLoop.DisplayHeight * 0.5)), mSaveTexture, 18);
            mCancelButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.45),
                (int)(GameLoop.DisplayHeight * 0.5)), mCancelTexture, 18);
            mNotSaveButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.58),
                (int)(GameLoop.DisplayHeight * 0.5)), mNotSaveTexture, 18);

            // call button click if dropdown menu is open
            mSaveButton.Click += SaveButton_Click;
            mNotSaveButton.Click += NotSaveButton_Click;
            mCancelButton.Click += CancelButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!GameLoop.mTechDemo)
            {
                mGameLoop.RenderManager.SoundManager.PlaySound("button");
                GameLoop.Serialization.SaveGame();
                GameLoop.ChangeState(new MenuState(GameLoop));
            }
            else
            {
                mGameLoop.RenderManager.SoundManager.PlaySound("error");
            }
        }
        private void NotSaveButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            GameLoop.ChangeState(new MenuState(GameLoop));
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            GameLoop.ChangeState(PrevState);
        }
    }
}
