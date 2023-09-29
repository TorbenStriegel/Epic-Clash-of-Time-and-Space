using System;
using System.Collections.Generic;
using ECTS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.States
{
    internal sealed class AboutEctsState : State
    {
        private readonly List<Component> mComponents;
        private readonly Texture2D mBackground;
        private readonly State mPreviousState;
        public AboutEctsState(GameLoop game, State prevState) : base(game)
        {
            var content = mGameLoop.Content;
            mPreviousState = prevState;
            mBackground = content.Load<Texture2D>("Background");
            var buttonTextureAboutEcts = content.Load<Texture2D>("Components/AboutECTS"); // Loads the text for the menu
            var buttonTextureBack = content.Load<Texture2D>("Components/BackButton");

            // Creates the buttons to be displayed in the menu.
            var aboutEctsButton = new Button(new Vector2(30, 40), buttonTextureAboutEcts, 60, true);
            var backButton = new Button(buttonTextureBack, 40, (int)(mGameLoop.DisplayHeight * 0.92), false);
            backButton.Click += BackButton_Click; // Adds the appropriate EventHandler to the button

            mComponents = new List<Component> // Saves all components in a list.
            {
                aboutEctsButton,
                backButton
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in mComponents)
            {
                component.Update();
            }

            mGameLoop.Settings.mEctsMenu = mGameLoop.Settings.mEctsMenu.Add(gameTime.ElapsedGameTime);
            if (mGameLoop.Settings.mEctsMenu >= TimeSpan.FromSeconds(60))
            {
                mGameLoop.Settings.mEctsTime = true;
            }
            mGameLoop.Settings.UpdateAchievements();
        }

        public override void Draw()
        {

        }

        public override void DrawGui(GameTime gameTime)
        {
            mGameLoop.SpriteBatch.Draw(mBackground, new Rectangle(0, 0, mGameLoop.DisplayWidth, mGameLoop.DisplayHeight), Color.LightGray);
            foreach (var component in mComponents) // Draw all created components.
            {
                component.Draw(mGameLoop.SpriteBatch);
            }
        }
        private void BackButton_Click(object sender, EventArgs e) // EventHandler for the BackButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.Settings.mEctsMenu = new TimeSpan(0, 0, 0);
            mGameLoop.ChangeState(mPreviousState);
        }
    }
}
