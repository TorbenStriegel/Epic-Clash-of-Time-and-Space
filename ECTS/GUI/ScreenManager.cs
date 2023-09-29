using System;
using System.Collections.Generic;
using ECTS.GUI.InGameMenu;
using ECTS.Objects;
using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.GUI
{
    /// <summary>
    /// responsible for playing surface
    /// </summary>
    internal sealed class ScreenManager
    {
        private GameLoop GameLoop { get; }
        private HamburgerMenu HamburgerMenu { get; }
        internal HudMenu HudMenu { get; }
        public ResourceMenu ResourceMenu { get; }
        internal Tutorial Tutorial { get; }
        internal bool mOpenHud;
        internal bool mStayOpen;
        private Texture2D mMarkedBox;
        private Texture2D mMarkedItemTexture;
        internal Rectangle mDestinationRectangle;

        internal ScreenManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            HamburgerMenu = new HamburgerMenu(GameLoop);
            HudMenu = new HudMenu(GameLoop);
            ResourceMenu = new ResourceMenu(GameLoop);
            Tutorial = new Tutorial(gameLoop);
            mOpenHud = false;
            mStayOpen = false;
        }

        /// <summary>
        /// loads content of: hamburger menu, resources, hud, markings
        /// </summary>
        internal void LoadContent()
        {
            HamburgerMenu.LoadContent();
            HudMenu.LoadContent();
            ResourceMenu.LoadContent();
            Tutorial.LoadContent();
            mMarkedBox = GameLoop.GetTexture("Player/MarkedBox");
            mMarkedItemTexture = GameLoop.GetTexture("Player/active_object");
        }

        /// <summary>
        /// updates the InGameMenus
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            GameLoop.RenderManager.ScreenManager.HamburgerMenu.Update();
            GameLoop.RenderManager.ScreenManager.ResourceMenu.Update(gameTime);
			GameLoop.RenderManager.ScreenManager.HudMenu.Update(gameTime);
            GameLoop.RenderManager.ScreenManager.Tutorial.Update();
        }

        /// <summary>
        /// draws all parts of the hud and menus
        /// </summary>
        public void Draw()
        {
            HamburgerMenu.Render();
            HudMenu.RenderClock();
            HudMenu.RenderBackground();
			HudMenu.RenderCloneMachine();  // render right third -> clone machine
            ResourceMenu.Render();
            Tutorial.Render();

            SelectDrawnObjects(GameLoop.ObjectManager.DataManager.MarkedPlayerObjects,
                GameLoop.ObjectManager.DataManager.MarkedEnemiesObjects, GameLoop.ObjectManager.DataManager.OneMarkedObject);
            
            if (mOpenHud && mStayOpen)
            {
                HamburgerMenu.RenderDropdownMenu();
                
                HamburgerMenu.Update();
            }
        }

        /// <summary>
        /// render marked objects priority: multiple players > multiple enemies > one player > one monster
        /// </summary>
        /// <param name="markedPlayers">List of all marked player objects</param>
        /// <param name="markedEnemies">List of all marked monsters</param>
        /// <param name="oneMarkedObject">if only one piece of wall or spaceship is marked -> object, else null</param>
        private void SelectDrawnObjects(List<GameObject> markedPlayers,
            List<GameObject> markedEnemies, GameObject oneMarkedObject)
        {
            if (oneMarkedObject != null)
            {
                HudMenu.RenderOneMarkedObject(oneMarkedObject);
            } 
            else if (markedPlayers.Count > 1)
            {
                ObjectManager.MarkAll(true, markedPlayers);
                ObjectManager.MarkAll(false, markedEnemies);
                HudMenu.RenderMultipleMarkedPlayers(markedPlayers);
            }
            else if (markedEnemies.Count > 1)
            {
                ObjectManager.MarkAll(true, markedEnemies);
                ObjectManager.MarkAll(false, markedPlayers);
                HudMenu.RenderMultipleMarkedEnemies(markedEnemies);
            } 
            else if (markedPlayers.Count == 1)
            {
                ObjectManager.MarkAll(true, markedPlayers);
                ObjectManager.MarkAll(false, markedEnemies);
                HudMenu.RenderOneMarkedObject(markedPlayers[0]);
            }
            else if (markedEnemies.Count == 1)
            {
                ObjectManager.MarkAll(true, markedEnemies);
                ObjectManager.MarkAll(false, markedPlayers);
                HudMenu.RenderOneMarkedObject(markedEnemies[0]);
            }
        }

        internal void DrawWithCameraDisplacement()
        {
            DrawMarkedBox();
            DrawMarkItemAboveObject();
        }

        /// <summary>
        /// draws the grey box for marking objects, box can be created by mouse clicking
        /// </summary>
        private void DrawMarkedBox()
        {
            if (!mDestinationRectangle.IsEmpty) // not empty
            {
                GameLoop.SpriteBatch.Draw(mMarkedBox, mDestinationRectangle,
                    new Color(new Vector4(0.5f, 0.5f, 0.5f, (float)0.5)));
            }
        }

        /// <summary>
        /// Renders the yellow triangle above a marked object
        /// </summary>
        private void DrawMarkItemAboveObject()
        {
            var quadTree = GameLoop.ObjectManager.DataManager.mUnits;
            foreach (var gameObject in quadTree.GetAllEntries())
            {
                if (!gameObject.IsMarked)
                {
                    continue;
                }

                var xObject = gameObject.Position.X;
                var yObject = gameObject.Position.Y;

                var destinationRectangle = new Rectangle(xObject, yObject - 30, 30, 30);

                GameLoop.SpriteBatch.Draw(mMarkedItemTexture, destinationRectangle, Color.White);
            }
        }

        /// <summary>
        /// calculates the right rectangle of the heart sprite for the given health
        /// </summary>
        /// <param name = "health" > health of object: int between 0 - 100</param>
        /// <returns>Rectangle sprite of hearts</returns>
        public static Rectangle GetHeartTextureRectangle(int health)
        {
            if (health < 0)
            {
                health = 0;
            }
            else if (health > 100)
            {
                health = 100;
            }
            
            var lostHearts = (int) (10 - Math.Ceiling(health / 10f));
            return new Rectangle(0, lostHearts * 16, 134, 16);
        }
    }
}
