using System;
using System.Collections.Generic;
using ECTS.Components;
using ECTS.Data;
using ECTS.Objects;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.controllable_units;
using ECTS.Objects.GameObjects.uncontrollable_units.spaceship;
using ECTS.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ECTS.Objects.GameObjects.GameObject;

namespace ECTS.GUI.InGameMenu
{
    /// <summary>
    /// creates InGameHudMenu on the bottom of the game with clone machine, clock, idle button and object information:
    /// states: nothing is marked, one object is marked, multiple units are marked
    /// </summary>
    internal sealed class HudMenu
    {
		private GameLoop GameLoop { get; }

        private readonly Rectangle mFrontRectangle = new Rectangle(32, 0, 32, 32);
        private Rectangle mWorkerIconRectangle;
        private Rectangle mEngineerIconRectangle;
        private Rectangle mFighterCloseRectangle;
        private Rectangle mFighterDistanceRectangle;
        private Rectangle mIdleRectangle;

        private Texture2D mBackgroundTexture;
        private Texture2D mWorkerIconTexture;
        private Texture2D mEngineerIconTexture;
        private Texture2D mFighterCloseIconTexture;
        private Texture2D mFighterDistanceIconTexture;
        private Texture2D mMonsterIconTexture;
        private Texture2D mHeartsTexture;
        private Texture2D mQueueCircleTexture;
        private Texture2D mIdleTexture;
        private Texture2D mSunTexture;
        private Texture2D mMoonTexture;
        private Texture2D mFrameTexture;
        private Texture2D mNightTexture;
        private Texture2D mDayTexture;

        private Button mNewWorkerButton;
        private Button mNewEngineerButton;
        private Button mNewFighterCloseButton;
        private Button mNewFighterDistanceButton;
        private Button mIdleButton;

        private SpriteFont mMediumFont;
        private SpriteFont mSmallFont;

        private List<Component> mComponents;

        private readonly Random mRand = new Random();

        /// <summary>
        /// period in seconds until new object is produced { Worker, Engineer, FighterClose, FighterDistance }
        /// </summary>
        private readonly List<int> mOriginalTimer = new List<int> { 20, 30, 45, 60};

        private readonly List<string> mObjectNames = new List<string>
        {
            "Worker",
            "Engineer",
            "Fighter Close",
            "Fighter Distance"
        };

        internal HudMenu(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }

		/// <summary>
        /// Loads Content and initializes buttons
        /// </summary>			  
        internal void LoadContent()
        {
            mBackgroundTexture = GameLoop.Content.Load<Texture2D>("Menu/blueHud");
            mWorkerIconTexture = GameLoop.Content.Load<Texture2D>("Player/worker_levels");
            mEngineerIconTexture = GameLoop.Content.Load<Texture2D>("Player/engineer_levels");
            mFighterCloseIconTexture = GameLoop.Content.Load<Texture2D>("Player/fighterclose_levels");
            mFighterDistanceIconTexture = GameLoop.Content.Load<Texture2D>("Player/fighterdistance_levels");
            mMonsterIconTexture = GameLoop.Content.Load<Texture2D>("Player/Monster");
            mHeartsTexture = GameLoop.Content.Load<Texture2D>("Player/lebensanzeige_hearts");


            mQueueCircleTexture = GameLoop.Content.Load<Texture2D>("Menu/Queue");
            mIdleTexture = GameLoop.Content.Load<Texture2D>("Menu/IdleButton");
            mSunTexture = GameLoop.Content.Load<Texture2D>("Menu/clock/Sun");
            mMoonTexture = GameLoop.Content.Load<Texture2D>("Menu/clock/Moon_with_Starts");
            mFrameTexture = GameLoop.Content.Load<Texture2D>("Menu/clock/Transparenter_Balken");
            mDayTexture = GameLoop.Content.Load<Texture2D>("Menu/clock/Tag_Balken");
            mNightTexture = GameLoop.Content.Load<Texture2D>("Menu/clock/Nacht_Balken");

            mMediumFont = GameLoop.RenderManager.FontSelector.GetFont(GameLoop.DisplayWidth, FontSelector.FontSize.Medium);
            mSmallFont = GameLoop.RenderManager.FontSelector.GetFont(GameLoop.DisplayWidth, FontSelector.FontSize.Small);

            InitButtons(); // clone machine buttons
        }

        /// <summary>
        /// initializes buttons only once after loading content for the clone machine
        /// </summary>
        private void InitButtons()
        {
            var iconSize = (int) Math.Min(GameLoop.DisplayWidth * 1/3f * 1/4f * 2/3f, GameLoop.DisplayHeight * 0.15 * 1/3);
            var spaceHorizontal = (int)(GameLoop.DisplayWidth * 1/3f * 1/4f * 1/6f);
            var spaceVerticalHudIcon = (int)((GameLoop.DisplayHeight * 0.15 - iconSize) / 2f);

            mWorkerIconRectangle = new Rectangle((int)(GameLoop.DisplayWidth * 2 / 3f + spaceHorizontal),
                (int)(GameLoop.DisplayHeight * 0.85 + spaceVerticalHudIcon), iconSize, iconSize);
            mEngineerIconRectangle = new Rectangle(
                (int)(GameLoop.DisplayWidth * 2 / 3f + 3 * spaceHorizontal + iconSize),
                (int)(GameLoop.DisplayHeight * 0.85 + spaceVerticalHudIcon), iconSize, iconSize);
            mFighterCloseRectangle = new Rectangle(
                (int)(GameLoop.DisplayWidth * 2 / 3f + 5 * spaceHorizontal + 2 * iconSize),
                (int)(GameLoop.DisplayHeight * 0.85 + spaceVerticalHudIcon), iconSize, iconSize);
            mFighterDistanceRectangle = new Rectangle(
                (int)(GameLoop.DisplayWidth * 2 / 3f + 7 * spaceHorizontal + 3 * iconSize),
                (int)(GameLoop.DisplayHeight * 0.85 + spaceVerticalHudIcon), iconSize, iconSize);
            mIdleRectangle = new Rectangle(
                (int)(GameLoop.DisplayWidth * 2 / 3f + 9 * spaceHorizontal + 4.3 * iconSize),
                (int)(GameLoop.DisplayHeight * 0.85 + (GameLoop.DisplayHeight * 0.15 - iconSize * 1.5) / 2f), (int)(iconSize * 1.5), (int)(iconSize * 1.5));

            mNewWorkerButton = new Button(mWorkerIconRectangle, mWorkerIconTexture, mFrontRectangle, 0, true);
            mNewEngineerButton = new Button(mEngineerIconRectangle, mEngineerIconTexture, mFrontRectangle, 1, true);
            mNewFighterCloseButton = new Button(mFighterCloseRectangle, mFighterCloseIconTexture, mFrontRectangle, 2, true);
            mNewFighterDistanceButton = new Button(mFighterDistanceRectangle, mFighterDistanceIconTexture, mFrontRectangle, 3, true);
            mIdleButton = new Button(mIdleRectangle, mIdleTexture, new Rectangle(0, 0, 184, 184), -1);

            mNewWorkerButton.Click += NewWorkerButton_Click;
            mNewEngineerButton.Click += NewEngineerButton_Click;
            mNewFighterCloseButton.Click += NewFighterCloseButton_Click;
            mNewFighterDistanceButton.Click += NewFighterDistanceButton_Click;
            mIdleButton.Click += IdleButton_Click;
        }

        internal void Update(GameTime gameTime)
        {
            if (!Global.mPause)
            {
                if (mComponents == null)
                {
                    return;
                }

                foreach (var component in mComponents)
                {
                    component.Update();
                }

                for (var i = 0; i < 4; i++)
                {
                    if (!GameLoop.ObjectManager.DataManager.mMachineInProgress[i] &&
                        GameLoop.ObjectManager.DataManager.mNewObjectQueue[i] > 0)
                    {
                        // no object is in progress
                        // new object from queue gets produced
                        if (i == 0)
                        {
                            GameLoop.ObjectManager.DataManager.mWorkerQueue -= 1;
                        }
                        if (i == 1)
                        {
                            GameLoop.ObjectManager.DataManager.mEngineerQueue -= 1;
                        }
                        if (i == 2)
                        {
                            GameLoop.ObjectManager.DataManager.mCloseFighterQueue -= 1;
                        }
                        if (i == 3)
                        {
                            GameLoop.ObjectManager.DataManager.mDistanceFighterQueue -= 1;
                        }
                        GameLoop.ObjectManager.DataManager.mNewObjectQueue[i] -= 1;
                        GameLoop.ObjectManager.DataManager.mRunningTimer[i] = mOriginalTimer[i];
                        GameLoop.ObjectManager.DataManager.mMachineInProgress[i] = true;
                    }

                    if (GameLoop.ObjectManager.DataManager.mRunningTimer[i] > 0)
                    {
                        GameLoop.ObjectManager.DataManager.mRunningTimer[i] -= gameTime.ElapsedGameTime.TotalSeconds;
                        if (GameLoop.ObjectManager.DataManager.mRunningTimer[i] <= 0)
                        {
                            GameLoop.ObjectManager.DataManager.mRunningTimer[i] = 0;
                            CreateNewObject(i);
                            GameLoop.ObjectManager.DataManager.mMachineInProgress[i] = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// renders a clock with left time of a day/night
        /// </summary>
        internal void RenderClock()
        {
            var heightFrame = GameLoop.DisplayHeight * (1 - (0.06 + 0.15)) - 40;
            var destinationFrame = new Rectangle(10,
                GameLoop.RenderManager.ScreenManager.ResourceMenu.mResourceMenuRectangle.Height + 20,
                80,
                (int) heightFrame);
            var progressDay = 0f;
            if (!GameLoop.mTechDemo)
            {
                if (GameLoop.mCurrentState is GameStateDay dayState)
                {
                    progressDay = GameLoop.ObjectManager.DataManager.mElapsedTime / dayState.mDayTime;
                }
                else if (GameLoop.mCurrentState is GameStateNight nightState)
                {
                    progressDay = GameLoop.ObjectManager.DataManager.mElapsedTime / nightState.mNightTime;
                }

                progressDay = progressDay > 1 ? 1 : progressDay;
            }

            if (GameLoop.mCurrentState is GameStateDay)
            {
                // Background
                GameLoop.SpriteBatch.Draw(mNightTexture, destinationFrame, Color.White); // Background

                // Old Moon
                var scaleMoon = (destinationFrame.Width - 26) / (float)mMoonTexture.Width;
                var heightMoon = mMoonTexture.Height * scaleMoon;
                var destinationMoon = new Rectangle(destinationFrame.X + 15,
                    destinationFrame.Y + 17,(int) (mMoonTexture.Width * scaleMoon),
                    (int) heightMoon);
                GameLoop.SpriteBatch.Draw(mMoonTexture, destinationMoon, Color.White);

                // Progressbar
                var sourceDay = new Rectangle(0, 0, mDayTexture.Width, (int)(mDayTexture.Height * progressDay));
                var destinationDay = new Rectangle(destinationFrame.X, destinationFrame.Y,destinationFrame.Width,
                    (int) (destinationFrame.Height * progressDay));
                GameLoop.SpriteBatch.Draw(mDayTexture, destinationDay, sourceDay, Color.White);

                // Sun
                var scaleSun = (destinationFrame.Width - 26) / (float) mSunTexture.Width;
                var heightSun = mSunTexture.Height * scaleSun;
                var ySun = destinationFrame.Y - heightSun + destinationFrame.Height * progressDay;
                ySun = ySun > destinationFrame.Y + 17 ? destinationFrame.Y + 17 : ySun;
                var progressSun = (ySun - (destinationFrame.Y - heightSun)) / (heightSun + 17);
                var sourceSun = new Rectangle(0,(int) (mSunTexture.Height * (1 - progressSun)),
                    mSunTexture.Width,(int) (mSunTexture.Height * progressSun));
                var destinationYSun = ySun < destinationFrame.Y ? destinationFrame.Y : ySun;
                var destinationSun = new Rectangle(destinationFrame.X + 15, (int) destinationYSun, 
                    (int)(mSunTexture.Width * scaleSun), (int) (heightSun * progressSun));
                GameLoop.SpriteBatch.Draw(mSunTexture, destinationSun, sourceSun, Color.White);
            }
            else if (GameLoop.mCurrentState is GameStateNight)
            {
                // Background
                GameLoop.SpriteBatch.Draw(mDayTexture, destinationFrame, Color.White); // Background

                // Old Sun
                var scaleSun = (destinationFrame.Width - 26) / (float)mSunTexture.Width;
                var heightSun = mSunTexture.Height * scaleSun;
                var destinationSun = new Rectangle(destinationFrame.X + 15, destinationFrame.Y + 17, 
                    (int)(mSunTexture.Width * scaleSun), (int)heightSun);
                GameLoop.SpriteBatch.Draw(mSunTexture, destinationSun, Color.White);

                // Progressbar
                var sourceNight = new Rectangle(0, 0, mNightTexture.Width, (int)(mNightTexture.Height * progressDay));
                var destinationNight = new Rectangle(destinationFrame.X, destinationFrame.Y, destinationFrame.Width, 
                    (int)(destinationFrame.Height * progressDay));
                GameLoop.SpriteBatch.Draw(mNightTexture, destinationNight, sourceNight, Color.White);

                // Moon
                var scaleMoon = (destinationFrame.Width - 26) / (float)(mMoonTexture.Width);
                var heightMoon = mMoonTexture.Height * scaleMoon;
                var yMoon = destinationFrame.Y - heightMoon + destinationFrame.Height * progressDay;
                yMoon = yMoon > destinationFrame.Y + 17 ? destinationFrame.Y + 17 : yMoon;
                var progressMoon = (yMoon - (destinationFrame.Y - heightMoon)) / (heightMoon + 17);
                var sourceMoon = new Rectangle(0, (int)(mMoonTexture.Height * (1 - progressMoon)), 
                    mMoonTexture.Width, (int)(mMoonTexture.Height * progressMoon));
                var destinationYMoon = yMoon < destinationFrame.Y ? destinationFrame.Y : yMoon;
                var destinationMoon = new Rectangle(destinationFrame.X + 15, (int)destinationYMoon, 
                    (int)(mMoonTexture.Width * scaleMoon), (int)(heightMoon * progressMoon));
                GameLoop.SpriteBatch.Draw(mMoonTexture, destinationMoon, sourceMoon, Color.White);
            }

            GameLoop.SpriteBatch.Draw(mFrameTexture, destinationFrame, Color.White);
        }

        /// <summary>
        /// renders background of the hud								 
        /// </summary>
        internal void RenderBackground()
        {
            var hudMenuRectangle = new Rectangle(0,
                (int) (GameLoop.DisplayHeight * 0.85),
                GameLoop.DisplayWidth, (int) (GameLoop.DisplayHeight * 0.15));
            GameLoop.SpriteBatch.Draw(mBackgroundTexture, hudMenuRectangle, Color.White);
        }

        /// <summary>
        /// renders in the right third the clone machine
        /// icons, names, counter, amount of clones in progress
        /// </summary>
        internal void RenderCloneMachine()
        {
            RenderButtons();

            RenderQueue();

            // draw timer
            var minTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[0] / 60).ToString("00");
            if (minTimer.Length < 2)
            {
                minTimer = "0" + minTimer;
            }
            var secTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[0] % 60).ToString("00");
            if (secTimer.Length < 2)
            {
                secTimer = "0" + secTimer;
            }
            GameLoop.SpriteBatch.DrawString(mSmallFont, minTimer + ":" + secTimer,
                new Vector2(mWorkerIconRectangle.X,
                    (int) (mWorkerIconRectangle.Y + mWorkerIconRectangle.Height + GameLoop.DisplayHeight * 0.01)),
                Color.Black);

            minTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[1] / 60).ToString("00");
            if (minTimer.Length < 2)
            {
                minTimer = "0" + minTimer;
            }
            secTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[1] % 60).ToString("00");
            if (secTimer.Length < 2)
            {
                secTimer = "0" + secTimer;
            }
            GameLoop.SpriteBatch.DrawString(mSmallFont, minTimer + ":" + secTimer,
                new Vector2(mEngineerIconRectangle.X,
                    (int) (mEngineerIconRectangle.Y + mEngineerIconRectangle.Height + GameLoop.DisplayHeight * 0.01)),
                Color.Black);

            minTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[2] / 60).ToString("00");
            if (minTimer.Length < 2)
            {
                minTimer = "0" + minTimer;
            }
            secTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[2] % 60).ToString("00");
            if (secTimer.Length < 2)
            {
                secTimer = "0" + secTimer;
            }
            GameLoop.SpriteBatch.DrawString(mSmallFont, minTimer + ":" + secTimer,
                new Vector2(mFighterCloseRectangle.X,
                    (int) (mFighterCloseRectangle.Y + mFighterCloseRectangle.Height + GameLoop.DisplayHeight * 0.01)),
                Color.Black);

            minTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[3] / 60).ToString("00");
            if (minTimer.Length < 2)
            {
                minTimer = "0" + minTimer;
            }
            secTimer = (GameLoop.ObjectManager.DataManager.mRunningTimer[3] % 60).ToString("00");
            if (secTimer.Length < 2)
            {
                secTimer = "0" + secTimer;
            }
            GameLoop.SpriteBatch.DrawString(mSmallFont, minTimer + ":" + secTimer,
                new Vector2(mFighterDistanceRectangle.X,
                    (int) (mFighterDistanceRectangle.Y + mFighterDistanceRectangle.Height +
                           GameLoop.DisplayHeight * 0.01)),
                Color.Black);
        }

        private void RenderButtons()
        {
            // draw buttons
            mNewWorkerButton.Draw(GameLoop.SpriteBatch);
            mNewEngineerButton.Draw(GameLoop.SpriteBatch);
            mNewFighterCloseButton.Draw(GameLoop.SpriteBatch);
            mNewFighterDistanceButton.Draw(GameLoop.SpriteBatch);
            mIdleButton.Draw(GameLoop.SpriteBatch);

            // click event handler
            mComponents = new List<Component>
            {
                mNewWorkerButton,
                mNewEngineerButton,
                mNewFighterCloseButton,
                mNewFighterDistanceButton,
                mIdleButton
            };

            // draw buttons
            mNewWorkerButton.Draw(GameLoop.SpriteBatch);
            mNewEngineerButton.Draw(GameLoop.SpriteBatch);
            mNewFighterCloseButton.Draw(GameLoop.SpriteBatch);
            mNewFighterDistanceButton.Draw(GameLoop.SpriteBatch);
            mIdleButton.Draw(GameLoop.SpriteBatch);
        }

        private void RenderQueue()
        {
            var queue = (int)(mWorkerIconRectangle.Width * 0.95);
            
            GameLoop.SpriteBatch.Draw(mQueueCircleTexture,
                new Rectangle((int)(mWorkerIconRectangle.X + mWorkerIconRectangle.Width - queue * 0.38),
                    (int)(mWorkerIconRectangle.Y - queue * 0.45), queue, queue), Color.White);
            GameLoop.SpriteBatch.DrawString(mSmallFont, GameLoop.ObjectManager.DataManager.mNewObjectQueue[0].ToString(),
                new Vector2(mWorkerIconRectangle.X + mWorkerIconRectangle.Width - queue / 5,
                    mWorkerIconRectangle.Y - queue / 4), Color.Black);

            GameLoop.SpriteBatch.Draw(mQueueCircleTexture,
                new Rectangle((int)(mEngineerIconRectangle.X + mEngineerIconRectangle.Width - queue * 0.38),
                    (int)(mEngineerIconRectangle.Y - queue * 0.45), queue, queue), Color.White);
            GameLoop.SpriteBatch.DrawString(mSmallFont, GameLoop.ObjectManager.DataManager.mNewObjectQueue[1].ToString(),
                new Vector2(mEngineerIconRectangle.X + mEngineerIconRectangle.Width - queue / 5,
                    mEngineerIconRectangle.Y - queue / 4), Color.Black);

            GameLoop.SpriteBatch.Draw(mQueueCircleTexture,
                new Rectangle((int)(mFighterCloseRectangle.X + + mFighterCloseRectangle.Width - queue * 0.38),
                    (int)(mFighterCloseRectangle.Y - queue * 0.45), queue, queue), Color.White);
            GameLoop.SpriteBatch.DrawString(mSmallFont, GameLoop.ObjectManager.DataManager.mNewObjectQueue[2].ToString(),
                new Vector2(mFighterCloseRectangle.X + mFighterCloseRectangle.Width - queue / 5,
                    mFighterCloseRectangle.Y - queue / 4), Color.Black);

            GameLoop.SpriteBatch.Draw(mQueueCircleTexture,
                new Rectangle((int)(mFighterDistanceRectangle.X + mFighterDistanceRectangle.Width - queue * 0.38),
                    (int)(mFighterDistanceRectangle.Y - queue * 0.45), queue, queue), Color.White);
            GameLoop.SpriteBatch.DrawString(mSmallFont, GameLoop.ObjectManager.DataManager.mNewObjectQueue[3].ToString(),
                new Vector2(mFighterDistanceRectangle.X + mFighterDistanceRectangle.Width - queue / 5,
                    mFighterDistanceRectangle.Y - queue / 4), Color.Black);
        }

        /// <summary>
        /// draws costs of the object when the mouse is hovering the button
        /// red costs if the player got not enough resources
        /// </summary>
        /// <param name="buttonSize">position for the costs window</param>
        /// <param name="objectType">0: Worker, 1: Engineer, 2: FighterC, 3: FighterD</param>
        internal void DrawCosts(Rectangle buttonSize, int objectType)
        {
            if (objectType > 3 || objectType < 0)
            {
                return;
            }
            
            var backgroundCostsTexture = GameLoop.Content.Load<Texture2D>("Menu/Costs");
            var backgroundRectangle = new Rectangle(
                (int) (buttonSize.X + buttonSize.Width - GameLoop.DisplayWidth * 0.15),
                (int) (buttonSize.Y - buttonSize.Height * 2 - GameLoop.DisplayHeight * 0.15),
                (int) (GameLoop.DisplayWidth * 0.15), (int) (GameLoop.DisplayWidth * 0.13));
            GameLoop.SpriteBatch.Draw(backgroundCostsTexture, backgroundRectangle, Color.White);
            GameLoop.SpriteBatch.DrawString(mSmallFont, mObjectNames[objectType] + ":",
                new Vector2((int)(backgroundRectangle.X + backgroundRectangle.Width * 0.2), 
                    (int)(backgroundRectangle.Y + backgroundRectangle.Height * 0.1)), Color.Black);

            GameLoop.SpriteBatch.DrawString(mSmallFont,
                "Metal: " + GameLoop.ObjectManager.mProductionCosts[objectType][0],
                new Vector2((int) (backgroundRectangle.X + backgroundRectangle.Width * 0.2),
                    (int) (backgroundRectangle.Y + backgroundRectangle.Height * 0.25)),
                GameLoop.ObjectManager.DataManager.GetColorFontForNeededResource(
                    GameLoop.ObjectManager.mProductionCosts[objectType][0],
                    GameLoop.ObjectManager.DataManager.ResourceList[0]));

            GameLoop.SpriteBatch.DrawString(mSmallFont,
                "Stone: " + GameLoop.ObjectManager.mProductionCosts[objectType][1],
                new Vector2((int) (backgroundRectangle.X + backgroundRectangle.Width * 0.2),
                    (int) (backgroundRectangle.Y + backgroundRectangle.Height * 0.4)),
                GameLoop.ObjectManager.DataManager.GetColorFontForNeededResource(
                    GameLoop.ObjectManager.mProductionCosts[objectType][1],
                    GameLoop.ObjectManager.DataManager.ResourceList[1]));

            GameLoop.SpriteBatch.DrawString(mSmallFont,
                "Wood: " + GameLoop.ObjectManager.mProductionCosts[objectType][2],
                new Vector2((int) (backgroundRectangle.X + backgroundRectangle.Width * 0.2),
                    (int) (backgroundRectangle.Y + backgroundRectangle.Height * 0.55)),
                GameLoop.ObjectManager.DataManager.GetColorFontForNeededResource(
                    GameLoop.ObjectManager.mProductionCosts[objectType][2],
                    GameLoop.ObjectManager.DataManager.ResourceList[2]));

            GameLoop.SpriteBatch.DrawString(mSmallFont,
                "Food: " + GameLoop.ObjectManager.mProductionCosts[objectType][3],
                new Vector2((int) (backgroundRectangle.X + backgroundRectangle.Width * 0.2),
                    (int) (backgroundRectangle.Y + backgroundRectangle.Height * 0.7)),
                GameLoop.ObjectManager.DataManager.GetColorFontForNeededResource(
                    GameLoop.ObjectManager.mProductionCosts[objectType][3],
                    GameLoop.ObjectManager.DataManager.ResourceList[3]));
        }

        /// <summary>
        /// renders in the left two thirds the icon, name, health (and progress) of the one marked object
        /// </summary>
        /// <param name="gameObject">marked game object</param>
        internal void RenderOneMarkedObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            if (!(gameObject.mObjectType is ObjectType.Enemy || gameObject.mObjectType is ObjectType.Player ||
                  gameObject.mObjectType is ObjectType.Wall || gameObject.mObjectType is ObjectType.Spaceship))
            {
                return;
            }

            // edge length of the squared icon
            var iconSizeMax = (int)Math.Min(GameLoop.DisplayWidth / 20f, GameLoop.DisplayHeight * 0.09);
            var name = gameObject.GetType().Name;
            var spaceVertical = (int)((GameLoop.DisplayHeight * 0.15 - iconSizeMax) / 2);

            // calculate icon size
            int iconWidth;
            int iconHeight;
            
            if (gameObject.mFace.Height == gameObject.mFace.Width)
            {
                iconWidth = iconSizeMax;
                iconHeight = iconSizeMax;
            }
            else if (gameObject.mFace.Height > gameObject.mFace.Width)
            {
                iconWidth = (int)(gameObject.mFace.Width / (float)gameObject.mFace.Height * iconSizeMax);
                iconHeight = (int)(gameObject.mFace.Height / (float)gameObject.mFace.Width * iconWidth);
            }
            else
            {
                iconHeight = (int)(gameObject.mFace.Height / (float)gameObject.mFace.Width * iconSizeMax);
                iconWidth = (int)(gameObject.mFace.Width / (float)gameObject.mFace.Height * iconHeight);
            }

            var iconX = (int)(GameLoop.DisplayWidth / 15f - iconWidth * 0.5);
            var iconY = (int)((GameLoop.DisplayHeight * 0.15 - iconHeight) / 2f + GameLoop.DisplayHeight * 0.85);

            GameLoop.SpriteBatch.Draw(gameObject.mSpriteTexture, new Rectangle(iconX, iconY, iconWidth, iconHeight),
                gameObject.mFace, Color.White);

            // draw name of game object
            GameLoop.SpriteBatch.DrawString(mMediumFont, name,
                new Vector2(iconSizeMax + 3 * spaceVertical, (int) (GameLoop.DisplayHeight * 0.85 + spaceVertical)), Color.Black);

            // draw health of object
            var heartsTextureRectangle = ScreenManager.GetHeartTextureRectangle((int)gameObject.Health);
            var heartsTextureWidth = GameLoop.DisplayWidth * 2 / 9 - 2 * spaceVertical;
            const float heartsTextureRatio = 16/134f; // Height / Width
            var heartsPositionRectangle = new Rectangle(iconSizeMax + 3 * spaceVertical, (int) (GameLoop.DisplayHeight * 0.925),
                heartsTextureWidth, (int)(heartsTextureRatio * heartsTextureWidth));

            GameLoop.SpriteBatch.Draw(mHeartsTexture, heartsPositionRectangle, heartsTextureRectangle, Color.White);

            if (gameObject is Spaceship1)
            {
                var iconSize = (int)Math.Min(GameLoop.DisplayWidth * 0.08, GameLoop.DisplayHeight * 0.08);
                var leftPosition = iconSizeMax + 5 * spaceVertical + heartsTextureWidth;
                var yCoordinate = (int)((GameLoop.DisplayHeight * 0.15 - iconSize) / 2f + GameLoop.DisplayHeight * 0.85);
                var spaceBetween = mWorkerIconRectangle.X - leftPosition;

                // draw image of icons
                GameLoop.SpriteBatch.Draw(GameLoop.RenderManager.ScreenManager.ResourceMenu.mWoodTexture2D, new Rectangle(
                    leftPosition, yCoordinate, iconSize, iconSize), Color.White);
                GameLoop.SpriteBatch.Draw(GameLoop.RenderManager.ScreenManager.ResourceMenu.mStoneTexture2D, new Rectangle(
                    (int)(leftPosition + 1 / 3f * spaceBetween), yCoordinate, iconSize, iconSize), Color.White);
                GameLoop.SpriteBatch.Draw(GameLoop.RenderManager.ScreenManager.ResourceMenu.mMetalTexture2D, new Rectangle(
                    (int)(leftPosition + 2 / 3f * spaceBetween), yCoordinate, iconSize, iconSize), Color.White);

                // Needed resources depending on difficulty
                var resourcesPerHealth = GameLoop.ObjectManager.DataManager.Difficulty switch
                {
                    DataManager.GameDifficulty.Easy => 40,
                    DataManager.GameDifficulty.Medium => 60,
                    DataManager.GameDifficulty.Hard => 100,
                    _ => 100
                };

                var neededResources = (int)((100 - GameLoop.ObjectManager.DataManager.Spaceship.Health) * resourcesPerHealth);

                // needed resources for spaceship
                GameLoop.SpriteBatch.DrawString(mMediumFont, neededResources.ToString(),
                    new Vector2(leftPosition + 1.2f * iconSize, yCoordinate + iconSize * 0.2f), Color.Black);
                GameLoop.SpriteBatch.DrawString(mMediumFont, neededResources.ToString(),
                    new Vector2(leftPosition + 1 / 3f * spaceBetween + 1.2f * iconSize, 
                        yCoordinate + iconSize * 0.2f), Color.Black);
                GameLoop.SpriteBatch.DrawString(mMediumFont, neededResources.ToString(),
                    new Vector2(leftPosition + 2 / 3f * spaceBetween + 1.2f * iconSize, 
                        yCoordinate + iconSize * 0.2f), Color.Black);
            }
        }

        /// <summary>
        /// renders in the left two thirds the icon, name, and amount of objects of multiple marked object
        /// </summary>
        /// <param name="gameObjects"></param>
        internal void RenderMultipleMarkedPlayers(List<GameObject> gameObjects)
        {
			if (gameObjects == null)
            {
                return;
            }
            
            // space between two icons (icon and left display edge)
            var space = GameLoop.DisplayWidth / 12;

            // edge length of the squared icon
            var iconSize = (int)Math.Min(GameLoop.DisplayWidth * 0.12, GameLoop.DisplayHeight * 0.1);

            var workerIconRectangle = new Rectangle(space,
                (int) ((GameLoop.DisplayHeight * 0.15 - iconSize) / 2 + GameLoop.DisplayHeight * 0.85), iconSize,
                iconSize);
				
            var engineerIconRectangle = new Rectangle(2 * space + iconSize,
                (int)((GameLoop.DisplayHeight * 0.15 - iconSize) / 2 + GameLoop.DisplayHeight * 0.85), iconSize,
                iconSize);
			
            var fighterCIconRectangle = new Rectangle(3 * space + 2 * iconSize,
                (int)((GameLoop.DisplayHeight * 0.15 - iconSize) / 2 + GameLoop.DisplayHeight * 0.85), iconSize,
                iconSize);

            var fighterDIconRectangle = new Rectangle(4 * space + 3 * iconSize,
                (int)((GameLoop.DisplayHeight * 0.15 - iconSize) / 2 + GameLoop.DisplayHeight * 0.85), iconSize,
                iconSize);

            var workerCounter = 0;
            var engineerCounter = 0;
            var fighterCloseCounter = 0;
            var fighterDistanceCounter = 0;

            // counts the amount of marked objects type worker, engineer, fighter
            foreach (var gameObject in gameObjects)
            {
                if (gameObject is Worker)
                {
                    workerCounter += 1;
                }
                if (gameObject is Engineer)
                {
                    engineerCounter += 1;
                }
                if (gameObject is FighterClose)
                {
                    fighterCloseCounter += 1;
                }
                if (gameObject is FighterDistance)
                {
                    fighterDistanceCounter += 1;
                }
            }
            
            // draws icons
            GameLoop.SpriteBatch.Draw(mWorkerIconTexture, workerIconRectangle, mFrontRectangle, Color.White);
            GameLoop.SpriteBatch.Draw(mEngineerIconTexture, engineerIconRectangle, mFrontRectangle, Color.White);
            GameLoop.SpriteBatch.Draw(mFighterCloseIconTexture, fighterCIconRectangle, mFrontRectangle, Color.White);
            GameLoop.SpriteBatch.Draw(mFighterDistanceIconTexture, fighterDIconRectangle, mFrontRectangle, Color.White);

            // draws the amount
            GameLoop.SpriteBatch.DrawString(mMediumFont, workerCounter + " x ",
                new Vector2(space / 4 + 0 * iconSize, (int)(GameLoop.DisplayHeight * 0.9)), Color.Black);
            GameLoop.SpriteBatch.DrawString(mMediumFont, engineerCounter + " x ",
                new Vector2(space / 4 + 1 * iconSize + 1 * space, (int)(GameLoop.DisplayHeight * 0.9)), Color.Black);
            GameLoop.SpriteBatch.DrawString(mMediumFont, fighterCloseCounter + " x ",
                new Vector2(space / 4 + 2 * iconSize + 2 * space, (int)(GameLoop.DisplayHeight * 0.9)), Color.Black);
            GameLoop.SpriteBatch.DrawString(mMediumFont, fighterDistanceCounter + " x ",
                new Vector2(space / 4 + 3 * iconSize + 3 * space, (int)(GameLoop.DisplayHeight * 0.9)), Color.Black);
        }

        /// <summary>
        /// renders in the left two thirds the icon, name, and amount of objects of multiple marked monsters
        /// </summary>
        /// <param name="gameObjects"></param>
        internal void RenderMultipleMarkedEnemies(List<GameObject> gameObjects)
        {
            if (gameObjects == null)
            {
                return;
            }

            // space between two icons (icon and left display edge)
            var space = GameLoop.DisplayWidth / 12;

            // edge length of the squared icon
            var iconSize = (int)Math.Min(GameLoop.DisplayWidth * 0.12, GameLoop.DisplayHeight * 0.1);

            int frogIconWidth;
            int frogIconHeight;
            int pigIconWidth;
            int pigIconHeight;
            var frogFrontRectangle = new Rectangle(199, 47, 33, 27);
            var pigFrontRectangle = new Rectangle(341, 20, 36, 51);

            if (frogFrontRectangle.Height > frogFrontRectangle.Width)
            {
                frogIconWidth = (int)(frogFrontRectangle.Width / (float)frogFrontRectangle.Height * iconSize);
                frogIconHeight = (int)(frogFrontRectangle.Height / (float)frogFrontRectangle.Width * frogIconWidth);
            }
            else
            {
                frogIconHeight = (int)(frogFrontRectangle.Height / (float)frogFrontRectangle.Width * iconSize);
                frogIconWidth = (int)(frogFrontRectangle.Width / (float)frogFrontRectangle.Height * frogIconHeight);
            }

            var frogIconRectangle = new Rectangle(space,
                (int)((GameLoop.DisplayHeight * 0.15 - frogIconHeight) / 2 + GameLoop.DisplayHeight * 0.85), frogIconWidth,
                frogIconHeight);

            if (pigFrontRectangle.Height > pigFrontRectangle.Width)
            {
                pigIconWidth = (int)(pigFrontRectangle.Width / (float)pigFrontRectangle.Height * iconSize);
                pigIconHeight = (int)(pigFrontRectangle.Height / (float)pigFrontRectangle.Width * pigIconWidth);
            }
            else
            {
                pigIconHeight = (int)(pigFrontRectangle.Height / (float)pigFrontRectangle.Width * iconSize);
                pigIconWidth = (int)(pigFrontRectangle.Width / (float)pigFrontRectangle.Height * pigIconHeight);
            }

            var pigIconRectangle = new Rectangle(2 * space + frogIconWidth,
                (int)((GameLoop.DisplayHeight * 0.15 - pigIconHeight) / 2 + GameLoop.DisplayHeight * 0.85), pigIconWidth,
                pigIconHeight);

            var frogCounter = 0;
            var pigCounter = 0;

            // counts the amount of marked objects type worker, engineer, fighter
            foreach (var gameObject in gameObjects)
            {
                if (gameObject is MonsterFrog)
                {
                    frogCounter += 1;
                }
                if (gameObject is MonsterPig)
                {
                    pigCounter += 1;
                }
            }

            // draws icons
            GameLoop.SpriteBatch.Draw(mMonsterIconTexture, frogIconRectangle, 
                new Rectangle(199, 47, 33, 27), Color.White);
            GameLoop.SpriteBatch.Draw(mMonsterIconTexture, pigIconRectangle, 
                new Rectangle(341, 20, 36, 51), Color.White);

            // draws the amount
            GameLoop.SpriteBatch.DrawString(mMediumFont, frogCounter + " x ",
                new Vector2(space / 4 + 0 * iconSize, (int)(GameLoop.DisplayHeight * 0.9)), Color.Black);
            GameLoop.SpriteBatch.DrawString(mMediumFont, pigCounter + " x ",
                new Vector2(space / 4 + 1 * iconSize + 1 * space, (int)(GameLoop.DisplayHeight * 0.9)), Color.Black);
        }

        /// <summary>
        /// creates new object on the map after the timer elapsed
        /// </summary>
        /// <param name="type">0: Worker, 1: Engineer, 2: FighterC, 3: FighterD</param>
        private void CreateNewObject(int type)
        {
            if (type > 3 || type < 0)
            {
                return;
            }

            var xSpawn = mRand.Next(2000, 2850);
            var ySpawn = mRand.Next(2700, 2900);

            while (GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(
                       new Rectangle(xSpawn, ySpawn, 32, 32)).Count != 0)
            {
                xSpawn = mRand.Next(2000, 2850);
                ySpawn = mRand.Next(2700, 2900);
            }

            switch (type)
            {
                case 0:
                    GameLoop.RenderManager.SoundManager.PlaySound("dna");

                    GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new Worker(GameLoop)
                        {Position = new Rectangle(xSpawn, ySpawn, 32, 32)});

                    GameLoop.ObjectManager.DataManager.mBuildUnits += 1;
                    GameLoop.Settings.mBuildUnits += 1;
                    break;
                case 1:
                    GameLoop.RenderManager.SoundManager.PlaySound("dna");

                    GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new Engineer(GameLoop)
                        {Position = new Rectangle(xSpawn, ySpawn, 32, 32)});

                    GameLoop.ObjectManager.DataManager.mBuildUnits += 1;
                    GameLoop.Settings.mBuildUnits += 1;
                    break;
                case 2:
                    GameLoop.RenderManager.SoundManager.PlaySound("dna");

                    GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new FighterClose(GameLoop)
                        {Position = new Rectangle(xSpawn, ySpawn, 32, 32)});

                    GameLoop.ObjectManager.DataManager.mBuildUnits += 1;
                    GameLoop.Settings.mBuildUnits += 1;
                    break;
                case 3:
                    GameLoop.RenderManager.SoundManager.PlaySound("dna");

                    GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new FighterDistance(GameLoop)
                        {Position = new Rectangle(xSpawn, ySpawn, 32, 32)});

                    GameLoop.ObjectManager.DataManager.mBuildUnits += 1;
                    GameLoop.Settings.mBuildUnits += 1;
                    break;
            }
        }

        // Functions that are executed when a button in the clone machine of InGameHud is pressed.
        private void NewWorkerButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.ObjectManager.DataManager.EnoughResourceList(GameLoop.ObjectManager.mProductionCosts[0]))
            {
                GameLoop.ObjectManager.DataManager.mNewObjectQueue[0] += 1;
                GameLoop.ObjectManager.DataManager.mWorkerQueue += 1;
                GameLoop.ObjectManager.DataManager.RemoveResources(GameLoop.ObjectManager.mProductionCosts[0]);
            }
        }
        private void NewEngineerButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.ObjectManager.DataManager.EnoughResourceList(GameLoop.ObjectManager.mProductionCosts[1]))
            {
                GameLoop.ObjectManager.DataManager.mNewObjectQueue[1] += 1;
                GameLoop.ObjectManager.DataManager.mEngineerQueue += 1;
                GameLoop.ObjectManager.DataManager.RemoveResources(GameLoop.ObjectManager.mProductionCosts[1]);
            }
        }
        private void NewFighterCloseButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.ObjectManager.DataManager.EnoughResourceList(GameLoop.ObjectManager.mProductionCosts[2]))
            {
                GameLoop.ObjectManager.DataManager.mNewObjectQueue[2] += 1;
                GameLoop.ObjectManager.DataManager.mCloseFighterQueue += 1;
                GameLoop.ObjectManager.DataManager.RemoveResources(GameLoop.ObjectManager.mProductionCosts[2]);
            }
        }
        private void NewFighterDistanceButton_Click(object sender, EventArgs e)
        {
            if (GameLoop.ObjectManager.DataManager.EnoughResourceList(GameLoop.ObjectManager.mProductionCosts[3]))
            {
                GameLoop.ObjectManager.DataManager.mNewObjectQueue[3] += 1;
                GameLoop.ObjectManager.DataManager.mDistanceFighterQueue += 1;
                GameLoop.ObjectManager.DataManager.RemoveResources(GameLoop.ObjectManager.mProductionCosts[3]);
            }
        }

        private void IdleButton_Click(object sender, EventArgs e)
        {
            ObjectManager.MarkAll(false, GameLoop.ObjectManager.DataManager.MarkedPlayerObjects);
            ObjectManager.MarkAll(false, GameLoop.ObjectManager.DataManager.MarkedEnemiesObjects);

            GameLoop.ObjectManager.DataManager.MarkedEnemiesObjects.Clear();
            GameLoop.ObjectManager.DataManager.MarkedPlayerObjects.Clear();
            GameLoop.ObjectManager.DataManager.OneMarkedObject = null;
            
            foreach (var worker in GameLoop.ObjectManager.DataManager.mUnits.GetAllEntries())
            {
                if (worker is Worker && worker.State == ObjectState.Idle)
                {
                    worker.IsMarked = true;
                    GameLoop.ObjectManager.DataManager.MarkedPlayerObjects.Add(worker);
                }
            }
        }
    }
}
