using ECTS.Components;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.uncontrollable_units.spaceship;
using ECTS.Objects.GameObjects.uncontrollable_units.walls;
using ECTS.Pathfinder;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Objects.GameObjects.uncontrollable_units.crack;


namespace ECTS.Data
{
    [DataContract(IsReference = true)]
    public sealed class DataManager
    {
        private GameLoop GameLoop { get; set; }

        [DataMember] public QuadTree mEnvironment;
        [DataMember] public QuadTree mUnits;
        [DataMember] internal HashSet<Flock> ActiveFlocks { get; private set; }
        internal List<GameObject> MarkedPlayerObjects { get; private set; }
        /// <summary>
        /// list of marked enemies (monster frog, monster pig, ...)
        /// </summary>
        internal List<GameObject> MarkedEnemiesObjects { get; private set; }
        /// <summary>
        /// one piece of wall or the spaceship
        /// </summary>
        public GameObject OneMarkedObject { get; set; }
        [DataMember] public Rectangle mWorldRectangle;
        [DataMember] public TimeSpan TotalGameTime { get; private set; }  // counts Time of running game

        // GameStateDay/Night time and bool
        [DataMember] internal float mElapsedTime;
        [DataMember] internal bool mDay;
        public enum GameDifficulty {Easy, Medium, Hard}

        [DataMember]
        internal GameDifficulty Difficulty
        {
            get; 
            set;
        }

        [DataMember] public string mPlayerName;

        [DataMember] public float mZoomValue;

        // Metal, Stone, Wood, Food
        [field: DataMember] internal List<float> ResourceList { get; } = new List<float> {20, 100, 100, 50};

        [DataMember] public int mBuildUnits;
        [DataMember] public int mKilledEnemies;
        [DataMember] public float mCollectedWood;
        [DataMember] public float mCollectedStone;
        [DataMember] public float mCollectedMetal;
        [DataMember] public float mCollectedFood;
        [DataMember] public float mSpaceshipRepaired;
        [DataMember] public float mSpaceshipRepairing;
        [DataMember] public int mDiedUnits;
        [DataMember] public int mSurvivedDays;
        [DataMember] public int mCollectedDna;
        [DataMember] public int mWastedDna;

        [DataMember] public int mWorkerQueue;
        [DataMember] public int mEngineerQueue;
        [DataMember] public int mCloseFighterQueue;
        [DataMember] public int mDistanceFighterQueue;

        /// <summary>
        /// true, if a new object is in progress { Worker, Engineer, FighterClose, FighterDistance }
        /// </summary>
        [DataMember] internal List<bool> mMachineInProgress = new List<bool> { false, false, false, false };
        /// <summary>
        /// amount of new objects in the queue { Worker, Engineer, FighterClose, FighterDistance }
        /// </summary>
        [DataMember] internal List<int> mNewObjectQueue = new List<int> { 0, 0, 0, 0 };
        /// <summary>
        /// time left in seconds until new object ist produced { Worker, Engineer, FighterClose, FighterDistance }
        /// </summary>
        [DataMember] internal List<double> mRunningTimer = new List<double> { 0, 0, 0, 0 };

        [DataMember] internal Spaceship1 Spaceship { get; private set; }
        [DataMember] internal GateVertical GateW { get; set; }
        [DataMember] internal GateVertical GateO { get; set; }
        [DataMember] internal GateHorizontal GateN { get; set; }
        [DataMember] internal GateHorizontal GateS { get; set; }
        [DataMember] internal Crack Crack { get; set; }

        /// <summary>
        /// Remove resources (costs for buildings, units) by calling this method with their mProductionCost array
        /// </summary>
        /// <param name="removingResourceList"></param>
        internal void RemoveResources(List<int> removingResourceList)
        {
            for (var i = 0; i < 4; i++)
            {
                ResourceList[i] -= removingResourceList[i];
                if (ResourceList[i] < 0)
                {
                    ResourceList[i] = 0;
                }
            }
        }

        /// <summary>
        /// checks if the given list of needed resources are less or equal the owning resources
        /// </summary>
        /// <param name="neededResourcesList">{Metal, Stone, Wood, Food}</param>
        /// <returns>true, if they are enough resources</returns>
        internal bool EnoughResourceList(List<int> neededResourcesList)
        {
            if (neededResourcesList.Count != 4)
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                if (ResourceList[i] < neededResourcesList[i] && neededResourcesList[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if new fastest win is reached and updates values
        /// </summary>
        internal void FastWin()
        {
            if (TotalGameTime < GameLoop.Settings.mFastestWin)
            {
                GameLoop.Settings.mFastestWin = TotalGameTime;
                GameLoop.Settings.mFastWin = mPlayerName;
                if (mPlayerName.Equals(""))
                {
                    GameLoop.Settings.mFastWin = "Player Unknown";
                }
            }
        }

        /// <summary>
        /// Checks if new longest game was played and updates values
        /// </summary>
        internal void LongGame()
        {
            if (TotalGameTime > GameLoop.Settings.mLongestGame)
            {
                GameLoop.Settings.mLongestGame = TotalGameTime;
                GameLoop.Settings.mLongGame = mPlayerName;
                GameLoop.Settings.mMostDays = mSurvivedDays;
                if (mPlayerName.Equals(""))
                {
                    GameLoop.Settings.mLongGame = "Player Unknown";
                }
            }
        }

        internal Color GetColorFontForNeededResource(int neededResource, float owningResource)
        {
            if (owningResource < neededResource && neededResource != 0)
            {
                return Color.Red;
            }

            return Color.Black;
        }

        /// <summary>
        /// Initializing Method for this Class used in Constructor and for Serialization.
        /// </summary>
        private void Initialize()
        {
            GameLoop = Global.mGameLoop;
            mWorldRectangle = new Rectangle(0, 0, 100 * 48, 100 * 48);
            MarkedPlayerObjects = new List<GameObject>();
            MarkedEnemiesObjects = new List<GameObject>();
            mUnits = new QuadTree(mWorldRectangle, 4, 0);
            mEnvironment = new QuadTree(mWorldRectangle, 4, 0);
            ActiveFlocks = new HashSet<Flock>();

            Spaceship = new Spaceship1(Global.mGameLoop) { Position = new Rectangle(2250, 2173, 300, 454), Health = 50 };
            GateW = new GateVertical(Global.mGameLoop) { Position = new Rectangle(1968, 1920 + 10 * 48, 48, 96), IsColliding = false };
            GateO = new GateVertical(Global.mGameLoop) { Position = new Rectangle(2832, 1920 + 10 * 48, 48, 96), IsColliding = false };
            GateN = new GateHorizontal(Global.mGameLoop) { Position = new Rectangle(2016 + 8 * 48, 1872, 96, 48), IsColliding = false };
            GateS = new GateHorizontal(Global.mGameLoop) { Position = new Rectangle(2016 + 8 * 48, 2928, 96, 48), IsColliding = false };
            Crack = new Crack(Global.mGameLoop);
        }

        public DataManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            Initialize();
            Difficulty = GameDifficulty.Easy;
        }

        internal void Update(GameTime gameTime)
        {
            TotalGameTime = TotalGameTime.Add(gameTime.ElapsedGameTime);
        }

        // Serialization
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }
    }
}
