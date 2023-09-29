using System;
using ECTS.Components;
using ECTS.GUI;
using ECTS.Objects;
using ECTS.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS
{
    public sealed class GameLoop : Game
    {
        /// <summary>
        /// Debug Mode: Window Mode,
        /// false = full screen
        /// true = debug screen
        /// </summary>
        public bool mDebugMode = false;

        // Tech Demo (for testing)
        public bool mTechDemo = false;

        public Settings Settings { get; private set; }
        public AiManager AiManager { get; }
        public InputManager InputManager { get; }
        public RenderManager RenderManager { get; private set; }
        public ObjectManager ObjectManager { get; private set; }
        public Serialization Serialization { get; }

        public SpriteBatch SpriteBatch { get; private set; }

        public int DisplayWidth { get; private set; }
        public int DisplayHeight { get; private set; }

        public State mCurrentState;
        private State mNextState;
        public readonly GraphicsDeviceManager mGraphics;

        public GameLoop()
        {
            Global.mGameLoop = this;
            Global.mColor = new Color(255, 255, 255, 255);
            Settings = new Settings(this);

            mGraphics = new GraphicsDeviceManager(this);
            // LOAD SETTINGS
            if (Serialization.ExistsSaveGame("Settings.xml"))
            {
                LoadSettings();
            } // else: Standard settings will be applied.
            InputManager = new InputManager(this);
            RenderManager = new RenderManager(this);
            ObjectManager = new ObjectManager(this);
            AiManager = new AiManager(this);
            Serialization = new Serialization(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            if (mDebugMode)
            {
                DisplayWidth = GraphicsDevice.DisplayMode.Width/2;
                DisplayHeight = GraphicsDevice.DisplayMode.Height/2;
            }
            else
            {
                DisplayWidth = GraphicsDevice.DisplayMode.Width;
                DisplayHeight = GraphicsDevice.DisplayMode.Height;
            }
            base.Initialize();
        }
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            IsMouseVisible = true;

            if (mDebugMode)
            {
                mGraphics.PreferredBackBufferWidth = DisplayWidth;
                mGraphics.PreferredBackBufferHeight = DisplayHeight;
            }
            else
            {
                mGraphics.PreferredBackBufferWidth = DisplayWidth;
                mGraphics.PreferredBackBufferHeight = DisplayHeight;
                if (!Serialization.ExistsSaveGame("Settings.xml"))
                {
                    mGraphics.IsFullScreen = true;
                }
                
            }
            mGraphics.ApplyChanges();
            Window.Title = "ECTS";
            Window.AllowAltF4 = true;
            Window.Position = new Point(0, 0); // Fix error with windows task bar
            RenderManager.SoundManager.LoadContent();
            mCurrentState = new MenuState(this);
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive && !(mCurrentState is PauseState))  // Do not accept input if game is not active
            {
                return;
            }
            if (mNextState != null)
            {
                mCurrentState = mNextState;
                mNextState = null;
            }

            Settings.mTotalGameTime = Settings.mTotalGameTime.Add(gameTime.ElapsedGameTime);

            ObjectManager.UpdatePopup(gameTime);

            mCurrentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(transformMatrix: RenderManager.CameraManager.Transform, blendState: BlendState.NonPremultiplied);
            mCurrentState.Draw();
            SpriteBatch.End();

            SpriteBatch.Begin();
            mCurrentState.DrawGui(gameTime);
            ObjectManager.mCurrentPopUp?.Draw();
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public Texture2D GetTexture(string assetName)
        {
            return Content.Load<Texture2D>(assetName);
        }
        public void ChangeState(State state)
        {
            mNextState = state;
        }

        public void NewGame()
        {
            ObjectManager = new ObjectManager(this);
            ObjectManager.LoadContent();
            RenderManager = new RenderManager(this);
            RenderManager.LoadContent();
            RenderManager.CameraManager.SetCameraPosition(0, 0);
            InputManager.ResetScrollWheel();
        }

        /// <summary>
        /// Serializes GameSettings
        /// </summary>
        internal void SaveSettings(string path = "Settings.xml")
        {
            try
            {
                Serialization.Serialize(path, Settings);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        /// <summary>
        /// Deserializes GameSettings
        /// </summary>
        private void LoadSettings(string path = "Settings.xml")
        {
            try
            {
                Settings = (dynamic)Serialization.Deserialize(path, Settings);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}