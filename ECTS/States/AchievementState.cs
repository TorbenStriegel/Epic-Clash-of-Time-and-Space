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
    /// Shows the Achievements in the game
    /// </summary>
    [DataContract]
    internal sealed class AchievementState : State
    {
        private readonly List<Component> mComponents;
        private readonly Texture2D mBackground;
        private readonly Texture2D mMasterOfWar2D;
        private readonly Texture2D mCollector2D;
        private readonly Texture2D mBuilder2D;
        private readonly Texture2D mHumiliator2D;
        private readonly Texture2D mWinner2D;
        private readonly Texture2D mPigfrog2D;
        private readonly Texture2D mWaste2D;
        private readonly Texture2D m08152D;
        private readonly Texture2D mLose2D;
        private readonly Texture2D mEcts2D;
        private readonly Texture2D m0Stars2D;
        private readonly Texture2D m1Stars2D;
        private readonly Texture2D m2Stars2D;
        private readonly Texture2D m3Stars2D;
        private readonly State mPreviousState;
        private GameLoop Game { get; }

        public AchievementState(GameLoop game, State prevState) : base(game)
        {
            Game = game;
            mPreviousState = prevState;
            var content = mGameLoop.Content;
            mBackground = content.Load<Texture2D>("Background");
            var buttonTextureBack = content.Load<Texture2D>("Components/BackButton");
            var buttonTextureAchievements = content.Load<Texture2D>("Components/StartMenu/Achievements");
            mMasterOfWar2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_MasterOfWar");
            mCollector2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Collector");
            mBuilder2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Builder");
            mHumiliator2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Humiliator");
            mWinner2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Win");
            mPigfrog2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Pigfrog");
            mWaste2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Waste");
            m08152D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_0815");
            mLose2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_Lose");
            mEcts2D = Game.Content.Load<Texture2D>("Menu/popup/Achievements_ECTS");
            m0Stars2D = Game.Content.Load<Texture2D>("Menu/popup/0 Stars");
            m1Stars2D = Game.Content.Load<Texture2D>("Menu/popup/1 Stars");
            m2Stars2D = Game.Content.Load<Texture2D>("Menu/popup/2 Stars");
            m3Stars2D = Game.Content.Load<Texture2D>("Menu/popup/3 Stars");


            // Creates the buttons to be displayed in the menu.
            var backButton = new Button(buttonTextureBack, 40, (int)(mGameLoop.DisplayHeight * 0.92), false);
            backButton.Click += BackButton_Click;
            var achievementsButton = new Button(new Vector2((float)(game.DisplayWidth * 0.26), (float)(game.DisplayHeight * 0.01)),
                buttonTextureAchievements, 75, true);

            mComponents = new List<Component>
            {
                backButton,
                achievementsButton
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

            // edge length of the squared icon
            var iconSize = (int)Math.Min(Game.DisplayWidth * 0.1, Game.DisplayHeight * 0.1);
            var iconSize2 = (int)Math.Min(Game.DisplayWidth * 0.15, Game.DisplayHeight * 0.15);

            // chooses font
            var font = Game.RenderManager.FontSelector.GetFont(Game.DisplayWidth, FontSelector.FontSize.Large);
            var fontSmall = Game.RenderManager.FontSelector.GetFont(Game.DisplayWidth, FontSelector.FontSize.Small);

            //draws achievements
            if (Game.Settings.mMasterOfWar1)
            {
                Game.SpriteBatch.Draw(mMasterOfWar2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.20),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Master of War I",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.21)),
                    Color.Peru);
                Game.SpriteBatch.DrawString(fontSmall, "Win within the first 45 minutes on medium",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.26)),
                    Color.Peru);
                Game.SpriteBatch.Draw(m1Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.18),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mMasterOfWar2 && Game.Settings.mMasterOfWar1Open == false)
            {
                Game.SpriteBatch.Draw(mMasterOfWar2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.20),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Master of War II",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.21)),
                    Color.Silver);
                Game.SpriteBatch.DrawString(fontSmall, "Win within the first 60 minutes on hard",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.26)),
                    Color.Silver);
                Game.SpriteBatch.Draw(m2Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.18),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mMasterOfWar3 && Game.Settings.mMasterOfWar1Open == false &&
                     Game.Settings.mMasterOfWar2Open == false)
            {
                Game.SpriteBatch.Draw(mMasterOfWar2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.20),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Master of War III",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.21)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "You won within the first 60 minutes on hard",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.26)),
                    Color.Gold);
                Game.SpriteBatch.Draw(m3Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.18),
                        iconSize2, iconSize2), Color.White);
            }
            else
            {
                Game.SpriteBatch.Draw(mMasterOfWar2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.20),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Master of War",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.21)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Win within the first 30 minutes on easy",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.26)),
                    Color.White);
                Game.SpriteBatch.Draw(m0Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.18),
                        iconSize2, iconSize2), Color.White);
            }
            
            if (Game.Settings.mCollector1)
            {
                Game.SpriteBatch.Draw(mCollector2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.35),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Collector I",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.36)),
                    Color.Peru);
                Game.SpriteBatch.DrawString(fontSmall, "Collect 25.000 resources in one game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.41)),
                    Color.Peru);
                Game.SpriteBatch.Draw(m1Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.33),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mCollector2 && Game.Settings.mCollector1Open == false)
            {
                Game.SpriteBatch.Draw(mCollector2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.35),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Collector II",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.36)),
                    Color.Silver);
                Game.SpriteBatch.DrawString(fontSmall, "Collect a total of 100.000 resources in the game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.41)),
                    Color.Silver);
                Game.SpriteBatch.Draw(m2Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.33),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mCollector3 && Game.Settings.mCollector1Open == false &&
                     Game.Settings.mCollector2Open == false)
            {
                Game.SpriteBatch.Draw(mCollector2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.35),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Collector III",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.36)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "You collected a total of 100.000 resources in the game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.41)),
                    Color.Gold);
                Game.SpriteBatch.Draw(m3Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.33),
                        iconSize2, iconSize2), Color.White);
            }
            else
            {
                Game.SpriteBatch.Draw(mCollector2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.35),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Collector",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.36)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Collect 10.000 resources in one game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.41)),
                    Color.White);
                Game.SpriteBatch.Draw(m0Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.33),
                        iconSize2, iconSize2), Color.White);
            }

            if (Game.Settings.mBuilder1)
            {
                Game.SpriteBatch.Draw(mBuilder2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.50),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Builder I",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.51)),
                    Color.Peru);
                Game.SpriteBatch.DrawString(fontSmall, "Build 100 troops in one game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.56)),
                    Color.Peru);
                Game.SpriteBatch.Draw(m1Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.48),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mBuilder2 && Game.Settings.mBuilder1Open == false)
            {
                Game.SpriteBatch.Draw(mBuilder2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.50),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Builder II",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.51)),
                    Color.Silver);
                Game.SpriteBatch.DrawString(fontSmall, "Build a total of 1000 troops in the game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.56)),
                    Color.Silver);
                Game.SpriteBatch.Draw(m2Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.48),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mBuilder3 && Game.Settings.mBuilder1Open == false &&
                     Game.Settings.mBuilder2Open == false)
            {
                Game.SpriteBatch.Draw(mBuilder2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.50),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Builder III",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.51)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "You builded a total of 1000 troops in the game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.56)),
                    Color.Gold);
                Game.SpriteBatch.Draw(m3Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.48),
                        iconSize2, iconSize2), Color.White);
            }
            else
            {
                Game.SpriteBatch.Draw(mBuilder2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.50),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Builder",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.51)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Build 25 troops in one game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.56)),
                    Color.White);
                Game.SpriteBatch.Draw(m0Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.48),
                        iconSize2, iconSize2), Color.White);
            }

            if (Game.Settings.mHumiliator1)
            {
                Game.SpriteBatch.Draw(mHumiliator2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.65),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Humiliator I",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.66)),
                    Color.Peru);
                Game.SpriteBatch.DrawString(fontSmall, "Kill 100 enemies in one game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.71)),
                    Color.Peru);
                Game.SpriteBatch.Draw(m1Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.63),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mHumiliator2 && Game.Settings.mHumiliator1Open == false)
            {
                Game.SpriteBatch.Draw(mHumiliator2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.65),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Humiliator II",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.66)),
                    Color.Silver);
                Game.SpriteBatch.DrawString(fontSmall, "Kill a total of 1000 enemies in the game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.71)),
                    Color.Silver);
                Game.SpriteBatch.Draw(m2Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.63),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mHumiliator3 && Game.Settings.mHumiliator1Open == false &&
                     Game.Settings.mHumiliator2Open == false)
            {
                Game.SpriteBatch.Draw(mHumiliator2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.65),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Humiliator III",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.66)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "You killed a total of 1000 enemies in the game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.71)),
                    Color.Gold);
                Game.SpriteBatch.Draw(m3Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.63),
                        iconSize2, iconSize2), Color.White);
            }
            else
            {
                Game.SpriteBatch.Draw(mHumiliator2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.65),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Humiliator",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.66)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Kill 20 enemies in one game",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.71)),
                    Color.White);
                Game.SpriteBatch.Draw(m0Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.63),
                        iconSize2, iconSize2), Color.White);
            }

            if (Game.Settings.mWinnerOfHearts1)
            {
                Game.SpriteBatch.Draw(mWinner2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.80),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Winner of Hearts I",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.81)),
                    Color.Peru);
                Game.SpriteBatch.DrawString(fontSmall, "Win the game 10 times",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.86)),
                    Color.Peru);
                Game.SpriteBatch.Draw(m1Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.78),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mWinnerOfHearts2 && Game.Settings.mWinnerOfHearts1Open == false)
            {
                Game.SpriteBatch.Draw(mWinner2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.80),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Winner of Hearts II",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.81)),
                    Color.Silver);
                Game.SpriteBatch.DrawString(fontSmall, "Win the game 25 times",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.86)),
                    Color.Silver);
                Game.SpriteBatch.Draw(m2Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.78),
                        iconSize2, iconSize2), Color.White);
            }
            else if (Game.Settings.mWinnerOfHearts3 && Game.Settings.mWinnerOfHearts2Open == false &&
                     Game.Settings.mWinnerOfHearts1Open == false)
            {
                Game.SpriteBatch.Draw(mWinner2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.80),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Winner of Hearts III",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.81)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "You won the game 25 times",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.86)),
                    Color.Gold);
                Game.SpriteBatch.Draw(m3Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.78),
                        iconSize2, iconSize2), Color.White);
            }
            else
            {
                Game.SpriteBatch.Draw(mWinner2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.05), (int)(Game.DisplayHeight * 0.80),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Winner of Hearts",
                    new Vector2((int)(Game.DisplayWidth * 0.125), (int)(Game.DisplayHeight * 0.81)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Win the game once",
                    new Vector2((int)(Game.DisplayWidth * 0.13), (int)(Game.DisplayHeight * 0.86)),
                    Color.White);
                Game.SpriteBatch.Draw(m0Stars2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.40), (int)(Game.DisplayHeight * 0.78),
                        iconSize2, iconSize2), Color.White);
            }

            if (Game.Settings.mAttackOnPigfrog)
            {
                Game.SpriteBatch.Draw(mPigfrog2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.20),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Attack on Pigfrog",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.21)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "Reach the first night",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.26)),
                    Color.Gold);
            }
            else
            {
                Game.SpriteBatch.Draw(mPigfrog2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.20),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Attack on Pigfrog",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.21)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Hidden",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.26)),
                    Color.White);
            }

            if (Game.Settings.mSuchAWaste)
            {
                Game.SpriteBatch.Draw(mWaste2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.35),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Such a waste",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.36)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "Collect 5 DNA in a row with a level 3 unit",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.41)),
                    Color.Gold);
            }
            else
            {
                Game.SpriteBatch.Draw(mWaste2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.35),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "Such a waste",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.36)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Hidden",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.41)),
                    Color.White);
            }


            if (Game.Settings.mThe0815)
            {
                Game.SpriteBatch.Draw(m08152D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.50),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "That's so 08/15",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.51)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "Have 0 workers, 8 engineers, 1 close fighter and",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.56)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "5 distance fighters at the same time in the queue",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.58)),
                    Color.Gold);
            }
            else
            {
                Game.SpriteBatch.Draw(m08152D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.50),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "That's so 08/15",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.51)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Hidden",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.56)),
                    Color.White);
            }

            if (Game.Settings.mLoser)
            {
                Game.SpriteBatch.Draw(mLose2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.65),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "You tried your best...",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.66)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "...but you don't succeed. || Lose the game",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.71)),
                    Color.Gold);
            }
            else
            {
                Game.SpriteBatch.Draw(mLose2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.65),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "You tried your best...",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.66)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Hidden",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.71)),
                    Color.White);
            }

            if (Game.Settings.mPayRespect)
            {
                Game.SpriteBatch.Draw(mEcts2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.80),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "ECTS to Pay Respect",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.81)),
                    Color.Gold);
                Game.SpriteBatch.DrawString(fontSmall, "Read through the ECTS Menu",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.86)),
                    Color.Gold);
            }
            else
            {
                Game.SpriteBatch.Draw(mEcts2D,
                    new Rectangle((int)(Game.DisplayWidth * 0.55), (int)(Game.DisplayHeight * 0.80),
                        iconSize, iconSize), Color.White);
                Game.SpriteBatch.DrawString(font, "ECTS to Pay Respect",
                    new Vector2((int)(Game.DisplayWidth * 0.625), (int)(Game.DisplayHeight * 0.81)),
                    Color.White);
                Game.SpriteBatch.DrawString(fontSmall, "Hidden",
                    new Vector2((int)(Game.DisplayWidth * 0.63), (int)(Game.DisplayHeight * 0.86)),
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
