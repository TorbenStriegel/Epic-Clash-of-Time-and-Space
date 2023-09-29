using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Components;
using ECTS.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.States
{
    /// <summary>
    /// Shows the statistics in the game
    /// </summary>
    [DataContract]
    internal sealed class StatisticState : State
    {
        private readonly List<Component> mComponents;
        private readonly Texture2D mBackground;
        private readonly State mPreviousState;
        private GameLoop Game { get; }

        public StatisticState(GameLoop game, State prevState) : base(game)
        {
            Game = game;
            mPreviousState = prevState;
            var content = mGameLoop.Content;
            mBackground = content.Load<Texture2D>("Background");
            var buttonTextureBack = content.Load<Texture2D>("Components/BackButton");
            var buttonTextureStatistics = content.Load<Texture2D>("Components/StartMenu/Statistics");

            // Creates the buttons to be displayed in the menu.
            var backButton = new Button(buttonTextureBack, 40, (int)(mGameLoop.DisplayHeight * 0.92), false);
            backButton.Click += BackButton_Click;
            var statisticsButton = new Button(new Vector2((float)(game.DisplayWidth * 0.33), (float)(game.DisplayHeight * 0.01)),
                buttonTextureStatistics, 75, true);

            mComponents = new List<Component>
            {
                backButton,
                statisticsButton
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

            foreach (var component in mComponents) // Draw all created components.
            {
                component.Draw(mGameLoop.SpriteBatch);
            }

            // chooses font
            var font = Game.RenderManager.FontSelector.GetFont(Game.DisplayWidth, FontSelector.FontSize.Medium);

            // draws statistics
            if (mPreviousState is GameFinishedState || mPreviousState is GameStateDay || mPreviousState is GameStateNight)
            {
                Game.SpriteBatch.DrawString(font, "Survived Time: " + Game.ObjectManager.DataManager.TotalGameTime.ToString(@"hh\:mm\:ss"),
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.20)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Survived Days: " + Game.ObjectManager.DataManager.mSurvivedDays,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.30)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Wood: " + (int)Game.ObjectManager.DataManager.mCollectedWood,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.40)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Stone: " + (int)Game.ObjectManager.DataManager.mCollectedStone,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.50)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Metal: " + (int)Game.ObjectManager.DataManager.mCollectedMetal,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.60)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Food: " + (int)Game.ObjectManager.DataManager.mCollectedFood,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.70)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected DNA: " + Game.ObjectManager.DataManager.mCollectedDna,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.80)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Build Units: " + Game.ObjectManager.DataManager.mBuildUnits,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.20)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Killed Enemies: " + Game.ObjectManager.DataManager.mKilledEnemies,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.30)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Died Units: " + Game.ObjectManager.DataManager.mDiedUnits,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.40)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Spaceship Repaired: " + Math.Floor(Game.ObjectManager.DataManager.mSpaceshipRepairing * 100) / 100 + "%",
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.50)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Added Spaceship Health: " + (int)Game.ObjectManager.DataManager.mSpaceshipRepaired + " Health",
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.60)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Fastest Win: " + Game.Settings.mFastestWin.ToString(@"hh\:mm\:ss") +
                                                  " by " + Game.Settings.mFastWin,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.70)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Longest Game: " + Game.Settings.mLongestGame.ToString(@"hh\:mm\:ss") +
                                                  " (" + Game.Settings.mMostDays + " Days) \n                        " +
                                                  "by " + Game.Settings.mLongGame,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.80)),
                    Color.White);
            }
            else
            {
                Game.SpriteBatch.DrawString(font, "Time Spend In Game: " + Game.Settings.mTotalGameTime.ToString(@"hh\:mm\:ss"),
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.20)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Build Units: " + Game.Settings.mBuildUnits,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.30)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Wood: " + (int)Game.Settings.mCollectedWood,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.40)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Stone: " + (int)Game.Settings.mCollectedStone,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.50)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Metal: " + (int)Game.Settings.mCollectedMetal,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.60)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected Food: " + (int)Game.Settings.mCollectedFood,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.70)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Collected DNA: " + Game.Settings.mCollectedDna,
                    new Vector2((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.80)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Added Spaceship Health: " + (int)Game.Settings.mSpaceshipRepaired + " Health",
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.20)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Killed Enemies: " + Game.Settings.mKilledEnemies,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.30)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Died Units: " + Game.Settings.mDiedUnits,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.40)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Games Won: " + Game.Settings.mGamesWon,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.50)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Games Lost: " + Game.Settings.mGamesLost,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.60)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Fastest Win: " + Game.Settings.mFastestWin.ToString(@"hh\:mm\:ss") +
                    " by " + Game.Settings.mFastWin,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.70)),
                    Color.White);

                Game.SpriteBatch.DrawString(font, "Longest Game: " + Game.Settings.mLongestGame.ToString(@"hh\:mm\:ss") +
                    " (" + Game.Settings.mMostDays + " Days) \n                        " +
                    "by " + Game.Settings.mLongGame,
                    new Vector2((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.80)),
                    Color.White);
            }
        }

        private void BackButton_Click(object sender, EventArgs e) // EventHandler for the BackButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(mPreviousState);
        }
    }
}
