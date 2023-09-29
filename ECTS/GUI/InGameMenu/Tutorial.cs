using System;
using System.Collections.Generic;
using ECTS.Components;
using ECTS.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.GUI.InGameMenu
{
    /// <summary>
    /// Renders Tutorial Button and changes state to TutorialState
    /// </summary>
    internal sealed class Tutorial
    {
        private GameLoop GameLoop { get; }
        private List<Component> mComponents;
        private int mTutorialButtonSize;
        private Texture2D mQuestionMarkTexture;
        private Button mTutorialButton;
        public bool mIsTutorialOpen;

        public Tutorial(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            mIsTutorialOpen = false;
        }

        public void Update()
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

        /// <summary>
        /// Loads Content and initializes buttons
        /// </summary>
        public void LoadContent()
        {
            mQuestionMarkTexture = GameLoop.Content.Load<Texture2D>("Button/TutorialButton");

            InitButtons();
        }

        /// <summary>
        /// initializes buttons only once after loading content
        /// </summary>
        private void InitButtons()
        {
            mTutorialButtonSize = (int)(GameLoop.DisplayWidth * 0.03);

            mTutorialButton = new Button(new Rectangle((int)(GameLoop.DisplayWidth - 2.5 * mTutorialButtonSize), 0, mTutorialButtonSize,
                mTutorialButtonSize), mQuestionMarkTexture, new Rectangle(0, 0, 184, 184), -1);

            // call button click if dropdown menu is open
            mTutorialButton.Click += TutorialButton_Click;
        }

        public void Render()
        {
            mTutorialButton.Draw(GameLoop.SpriteBatch);

            mComponents = new List<Component>
            {
                mTutorialButton
            };

            // Draws button
            mTutorialButton.Draw(GameLoop.SpriteBatch);
        }

        private void TutorialButton_Click(object sender, EventArgs e)
        {
            if (!mIsTutorialOpen)
            {
                mIsTutorialOpen = true;
                GameLoop.ChangeState(new TutorialState(GameLoop, GameLoop.mCurrentState));
            }
        }
    }
}
