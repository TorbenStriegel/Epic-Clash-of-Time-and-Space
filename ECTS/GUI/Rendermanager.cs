using ECTS.Components;
using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;

namespace ECTS.GUI
{
    /// <summary>
    /// only draws relevant objects
    /// </summary>
    public sealed class RenderManager
    {
        private GameLoop GameLoop { get; }
        internal SoundManager SoundManager { get; }
        internal CameraManager CameraManager { get; private set; }
        internal ScreenManager ScreenManager { get; }
        internal FontSelector FontSelector { get; }
        internal RenderManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            SoundManager = new SoundManager(GameLoop);
            CameraManager = new CameraManager(GameLoop);
            ScreenManager = new ScreenManager(GameLoop);
            FontSelector = new FontSelector(GameLoop.Content);
        }

        /// <summary>
        /// Loads content of the sound and screen manager
        /// </summary>
        internal void LoadContent()
        {
            SoundManager.LoadContent(); 
            ScreenManager.LoadContent();
        }

        internal void Update(GameTime gameTime)
        {
            GameLoop.RenderManager.SoundManager.Update(gameTime);
            GameLoop.RenderManager.ScreenManager.Update(gameTime);
        }

        /// <summary>
        /// draws only the visible objects
        /// </summary>
        internal void DrawVisibleElements()
        {
            var cameraPosition = new Rectangle(0,0,
                GameLoop.DisplayWidth + 3000,
                GameLoop.DisplayHeight + 4000);

            var environment = GameLoop.ObjectManager.DataManager.mEnvironment;
            var objects = environment.GetEntriesInRectangle(cameraPosition);
            foreach (var gameObject in objects)
            {
                gameObject.Draw();
            }

            var units = GameLoop.ObjectManager.DataManager.mUnits;

            // always draw Spaceship first and below other objects.
            if (GameLoop.mTechDemo == false)
            {
                GameLoop.ObjectManager.DataManager.Spaceship.Draw();
            }

            foreach (var gameObject in units.GetEntriesInRectangle(cameraPosition))
            {
                if(gameObject.Type !=  GameObject.ObjectType.Spaceship)
                {
                    gameObject.Draw();
                }
            }
            ScreenManager.DrawWithCameraDisplacement();
        }

        // draws objects from the screen manager like game objects and hud
        internal void DrawGui()
        {
            ScreenManager.Draw();
        }

        /// <summary>
        /// Serialization
        /// </summary>
        /// <param name="path"></param>
        internal void Seri(string path = "CameraManager.xml")
        {
            Serialization.Serialize(path, CameraManager);
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        /// <param name="path"></param>
        internal void Deseri(string path = "CameraManager.xml")
        {
            CameraManager = (dynamic)Serialization.Deserialize(path, CameraManager);
        }
    }
}
