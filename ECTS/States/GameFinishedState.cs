using ECTS.Components;
using ECTS.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ECTS.States
{
    /// <summary>
    /// game is finished, window opens
    /// </summary>
    internal sealed class GameFinishedState : State
    {
        private readonly Vector2 mFinalPoint;
        private Vector2 mDirection;
        private float mDistanceCameraSpaceship;
        private float mLastDistanceCameraSpaceship;
        private readonly GameFinishedSituation mIsGameFinished;
        private GameLoop GameLoop { get; }
        private Texture2D mBackgroundTexture;
        private List<Component> mComponents;
        private bool mAreButtonsInit;
        private Texture2D mBackTexture;
        private Texture2D mStatisticsTexture;
        private Texture2D mAchievementsTexture;
        private Button mStatisticsButton;
        private Button mAchievementsButton;
        private Button mBackButton;

        /// <summary>
        /// game situations
        /// </summary>
        private enum GameFinishedSituation
        {
            NotFinished,
            Won,
            Lost
        }

        /// <summary>
        /// if game is finished -> camera moves to base
        /// </summary>
        /// <param name="gameLoop"></param>
        public GameFinishedState(GameLoop gameLoop) : base(gameLoop)
        {
            mFinalPoint = mGameLoop.ObjectManager.DataManager.Spaceship.Position.Center.ToVector2();
            mFinalPoint.X -= mGameLoop.DisplayWidth / 2f;
            mFinalPoint.Y -= mGameLoop.DisplayHeight / 2f;
            mFinalPoint.X *= -1;
            mFinalPoint.Y *= -1;
            mDistanceCameraSpaceship = float.MaxValue;
            UpdateDirection();

            GameLoop = gameLoop;
            mAreButtonsInit = false;

            if (gameLoop.ObjectManager.DataManager.Spaceship.Health >= 100)
            {
                mGameLoop.RenderManager.SoundManager.PlaySound("win");
                mIsGameFinished = GameFinishedSituation.Won;
                mGameLoop.RenderManager.SoundManager.PlayMusic("Finale");
            }
            else if (mGameLoop.ObjectManager.DataManager.Spaceship.Health <= 0)
            {
                mGameLoop.RenderManager.SoundManager.PlaySound("lose");
                mIsGameFinished = GameFinishedSituation.Lost;
            }
            else
            {
                mIsGameFinished = GameFinishedSituation.NotFinished;
            }
        }

        /// <summary>
        /// updates buttons
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

            mGameLoop.SaveSettings();
            mGameLoop.Settings.UpdateAchievements();

            // move camera to spaceship
            if (!mGameLoop.RenderManager.CameraManager.mPosition.Equals(mFinalPoint) && mDistanceCameraSpaceship < mLastDistanceCameraSpaceship && mDistanceCameraSpaceship > 0)
            {
                mGameLoop.RenderManager.CameraManager.SetZoom(1);
                mGameLoop.RenderManager.CameraManager.SetCameraPosition(
                    (int)(420 * mDirection.X * gameTime.ElapsedGameTime.TotalSeconds),
                    (int)(420 * mDirection.Y * gameTime.ElapsedGameTime.TotalSeconds));
                UpdateDirection();
            }
            else
            {
                mGameLoop.RenderManager.CameraManager.mPosition.X = mFinalPoint.X;
                mGameLoop.RenderManager.CameraManager.mPosition.Y = mFinalPoint.Y;
                mDistanceCameraSpaceship = 0;
            }
        }

        private void UpdateDirection()
        {
            mDirection = Vector2.Subtract(mFinalPoint, mGameLoop.RenderManager.CameraManager.mPosition);
            mLastDistanceCameraSpaceship = mDistanceCameraSpaceship;
            mDistanceCameraSpaceship = mDirection.LengthSquared();
            mDirection.Normalize();
        }

        public override void Draw()
        {
            mGameLoop.RenderManager.DrawVisibleElements();
        }

        public override void DrawGui(GameTime gameTime)
        {
            LoadContent();
            if (mDistanceCameraSpaceship < 0.5)
            {
                GameLoop.SpriteBatch.Draw(mBackgroundTexture,
                    new Rectangle(GameLoop.DisplayWidth / 4, GameLoop.DisplayHeight / 4,
                        GameLoop.DisplayWidth / 2, GameLoop.DisplayHeight / 2), Color.White);

                var mediumFont =
                    GameLoop.RenderManager.FontSelector.GetFont(GameLoop.DisplayWidth, FontSelector.FontSize.Medium);

                if (mIsGameFinished == GameFinishedSituation.Won)
                {
                    GameLoop.SpriteBatch.DrawString(mediumFont,
                        "Congratulations " + GameLoop.ObjectManager.DataManager.mPlayerName +
                        ",\nyou have managed to repair the\nspaceship. Your crew is now safely\non its way home.",
                        new Vector2(GameLoop.DisplayWidth * 0.33f, GameLoop.DisplayHeight * 0.35f), Color.Black);
                }
                else
                {
                    GameLoop.SpriteBatch.DrawString(mediumFont,
                        "Game Over,\nyour spaceship was completely\ndestroyed and all hope is lost.",
                        new Vector2(GameLoop.DisplayWidth * 0.33f, GameLoop.DisplayHeight * 0.35f), Color.Black);
                }

                // draw buttons
                mBackButton.Draw(GameLoop.SpriteBatch);
                mStatisticsButton.Draw(GameLoop.SpriteBatch);
                mAchievementsButton.Draw(GameLoop.SpriteBatch);

            }
            // click event handler
            mComponents = new List<Component>
            {
                mBackButton,
                mStatisticsButton,
                mAchievementsButton
            };
        }

        private void LoadContent()
        {
            if (mBackgroundTexture == null)
            {
                mBackgroundTexture = GameLoop.Content.Load<Texture2D>("Menu/GameFinished");
            }
            if (mBackTexture == null)
            {
                mBackTexture = GameLoop.Content.Load<Texture2D>("Components/BackButton");
            }
            if (mStatisticsTexture == null)
            {
                mStatisticsTexture = GameLoop.Content.Load<Texture2D>("Components/StartMenu/Statistics");
            }
            if (mAchievementsTexture == null)
            {
                mAchievementsTexture = GameLoop.Content.Load<Texture2D>("Components/StartMenu/Achievements");
            }

            if (!mAreButtonsInit)
            {
                InitButtons();
                mAreButtonsInit = true;
            }
        }

        private void InitButtons()
        {
            mBackButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.33),
                (int)(GameLoop.DisplayHeight * 0.6)), mBackTexture, 18);
            mBackButton.Click += BackButton_Click;
            mStatisticsButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.43),
                (int)(GameLoop.DisplayHeight * 0.6)), mStatisticsTexture, 18);
            mStatisticsButton.Click += StatisticsButton_Click;
            mAchievementsButton = new Button(new Vector2((int)(GameLoop.DisplayWidth * 0.55),
                (int)(GameLoop.DisplayHeight * 0.6)), mAchievementsTexture, 18);
            mAchievementsButton.Click += AchievementsButton_Click;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            GameLoop.ChangeState(new MenuState(GameLoop));
        }

        private void StatisticsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            GameLoop.ChangeState(new StatisticState(GameLoop, this));
        }

        private void AchievementsButton_Click(object sender, EventArgs e)
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            GameLoop.ChangeState(new AchievementState(GameLoop, this));
        }
    }
}
