using ECTS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Data;
using Microsoft.Xna.Framework.Input;

namespace ECTS.States
{
    internal sealed class NewGameState : State
    {
        // private GraphicsDevice mGraphicsDevice;
        private readonly List<Button> mComponents;
        private readonly Texture2D mBackground;
        private readonly State mPreviousState;

        [DataMember] public string mText = ""; //Start with no text
        private DataManager.GameDifficulty mDifficulty;
        private string mNewText;
        private int mCursorPos;
        private bool mWriteName = true;

        private float mTempGameTime;

        //These are set in the constructor:
        private readonly SpriteFont mFont;
        public NewGameState(GameLoop game, State prevState) : base(game)
        {
            mTempGameTime = 0;
            mPreviousState = prevState;
            mFont = Global.mGameLoop.Content.Load<SpriteFont>("Font/Arial30");
            var content = mGameLoop.Content;
            // Loads all required graphics
            mBackground = content.Load<Texture2D>("Background");
            var buttonTextureEasy = content.Load<Texture2D>("Components/NewGame/Easy");
            var buttonTextureMedium = content.Load<Texture2D>("Components/NewGame/Medium");
            var buttonTextureHard = content.Load<Texture2D>("Components/NewGame/Hard");
            var buttonTextureDifficulty = content.Load<Texture2D>("Components/NewGame/Difficulty");
            var buttonTextureNickname = content.Load<Texture2D>("Components/NewGame/Nickname");
            var buttonTextureStartNewGame = content.Load<Texture2D>("Components/NewGame/StartNewGame");
            var buttonTextureBack = content.Load<Texture2D>("Components/BackButton");
            var buttonTextureTechdemo = content.Load<Texture2D>("Components/NewGame/Techdemo");

            // Creates the buttons to be displayed in the menu.
            var difficultyEasyButton = new Button(new Vector2((float)(mGameLoop.DisplayWidth/4.0), (int)(mGameLoop.DisplayHeight * 0.28)), buttonTextureEasy, 50);
            difficultyEasyButton.Click += DifficultyEasyButton_Click; // Add the corresponding EventHandler.
            var difficultyMediumButton = new Button(new Vector2(difficultyEasyButton.Position.X + difficultyEasyButton.mButtonWidth + 60, (int)(mGameLoop.DisplayHeight * 0.28)), buttonTextureMedium, 50);
            difficultyMediumButton.Click += DifficultyMediumButton_Click; // Add the corresponding EventHandler.
            var difficultyHardButton = new Button(new Vector2(difficultyMediumButton.Position.X + difficultyMediumButton.mButtonWidth + 60, (int)(mGameLoop.DisplayHeight * 0.28)), buttonTextureHard, 50);
            difficultyHardButton.Click += DifficultyHardButton_Click; // Add the corresponding EventHandler.
            var nameButton = new Button(new Vector2(difficultyEasyButton.Position.X, (int)(mGameLoop.DisplayHeight * 0.486)), 10, 3);
            nameButton.Click += NameButton_Click;

            var techdemoButton = new Button(new Vector2(difficultyHardButton.Position.X + difficultyHardButton.mButtonWidth + 60, (int)(mGameLoop.DisplayHeight * 0.28)), buttonTextureTechdemo, 50);
            techdemoButton.Click += TechdemoButton_Click;

            var difficultyTextfield = new Button(new Vector2(difficultyEasyButton.Position.X, (int)(difficultyEasyButton.Position.Y-mGameLoop.DisplayHeight*0.07)), buttonTextureDifficulty, 50, true);
            var nameTextfield = new Button(new Vector2(difficultyEasyButton.Position.X, (int)(nameButton.Position.Y - mGameLoop.DisplayHeight * 0.07)), buttonTextureNickname, 50, true);

            difficultyEasyButton.mDeactivated = true;
            difficultyMediumButton.mDeactivated = false;
            difficultyHardButton.mDeactivated = false;

            var startNewGameButton = new Button(buttonTextureStartNewGame, 40, (int)(mGameLoop.DisplayHeight * 0.92), true);
            startNewGameButton.Click += StartNewGameButtonButton_Click;
            var backButton = new Button(buttonTextureBack, 40, (int)(mGameLoop.DisplayHeight * 0.92), false);
            backButton.Click += BackButton_Click;

            mComponents = new List<Button> // Saves all components in a list
            {
                difficultyEasyButton,
                difficultyMediumButton,
                difficultyHardButton,
                nameButton,
                difficultyTextfield,
                nameTextfield,
                startNewGameButton,
                backButton,
                techdemoButton
            };
        }

        public override void Update(GameTime gameTime)
        {
            mTempGameTime += gameTime.ElapsedGameTime.Milliseconds;
            foreach (var component in mComponents)
            {
                component.Update();
            }
            // Manages the text input field. Gets an array with all keys pressed.
            if (Keyboard.GetState().GetPressedKeys().Length > 0 && mWriteName && mComponents[3].mDeactivated)
            {
                mWriteName = false;
                if (Keyboard.GetState().GetPressedKeys()[0] == Keys.Back)
                {
                    mCursorPos--;
                    if (mCursorPos < 0)
                    {
                        mCursorPos = 0;
                    }
                    else
                    {
                        mText = mText.Remove(mCursorPos);
                    }
                }
                else if (Keyboard.GetState().GetPressedKeys()[0].ToString().Length == 1 || Keyboard.GetState().GetPressedKeys()[0] == Keys.Space)
                {
                    CharEntered((char)Keyboard.GetState().GetPressedKeys()[0]);
                }
                mComponents[3].Text = mText;
            }
            else if (mTempGameTime > 150)
            {
                mTempGameTime = 0;
                mWriteName = true;
            }
        }

        public override void Draw()
        {

        }

        public override void DrawGui(GameTime gameTime)
        {
            mGameLoop.SpriteBatch.Draw(mBackground, new Rectangle(0, 0, mGameLoop.DisplayWidth, mGameLoop.DisplayHeight), Color.White);
            foreach (var component in mComponents) // Draw all created components.
            {
                component.Draw(mGameLoop.SpriteBatch);
            }
        }
        // Event handlers for the respective buttons
        private void DifficultyEasyButton_Click(object sender, EventArgs e) // EventHandler for the StartButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mComponents[0].mDeactivated = true;
            mComponents[1].mDeactivated = false;
            mComponents[2].mDeactivated = false;
            mComponents[3].mDeactivated = false;
            mComponents[8].mDeactivated = false;
            mDifficulty = DataManager.GameDifficulty.Easy;
        }
        private void DifficultyMediumButton_Click(object sender, EventArgs e) // EventHandler for the StartButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mComponents[0].mDeactivated = false;
            mComponents[1].mDeactivated = true;
            mComponents[2].mDeactivated = false;
            mComponents[3].mDeactivated = false;
            mComponents[8].mDeactivated = false;
            mDifficulty = DataManager.GameDifficulty.Medium;

        }
        private void DifficultyHardButton_Click(object sender, EventArgs e) // EventHandler for the StartButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mComponents[0].mDeactivated = false;
            mComponents[1].mDeactivated = false;
            mComponents[2].mDeactivated = true;
            mComponents[3].mDeactivated = false;
            mComponents[8].mDeactivated = false;
            mDifficulty = DataManager.GameDifficulty.Hard;

        }
        private void TechdemoButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mComponents[0].mDeactivated = false;
            mComponents[1].mDeactivated = false;
            mComponents[2].mDeactivated = false;
            mComponents[3].mDeactivated = false;
            mComponents[8].mDeactivated = true;

        }
        private void NameButton_Click(object sender, EventArgs e) // EventHandler for the StartButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mComponents[3].mDeactivated = true;
        }

        private void StartNewGameButtonButton_Click(object sender, EventArgs e) // EventHandler for the StartButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.mTechDemo = mComponents[8].mDeactivated;
            mGameLoop.AiManager.SaveDifficulty();       // sets difficulty in ai manager
            Global.mPause = false;
            mGameLoop.NewGame();
            mGameLoop.ObjectManager.DataManager.Difficulty = mDifficulty;
            mGameLoop.ObjectManager.DataManager.mPlayerName = mText;
            mGameLoop.ChangeState(new GameStateDay(mGameLoop));
        }
        private void BackButton_Click(object sender, EventArgs e) // EventHandler for the BackButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(mPreviousState);
        }
        private void CharEntered(char c) // Manages the text input field
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mTempGameTime = 0;
            mNewText = mText.Insert(mCursorPos, c.ToString()); //Insert the char
            //Check if the text width is shorter than the back rectangle
            if (mFont.MeasureString(mNewText).X < mComponents[3].mButtonWidth)
            {
                mText = mNewText; //Set the text
                mCursorPos++; //Move the text cursor
            }
        }
    }
}
