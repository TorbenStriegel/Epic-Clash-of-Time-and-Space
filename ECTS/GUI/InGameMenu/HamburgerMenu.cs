using ECTS.Components;
using ECTS.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ECTS.GUI.InGameMenu
{
    /// <summary>
    /// creates HamburgerMenu in the upper right corner of the game
    /// two states: mouse is NOT above: Draw() or IS above: RenderDropdownMenu()
    /// </summary>
    internal sealed class HamburgerMenu
    {
        private GameLoop GameLoop { get; }
        private Texture2D mDropdownBackgroundTexture;
        private Texture2D mHamburgerButtonTexture;
		private Texture2D mSaveGameTexture;
        private Texture2D mStatisticsTexture;
        private Texture2D mOptionsTexture;
        private Texture2D mMainMenuTexture;
        private Texture2D mPlayAndPauseTexture;

        private Button mSafeGameButton;
        private Button mStatisticsButton;
        private Button mOptionsButton;
        private Button mMainMenuButton;
        private Button mPlayAndPauseButton;
		
        private List<Component> mComponents;

        private int mHamburgerButtonSize;

        private State mPreviousState;
        private Color mPreviousColour;

        public HamburgerMenu(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }

        /// <summary>
        /// updates button in dropdown menu
        /// </summary>
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
            mDropdownBackgroundTexture = GameLoop.Content.Load<Texture2D>("Menu/Dropdown");
            mHamburgerButtonTexture = GameLoop.Content.Load<Texture2D>("Menu/HamburgerButton");

			// loads button texture
            mSaveGameTexture = GameLoop.Content.Load<Texture2D>("Button/SaveGame");
            mStatisticsTexture = GameLoop.Content.Load<Texture2D>("Button/Statistics");
            mOptionsTexture = GameLoop.Content.Load<Texture2D>("Button/Options");
            mMainMenuTexture = GameLoop.Content.Load<Texture2D>("Button/MainMenu");

            mPlayAndPauseTexture = GameLoop.Content.Load<Texture2D>("Button/PlayPause");

            InitButtons();
        }

        /// <summary>
		/// initializes buttons only once after loading content
        /// </summary>
        private void InitButtons()
        {
            mPlayAndPauseButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.74),
                (int)(GameLoop.DisplayHeight * 0.11)), mPlayAndPauseTexture, 15);
            
            mSafeGameButton = new Button(new Vector2((int) (GameLoop.DisplayWidth * 0.74), 
                (int) (GameLoop.DisplayHeight * 0.185)), mSaveGameTexture, 15);

            mStatisticsButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.74),
                (int)(GameLoop.DisplayHeight * 0.26)), mStatisticsTexture, 15);

            mOptionsButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.74),
                (int)(GameLoop.DisplayHeight * 0.335)), mOptionsTexture, 15);

            mMainMenuButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.74), 
                (int)(GameLoop.DisplayHeight * 0.41)), mMainMenuTexture, 15);

            // call button click if dropdown menu is open
            mPlayAndPauseButton.Click += PlayAndPauseButton_Click;
            mSafeGameButton.Click += SafeGameButton_Click;
            mStatisticsButton.Click += StatisticsButton_Click;
            mOptionsButton.Click += OptionsButton_Click;
            mMainMenuButton.Click += MainMenuButton_Click;
        }

        /// <summary>
        /// InGame Hamburger Menu Symbol left top corner
        /// </summary>
        public void Render()
        {
            // 4% of the DisplayWidth is the width and height of the button
            mHamburgerButtonSize = (int)(GameLoop.DisplayWidth * 0.04);
            GameLoop.SpriteBatch.Draw(mHamburgerButtonTexture,
                new Rectangle(GameLoop.DisplayWidth - mHamburgerButtonSize, 0, mHamburgerButtonSize,
                    mHamburgerButtonSize), Color.White);
        }
        
        /// <summary>
        /// Dropdown Menu is open with buttons leading into different menus
        /// </summary>
        public void RenderDropdownMenu()
        {
            //calculates size of dropdown menu
            var dropdownMenuWidth = (int)(GameLoop.DisplayWidth * 0.25);
            var dropdownMenuHeight = (int)(GameLoop.DisplayHeight * 0.6);

            // draws white background
            GameLoop.SpriteBatch.Draw(mDropdownBackgroundTexture,
                new Rectangle(GameLoop.DisplayWidth - dropdownMenuWidth - mHamburgerButtonSize, 0, dropdownMenuWidth,
                    dropdownMenuHeight), Color.White);
            
            // draw buttons
            mSafeGameButton.Draw(GameLoop.SpriteBatch);
            mStatisticsButton.Draw(GameLoop.SpriteBatch);
            mOptionsButton.Draw(GameLoop.SpriteBatch);
            mMainMenuButton.Draw(GameLoop.SpriteBatch);
            mPlayAndPauseButton.Draw(GameLoop.SpriteBatch);

            mComponents = new List<Component>
            {
                mSafeGameButton,
                mStatisticsButton,
                mOptionsButton,
                mMainMenuButton,
                mPlayAndPauseButton
            };

            // Draws buttons
            mSafeGameButton.Draw(GameLoop.SpriteBatch);
            mStatisticsButton.Draw(GameLoop.SpriteBatch);
            mOptionsButton.Draw(GameLoop.SpriteBatch);
            mMainMenuButton.Draw(GameLoop.SpriteBatch);
            mPlayAndPauseButton.Draw(GameLoop.SpriteBatch);
        }
        
        // Functions that are executed when a button in the menu is pressed.
        private void SafeGameButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.RenderManager.ScreenManager.mOpenHud && GameLoop.RenderManager.ScreenManager.mStayOpen)
            {
                if (GameLoop.mTechDemo)
                {
                    GameLoop.RenderManager.SoundManager.PlaySound("error");
                    return;
                }

                GameLoop.Serialization.SaveGame();
            }
        }

        private void StatisticsButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.RenderManager.ScreenManager.mOpenHud && GameLoop.RenderManager.ScreenManager.mStayOpen)
            {
                GameLoop.ChangeState(new StatisticState(GameLoop, GameLoop.mCurrentState));
            }
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.RenderManager.ScreenManager.mOpenHud && GameLoop.RenderManager.ScreenManager.mStayOpen)
            {
                GameLoop.ChangeState(new SettingsState(GameLoop, GameLoop.mCurrentState));
            }
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.RenderManager.ScreenManager.mOpenHud && GameLoop.RenderManager.ScreenManager.mStayOpen)
            {
                if (GameLoop.mTechDemo)
                {
                    GameLoop.ChangeState(new MenuState(GameLoop));
                }
                else
                {
                    GameLoop.ChangeState(new SaveGameState(GameLoop, GameLoop.mCurrentState));
                }
            }
        }

        private void PlayAndPauseButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.RenderManager.ScreenManager.mOpenHud && GameLoop.RenderManager.ScreenManager.mStayOpen)
            {
                GameLoop.RenderManager.SoundManager.PlaySound("clone");
                if (Global.mPause)
                {
                    Global.mPause = false;
                    Global.mColor = mPreviousColour;
                    GameLoop.ChangeState(mPreviousState);
                }
                else
                {
                    Global.mPause = true;
                    mPreviousColour = Global.mColor;
                    mPreviousState = GameLoop.mCurrentState;
                    GameLoop.ChangeState(new PauseState(GameLoop));
                }
            }
        }
    }
}
