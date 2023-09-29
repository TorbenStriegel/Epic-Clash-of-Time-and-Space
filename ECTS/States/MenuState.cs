using System;
using System.Collections.Generic;
using ECTS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ECTS.States
{
    internal sealed class MenuState : State
    {
        private readonly List<Component> mComponents;
        private readonly Texture2D mBackground;

        public MenuState(GameLoop gameLoop)
            : base(gameLoop)
        {
            var content = mGameLoop.Content;
            // Loads all required graphics
            var buttonTextureNewGame = content.Load<Texture2D>("Components/StartMenu/NewGame");
            var buttonTextureContinueGame = content.Load<Texture2D>("Components/StartMenu/ContinueGame");
            var buttonTextureOptions = content.Load<Texture2D>("Components/StartMenu/Options");
            var buttonTextureStatistics = content.Load<Texture2D>("Components/StartMenu/Statistics");
            var buttonTextureAchievements = content.Load<Texture2D>("Components/StartMenu/Achievements");
            var buttonTextureAboutEcts = content.Load<Texture2D>("Components/StartMenu/AboutECTS");
            var buttonTextureQuitGame = content.Load<Texture2D>("Components/StartMenu/QuitGame");
            var buttonTextureCredits = content.Load<Texture2D>("Components/StartMenu/Credits");
            mBackground = content.Load<Texture2D>("StartMenu/Start_Background");
            mGameLoop.RenderManager.SoundManager.PlayMusic("Menu");
            // Creates the buttons to be displayed in the menu.
            var newGameButton = new Button(buttonTextureNewGame, 40, (int) (mGameLoop.DisplayHeight / 6.0), true);
            newGameButton.Click += NewGameButton_Click;
            var continueGameButton = new Button(buttonTextureContinueGame,
                40,
                (int) (newGameButton.Position.Y + newGameButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            continueGameButton.Click += ContinueGameButton_Click;
            var optionsButton = new Button(buttonTextureOptions,
                40,
                (int) (continueGameButton.Position.Y + continueGameButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            optionsButton.Click += OptionsButton_Click;
            if (!Serialization.ExistsSaveGame())
            {
                continueGameButton.mDeactivated = true;
            }
            var statisticsButton = new Button(buttonTextureStatistics,
                40,
                (int) (optionsButton.Position.Y + optionsButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            statisticsButton.Click += StatisticsButton_Click;
            var achievementsButton = new Button(buttonTextureAchievements,
                40,
                (int)(statisticsButton.Position.Y + statisticsButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            achievementsButton.Click += AchievementsButton_Click;
            var aboutEctsButton = new Button(buttonTextureAboutEcts,
                40,
                (int) (achievementsButton.Position.Y + achievementsButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            aboutEctsButton.Click += AboutEctsButton_Click;
            var creditsButton = new Button(buttonTextureCredits,
                40,
                (int)(aboutEctsButton.Position.Y + aboutEctsButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            creditsButton.Click += CreditsButton_Click;
            var quitGameButton = new Button(buttonTextureQuitGame,
                40,
                (int)(creditsButton.Position.Y + creditsButton.mButtonHeight + mGameLoop.DisplayHeight / 50.0),
                true);
            quitGameButton.Click += QuitGameButton_Click;
            mComponents = new List<Component> // Saves all components in a list
            {
                newGameButton,
                continueGameButton,
                optionsButton,
                statisticsButton,
                achievementsButton,
                aboutEctsButton,
                quitGameButton,
                creditsButton,
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
            mGameLoop.SpriteBatch.Draw(mBackground, new Rectangle(0, 0, mGameLoop.DisplayWidth, mGameLoop.DisplayHeight), Color.White);
            foreach (var component in mComponents) // Draw all created components.
            {
                component.Draw(mGameLoop.SpriteBatch);
            }
        }

        // Functions that are executed when a button in the menu is pressed.
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(new NewGameState(mGameLoop, this));
        }

        private void ContinueGameButton_Click(object sender, EventArgs e)
        {
            if (!Serialization.ExistsSaveGame())
            {
                return;
            }

            mGameLoop.mTechDemo = false;

            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.NewGame();
            mGameLoop.Serialization.LoadGame();
            if (mGameLoop.ObjectManager.DataManager.mDay)
            {
                mGameLoop.ChangeState(new GameStateDay(mGameLoop));
            }
            else
            {
                mGameLoop.ChangeState(new GameStateNight(mGameLoop));
            }

            mGameLoop.AiManager.SaveDifficulty();
        }
        private void OptionsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(new SettingsState(mGameLoop, this));
        }
        private void StatisticsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(new StatisticState(mGameLoop, this));
        }
        private void AchievementsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(new AchievementState(mGameLoop, this));
        }
        private void AboutEctsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(new AboutEctsState(mGameLoop, this));
        }
        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.Settings.UpdateAchievements(); 
            mGameLoop.SaveSettings();
            mGameLoop.Exit();
        }
        private void CreditsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(new CreditsState(mGameLoop, this));
        }

    }
}
