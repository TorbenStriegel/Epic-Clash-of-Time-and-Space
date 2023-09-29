
using ECTS.GUI.InGameMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using ECTS.Data;

namespace ECTS.Components
{
    /// <summary>
    /// Safes options and difficulty
    /// </summary>
    [DataContract]
    public sealed class Settings
    {
        private GameLoop GameLoop { get; set; }
        [DataMember]
        private float mVolumeMusic = 0.5f;                      // volume for Music (between 0 and 1), relative to system volume
        [DataMember]
        private float mVolumeEffect = 0.5f;                     // volume for Effects (between 0 and 1), relative to system volume

        // Achievements in game. True if reached, false if not.
        [DataMember] public bool mMasterOfWar1;
        [DataMember] public bool mMasterOfWar2;
        [DataMember] public bool mMasterOfWar3;
        [DataMember] public bool mCollector1;
        [DataMember] public bool mCollector2;
        [DataMember] public bool mCollector3;
        [DataMember] public bool mBuilder1;
        [DataMember] public bool mBuilder2;
        [DataMember] public bool mBuilder3;
        [DataMember] public bool mHumiliator1;
        [DataMember] public bool mHumiliator2;
        [DataMember] public bool mHumiliator3;
        [DataMember] public bool mWinnerOfHearts1;
        [DataMember] public bool mWinnerOfHearts2;
        [DataMember] public bool mWinnerOfHearts3;
        [DataMember] public bool mAttackOnPigfrog;
        [DataMember] public bool mLoser;
        [DataMember] public bool mSuchAWaste;
        [DataMember] public bool mThe0815;
        [DataMember] public bool mPayRespect;

        //Makes achievement checkable or not checkable / already reached once
        [DataMember] public bool mMasterOfWar1Open = true;
        [DataMember] public bool mMasterOfWar2Open = true;
        [DataMember] public bool mCollector1Open = true;
        [DataMember] public bool mCollector2Open = true;
        [DataMember] public bool mBuilder1Open = true;
        [DataMember] public bool mBuilder2Open = true;
        [DataMember] public bool mHumiliator1Open = true;
        [DataMember] public bool mHumiliator2Open = true;
        [DataMember] public bool mWinnerOfHearts1Open = true;
        [DataMember] public bool mWinnerOfHearts2Open = true;

        //Global Statistics 
        [DataMember] public TimeSpan mTotalGameTime;
        [DataMember] public int mBuildUnits;
        [DataMember] public int mKilledEnemies;
        [DataMember] public float mCollectedWood;
        [DataMember] public float mCollectedStone;
        [DataMember] public float mCollectedMetal;
        [DataMember] public float mCollectedFood;
        [DataMember] public float mSpaceshipRepaired;
        [DataMember] public int mDiedUnits;
        [DataMember] public int mCollectedDna;
        [DataMember] public int mGamesWon;
        [DataMember] public int mGamesLost;
        [DataMember] public bool mFirstNight;
        [DataMember] public bool mEctsTime;
        [DataMember] public TimeSpan mEctsMenu;
        [DataMember] public TimeSpan mFastestWin = TimeSpan.FromHours(3);
        [DataMember] public TimeSpan mLongestGame = TimeSpan.FromMinutes(10);
        [DataMember] public string mFastWin = "Jan";
        [DataMember] public string mLongGame = "Jan";
        [DataMember] public int mMostDays;


        [DataMember] internal int mWindowState;
        public Settings(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }

        // Set Music and Effect Volume
        internal float VolumeMusic
        {
            get => mVolumeMusic;
            set
            {
                if (value <= 1.0f && value >= 0.0f)     // volume has to be between 0 and 1
                {
                    mVolumeMusic = value;
                    GameLoop.SaveSettings();
                    GameLoop.RenderManager.SoundManager.SetVolume();
                }
            }

        }
        internal float VolumeEffect
        {
            get => mVolumeEffect;
            set
            {
                if (value <= 1.0f && value >= 0.0f)     // volume has to be between 0 and 1
                {
                    mVolumeEffect = value;
                    GameLoop.SaveSettings();
                    GameLoop.RenderManager.SoundManager.SetVolume();

                }
            }

        }

        // Looks if achievement is reached
        internal void UpdateAchievements()
        {
            var somethingChanged = false;

            if (mMasterOfWar1 == false && mMasterOfWar1Open)
            {
                if (GameLoop.ObjectManager.DataManager.TotalGameTime <= TimeSpan.FromMinutes(30) &&
                    GameLoop.ObjectManager.DataManager.Spaceship.Health >= 100 &&
                    GameLoop.ObjectManager.DataManager.Difficulty == DataManager.GameDifficulty.Easy)
                {
                    mMasterOfWar1 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_MasterOfWar"), "Achievement unlocked!\nMaster of War I"));
                }
            }

            if (mMasterOfWar2 == false && mMasterOfWar2Open)
            {
                if (GameLoop.ObjectManager.DataManager.TotalGameTime <= TimeSpan.FromMinutes(45) &&
                    GameLoop.ObjectManager.DataManager.Spaceship.Health >= 100 &&
                    GameLoop.ObjectManager.DataManager.Difficulty == DataManager.GameDifficulty.Medium)
                {
                    mMasterOfWar2 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_MasterOfWar"), "Achievement unlocked!\nMaster of War II"));
                }
            }

            if (mMasterOfWar2)
            {
                if (mMasterOfWar1)
                {
                    mMasterOfWar1 = false;
                    mMasterOfWar1Open = false;
                    somethingChanged = true;
                }
            }

            if (mMasterOfWar3 == false)
            {
                if (GameLoop.ObjectManager.DataManager.TotalGameTime <= TimeSpan.FromMinutes(60) &&
                    GameLoop.ObjectManager.DataManager.Spaceship.Health >= 100 &&
                    GameLoop.ObjectManager.DataManager.Difficulty == DataManager.GameDifficulty.Hard)
                {
                    mMasterOfWar3 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_MasterOfWar"), "Achievement unlocked!\nMaster of War III"));
                }
            }

            if (mMasterOfWar3)
            {
                if (mMasterOfWar2 && mMasterOfWar1Open == false)
                {
                    mMasterOfWar2 = false;
                    mMasterOfWar2Open = false;
                    somethingChanged = true;
                }
            }

            if (mCollector1 == false && mCollector1Open)
            {
                if ((int)GameLoop.ObjectManager.DataManager.mCollectedWood +
                    GameLoop.ObjectManager.DataManager.mCollectedStone +
                    GameLoop.ObjectManager.DataManager.mCollectedMetal +
                    GameLoop.ObjectManager.DataManager.mCollectedFood >= 10000)
                {
                    mCollector1 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                            ("Menu/popup/Achievements_Collector"), "Achievement unlocked!\nCollector I"));
                }
            }

            if (mCollector2 == false && mCollector2Open)
            {
                if ((int)GameLoop.ObjectManager.DataManager.mCollectedWood +
                    GameLoop.ObjectManager.DataManager.mCollectedStone +
                    GameLoop.ObjectManager.DataManager.mCollectedMetal +
                    GameLoop.ObjectManager.DataManager.mCollectedFood >= 25000)
                {
                    mCollector2 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Collector"), "Achievement unlocked!\nCollector II"));
                }
            }

            if (mCollector2)
            {
                if (mCollector1)
                {
                    mCollector1 = false;
                    mCollector1Open = false;
                    somethingChanged = true;
                }
            }

            if (mCollector3 == false)
            {
                if ((int)mCollectedWood + mCollectedStone + mCollectedMetal + mCollectedFood >= 100000)
                {
                    mCollector3 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Collector"), "Achievement unlocked!\nCollector III"));
                }
            }

            if (mCollector3)
            {
                if (mCollector2 && mCollector1Open == false)
                {
                    mCollector2 = false;
                    mCollector2Open = false;
                    somethingChanged = true;
                }
            }

            if (mBuilder1 == false && mBuilder1Open)
            {
                if (GameLoop.ObjectManager.DataManager.mBuildUnits >= 25)
                {
                    mBuilder1 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Builder"), "Achievement unlocked!\nBuilder I"));
                }
            }

            if (mBuilder2 == false && mBuilder2Open)
            {
                if (GameLoop.ObjectManager.DataManager.mBuildUnits >= 100)
                {
                    mBuilder2 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Builder"), "Achievement unlocked!\nBuilder II"));
                }
            }

            if (mBuilder2)
            {
                if (mBuilder1)
                {
                    mBuilder1 = false;
                    mBuilder1Open = false;
                    somethingChanged = true;
                }
            }

            if (mBuilder3 == false)
            {
                if (mBuildUnits >= 1000)
                {
                    mBuilder3 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Builder"), "Achievement unlocked!\nBuilder III"));
                }
            }

            if (mBuilder3)
            {
                if (mBuilder2 && mBuilder1Open == false)
                {
                    mBuilder2 = false;
                    mBuilder2Open = false;
                    somethingChanged = true;
                }
            }

            if (mHumiliator1 == false && mHumiliator1Open)
            {
                if (GameLoop.ObjectManager.DataManager.mKilledEnemies >= 20)
                {
                    mHumiliator1 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Humiliator"), "Achievement unlocked!\nHumiliator I"));
                }
            }

            if (mHumiliator2 == false && mHumiliator2Open)
            {
                if (GameLoop.ObjectManager.DataManager.mKilledEnemies >= 100)
                {
                    mHumiliator2 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Humiliator"), "Achievement unlocked!\nHumiliator II"));
                }
            }

            if (mHumiliator2)
            {
                if (mHumiliator1)
                {
                    mHumiliator1 = false;
                    mHumiliator1Open = false;
                    somethingChanged = true;
                }
            }

            if (mHumiliator3 == false)
            {
                if (mKilledEnemies >= 1000)
                {
                    mHumiliator3 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Humiliator"), "Achievement unlocked!\nHumiliator III"));
                }
            }

            if (mHumiliator3)
            {
                if (mHumiliator2 && mHumiliator1Open == false)
                {
                    mHumiliator2 = false;
                    mHumiliator2Open = false;
                    somethingChanged = true;
                }
            }

            if (mWinnerOfHearts1 == false && mWinnerOfHearts1Open)
            {
                if (mGamesWon >= 1)
                {
                    mWinnerOfHearts1 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Win"), "Achievement unlocked!\nWinner of Hearts I"));
                }
            }

            if (mWinnerOfHearts2 == false && mWinnerOfHearts2Open)
            {
                if (mGamesWon >= 10)
                {
                    mWinnerOfHearts2 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Win"), "Achievement unlocked!\nWinner of Hearts II"));
                }
            }

            if (mWinnerOfHearts2)
            {
                if (mWinnerOfHearts1)
                {
                    mWinnerOfHearts1 = false;
                    mWinnerOfHearts1Open = false;
                    somethingChanged = true;
                }
            }

            if (mWinnerOfHearts3 == false)
            {
                if (mGamesWon >= 25)
                {
                    mWinnerOfHearts3 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Win"), "Achievement unlocked!\nWinner of Hearts III"));
                }
            }

            if (mWinnerOfHearts3)
            {
                if (mWinnerOfHearts2 && mWinnerOfHearts1Open == false)
                {
                    mWinnerOfHearts2 = false;
                    mWinnerOfHearts2Open = false;
                    somethingChanged = true;
                }
            }

            if (mAttackOnPigfrog == false)
            {
                if (mFirstNight)
                {
                    mAttackOnPigfrog = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Pigfrog"), "Achievement unlocked!\nAttack on Pigfrog"));
                }
            }

            if (mSuchAWaste == false)
            {
                if (GameLoop.ObjectManager.DataManager.mWastedDna >= 5)
                {
                    mSuchAWaste = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Waste"), "Achievement unlocked!\nSuch a waste"));
                }
            }

            if (mThe0815 == false)
            {
                if (GameLoop.ObjectManager.DataManager.mWorkerQueue == 0 && 
                    GameLoop.ObjectManager.DataManager.mEngineerQueue == 8 &&
                    GameLoop.ObjectManager.DataManager.mCloseFighterQueue == 1 &&
                    GameLoop.ObjectManager.DataManager.mDistanceFighterQueue == 5)
                {
                    mThe0815 = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_0815"), "Achievement unlocked!\nThat's so 08/15"));
                }
            }

            if (mLoser == false)
            {
                if (mGamesLost >= 1)
                {
                    mLoser = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_Lose"), "Achievement unlocked!\nYou tried your best,\nbut you don't succeed."));
                }
            }

            if (mPayRespect == false)
            {
                if (mEctsTime)
                {
                    mPayRespect = true;
                    somethingChanged = true;
                    GameLoop.RenderManager.SoundManager.PlaySound("achievement");
                    GameLoop.ObjectManager.AddPopup(new PopUp(GameLoop, GameLoop.Content.Load<Texture2D>
                        ("Menu/popup/Achievements_ECTS"), "Achievement unlocked!\nECTS to Pay Respect"));
                }
            }

            if (somethingChanged)
            {
                GameLoop.SaveSettings();
            }
        }

        // Serialization
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            GameLoop = Global.mGameLoop;
            mWindowState = 1;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (GameLoop.mDebugMode)
            {
                return;
            }

            switch (mWindowState)
            {
                case 1:
                    GameLoop.mGraphics.IsFullScreen = true;
                    GameLoop.mGraphics.ApplyChanges();
                    GameLoop.Window.IsBorderless = false;
                    break;
                case 2:
                    GameLoop.mGraphics.PreferredBackBufferWidth = GameLoop.DisplayWidth;
                    GameLoop.mGraphics.PreferredBackBufferHeight = GameLoop.DisplayHeight;
                    GameLoop.mGraphics.IsFullScreen = false;
                    GameLoop.mGraphics.ApplyChanges();
                    GameLoop.Window.Position = new Point(0, 0);
                    GameLoop.Window.IsBorderless = false;
                    break;
                case 3:
                    GameLoop.Window.Position = new Point(0, 0);
                    GameLoop.Window.IsBorderless = true;
                    break;
            }
            GameLoop.mGraphics.ApplyChanges();
        }
    }
}
