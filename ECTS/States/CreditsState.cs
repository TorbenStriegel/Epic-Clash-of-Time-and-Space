using System;
using System.Collections.Generic;
using ECTS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.States
{
    internal sealed class CreditsState : State
    {
        private readonly List<Component> mComponents;
        private readonly Texture2D mBackground;
        private readonly State mPreviousState;

        public CreditsState(GameLoop game, State lastState) : base(game)
        {
            mBackground = mGameLoop.Content.Load<Texture2D>("Components/Credits/Background");
            mPreviousState = lastState;
            var backButtonTexture = mGameLoop.Content.Load<Texture2D>("Components/BackButton");
            var backButton = new Button(backButtonTexture, 40, (int)(mGameLoop.DisplayHeight * 0.92) , false);
            backButton.Click += BackButton_Click;
            mComponents = new List<Component>
            {
                backButton,
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in mComponents)
            {
                component.Update();
            }
        }

        public override void Draw()
        {
        }

        public override void DrawGui(GameTime gameTime)
        {
            mGameLoop.SpriteBatch.Draw(mBackground, new Rectangle(0, 0, mGameLoop.DisplayWidth, mGameLoop.DisplayHeight), Color.LightGray);
            foreach (var component in mComponents)
            {
                component.Draw(mGameLoop.SpriteBatch);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(mPreviousState);
        }
    }
}
