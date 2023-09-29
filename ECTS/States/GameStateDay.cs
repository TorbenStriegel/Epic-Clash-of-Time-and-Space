using ECTS.Components;
using ECTS.GUI;
using Microsoft.Xna.Framework;
// Executes everything that is needed during the day
namespace ECTS.States
{
    public sealed class GameStateDay : State
    {
        public readonly float mDayTime;  // in seconds

        public GameStateDay(GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop.RenderManager.SoundManager.PlayMusic("Day"); // Start the appropriate music
            Global.mColor.R = 255;
            Global.mColor.G = 255;
            Global.mColor.B = 255;
            mDayTime = gameLoop.mDebugMode ? 15 : 180;
            mGameLoop.ObjectManager.DataManager.mDay = true;
        }

        public override void Update(GameTime gameTime)
        {
            mGameLoop.ObjectManager.DataManager.mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds; // Stores how long the time of day has been running.
            if (mGameLoop.IsActive || mGameLoop.mDebugMode) // Tests if windows are visible or if you are in debug mode.
            {
                mGameLoop.InputManager.Update();
                mGameLoop.ObjectManager.Update(gameTime);
                mGameLoop.RenderManager.Update(gameTime);
            }

            if (mGameLoop.ObjectManager.DataManager.mElapsedTime > mDayTime && !mGameLoop.mTechDemo) // When the time for the time of day has expired, the day change is started here.
            {
                ChangeTimeOfDay(gameTime, 70);
            }

            if (!mGameLoop.mTechDemo)
            {
                if (mGameLoop.ObjectManager.DataManager.Spaceship.Health >= 100)
                {
                    mGameLoop.Settings.mGamesWon += 1;
                    mGameLoop.ObjectManager.DataManager.FastWin();
                    mGameLoop.ObjectManager.DataManager.LongGame();
                    mGameLoop.ChangeState(new GameFinishedState(mGameLoop));
                }

                if (mGameLoop.ObjectManager.DataManager.Spaceship.Health <= 0)
                {
                    mGameLoop.Settings.mGamesLost += 1;
                    mGameLoop.ObjectManager.DataManager.LongGame();
                    mGameLoop.ChangeState(new GameFinishedState(mGameLoop));
                }
            }
        }

        public override void Draw()
        {
            mGameLoop.RenderManager.DrawVisibleElements();
        }

        public override void DrawGui(GameTime gameTime)
        {
            mGameLoop.RenderManager.DrawGui();
            if (mGameLoop.mDebugMode || mGameLoop.mTechDemo)
            {
                var timeSinceLastUpdate = 1 / gameTime.ElapsedGameTime.TotalSeconds;

                mGameLoop.SpriteBatch.DrawString(
                    mGameLoop.RenderManager.FontSelector.GetFont(mGameLoop.DisplayWidth, FontSelector.FontSize.Medium),
                    "FPS: " + timeSinceLastUpdate.ToString("F"), new Vector2(5, mGameLoop.DisplayHeight - 140),
                    Color.White);
                mGameLoop.SpriteBatch.DrawString(
                    mGameLoop.RenderManager.FontSelector.GetFont(mGameLoop.DisplayWidth, FontSelector.FontSize.Medium),
                    "#Units: " + mGameLoop.ObjectManager.DataManager.mUnits.mCount,
                    new Vector2(5, mGameLoop.DisplayHeight - 200), Color.White);
            }
        }

        private void ChangeTimeOfDay(GameTime gameTime, int targetValue) // Responsible for the slide effect when changing the time of day.
        {
            if (targetValue < Global.mColor.R)
            {
                Global.mColor.R -= (byte)(gameTime.ElapsedGameTime.TotalMilliseconds / 16);
                Global.mColor.G -= (byte)(gameTime.ElapsedGameTime.TotalMilliseconds / 16);
                Global.mColor.B -= (byte)(gameTime.ElapsedGameTime.TotalMilliseconds / 16);
                if (Global.mColor.R >= targetValue)
                {
                    return;
                }
                Global.mColor.R = (byte)targetValue;
                Global.mColor.G = (byte)targetValue;
                Global.mColor.B = (byte)targetValue;
            }
            else
            {
                mGameLoop.ChangeState(new GameStateNight(mGameLoop));
                mGameLoop.Settings.mFirstNight = true;
                mGameLoop.ObjectManager.DataManager.mElapsedTime = 0;
            }
        }
    }
}
