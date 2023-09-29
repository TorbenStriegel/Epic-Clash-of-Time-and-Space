using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Components;
using ECTS.Data;
using ECTS.GUI.InGameMenu;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.uncontrollable_units;
using ECTS.Objects.GameObjects.uncontrollable_units.spaceship;
using ECTS.Objects.GameObjects.uncontrollable_units.walls;
using ECTS.Objects.GameObjects.weapons;
using ECTS.Pathfinder;
using Microsoft.Xna.Framework;


namespace ECTS.Objects
{
    /// <summary>
    /// manages movable and static objects in the game world
    /// </summary>
    public sealed class ObjectManager
    {
        private GameLoop GameLoop { get; }

        private readonly List<PopUp> mPopupList;
        internal PopUp mCurrentPopUp;

        internal DataManager DataManager { get; private set; }
        internal QuadTree mTempTree;
        [DataMember] public List<GameObject> mGameObjectsFromQueue;
        internal readonly Map mMap;
        internal Pathfinder.Pathfinder Pathfinder { get; }

        internal ActionManager ActionManager { get; }

        //for ActionControl / Path finding
        private float TimeSumResourceSpawn { get; set; }  // Time counter
        private const float TimeIntervalResourceSpawn = 288.000f; //  0.016f = 60FPS, 0.960f = every second,  57.600 = every minute
        private float TimeSumAction { get; set; }  // Time counter between each Update() for ObjectAction decisions
        private const float TimeIntervalAction = 0.016f; // TimeInterval for ObjectAction decisions.

        private Rectangle mTargetArea;      // If active object is sent to a location, this is the target area.
        private List<GameObject> mTargetList;   // List with targets for active object

        private List<GameObject> mPlayerEnemyList;          // List with acting Player and Enemy objects
        private List<GameObject> mTempPlayerEnemyList;      // Temp List with acting Player and Enemy objects
        private List<GameObject> mEnemyList;                // List with Enemy objects
        private List<GameObject> mTempEnemyList;            // Temp List with Enemy objects

        private List<GameObject> mBulletExplosionList;      // List with acting Player and Enemy objects
        private List<GameObject> mTempBulletExplosionList;  // Temp List with acting Player and Enemy objects

        private List<GameObject> mDnaList;                  // List with all available DNAs
        private List<GameObject> mTempDnaList;

        internal List<GameObject> mOpenWallGateList;         // List with all OPEN Walls and Gates (used if empty, monsters will not try to walk into the fortress)
        private List<GameObject> mTempOpenWallGateList;

        private int mPeListPointer;
        private int mListStop;

        internal bool mDayChange;


        /// <summary>
        /// Worker, Engineer, FighterClose, FighterDistance
        /// {Metal, Stone, Wood, Food}
        /// </summary>
        internal readonly List<List<int>> mProductionCosts = new List<List<int>> {
                new List<int> {  0,  0, 20, 20 },
                new List<int> {  0, 20, 25, 20 },
                new List<int> { 15, 25, 30, 50 },
                new List<int> { 40, 30, 20, 80 }
            };

        /// <summary>
        /// AI monster strength value
        /// MonsterFrog, MonsterPig
        /// </summary>
        internal readonly List<int> mMonsterValue = new List<int>
            {
                2,  // Frog
                1   // Pig
            };

        /// <summary>
        /// Initializes DataManager, Pathfinder and Action manager.  <para/>
        /// Initializes all objects which are in the game when started and puts them into the quad-tree. <para/>
        /// Loops through the quad-tree and updates all objects (positions, actions path finding...).
        /// </summary>
        internal ObjectManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            DataManager = new DataManager(gameLoop);
            mMap = new Map(gameLoop, DataManager);
            Pathfinder = new Pathfinder.Pathfinder(gameLoop, DataManager);
            ActionManager = new ActionManager(gameLoop);

            mTempTree = new QuadTree(DataManager.mUnits.mPosition, 4, 0);
            mGameObjectsFromQueue = new List<GameObject>();

            mPlayerEnemyList = new List<GameObject>();
            mTempPlayerEnemyList = new List<GameObject>();
            mEnemyList = new List<GameObject>();
            mTempEnemyList = new List<GameObject>();

            mBulletExplosionList = new List<GameObject>();
            mTempBulletExplosionList = new List<GameObject>();

            mDnaList = new List<GameObject>();
            mTempDnaList = new List<GameObject>();

            mOpenWallGateList = new List<GameObject>();         
            mTempOpenWallGateList = new List<GameObject>();


            // MAP SPAWNING
            if (!GameLoop.mTechDemo)
            {
                mMap.SpawnInfRes(); // unlimited Resources at the map-borders
                mMap.SpawnRenewableRes(70);  // random vegetation
                mMap.DoUnsortedThings();  // everything else that was in this if-statement
            }
            else
            {
                mMap.TechDemo();
                mMap.SpawnRenewableRes(70);  // random vegetation
            }

            Pathfinder = new Pathfinder.Pathfinder(gameLoop, DataManager);

            for (var x = 0; x < 100; x++)
            {
                for (var y = 0; y < 100; y++)
                {
                    DataManager.mEnvironment.Add(new Floor(gameLoop, x * 48, y * 48));
                }
            }

            mPopupList = new List<PopUp>();
            mCurrentPopUp = null;
        }

        /// <summary>
        /// Marks every object that touches the given rectangle
        /// </summary>
        /// <param name="markedRectangle">rectangle </param>
        internal void MarkObjectInRectangle(Rectangle markedRectangle)
        {
            // all objects = not marked
            MarkAll(false, DataManager.MarkedPlayerObjects);
            MarkAll(false, DataManager.MarkedEnemiesObjects);

            DataManager.MarkedEnemiesObjects.Clear();
            DataManager.MarkedPlayerObjects.Clear();
            DataManager.OneMarkedObject = null;

            var markedObjectsQuadtree = DataManager.mUnits.GetEntriesInRectangle(markedRectangle);
            if (markedObjectsQuadtree.Count == 0)
            {
                return;
            }

            var tmpList = new List<GameObject>(markedObjectsQuadtree);
            foreach (var unit in tmpList)
            {
                if (unit is GrenadeEnemyExplosion)
                {
                    markedObjectsQuadtree.Remove(unit);
                }
            }

            foreach (var unit in markedObjectsQuadtree)
            {
                if (unit == null)
                {
                    break;
                }

                if (markedObjectsQuadtree.Count == 1)
                {
                    if (unit is WallHorizontal || unit is WallDownLeft || unit is WallLeftUp ||
                        unit is WallRightDown || unit is WallUpRight || unit is WallVertical ||
                        unit is Spaceship1)
                    {
                        DataManager.OneMarkedObject = unit;
                    }
                }

                switch (unit.mObjectType)
                {
                    case GameObject.ObjectType.Gate when markedObjectsQuadtree.Count == 1:
                        unit.ChangeFace("SwitchGate");
                        break;
                    case GameObject.ObjectType.Player:
                        DataManager.MarkedPlayerObjects.Add(unit);
                        break;
                    case GameObject.ObjectType.Enemy:
                        DataManager.MarkedEnemiesObjects.Add(unit);
                        break;
                }
            }
        }

        /// <summary>
        /// sets IsMarked = mark on every game object
        /// </summary>
        /// <param name="mark">true if marked</param>
        /// <param name="objectList">list of objects</param>

        internal static void MarkAll(bool mark, IEnumerable<GameObject> objectList)

        {
            foreach (var unit in objectList)
            {
                unit.IsMarked = mark;
            }
        }

        internal void LoadContent()
        {
            foreach (var gameObject in DataManager.mUnits.GetAllEntries())
            {
                gameObject.LoadContent();
            }

            foreach (var gameObject in DataManager.mEnvironment.GetAllEntries())
            {
                gameObject.LoadContent();
            }
        }

        internal void UpdatePopup(GameTime gameTime)
        {
            if (mCurrentPopUp == null && mPopupList.Count > 0)
            {
                mCurrentPopUp = mPopupList[0];
                mPopupList.RemoveAt(0);
            }
            else if (mCurrentPopUp != null)
            {
                mCurrentPopUp.mLifeSpan += gameTime.ElapsedGameTime;

                if (mCurrentPopUp.mLifeSpan < TimeSpan.FromSeconds(1))
                {
                    mCurrentPopUp.mPosition -= Vector2.Multiply(new Vector2(GameLoop.DisplayWidth * (float)0.3, 0), (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (mCurrentPopUp.mLifeSpan < TimeSpan.FromSeconds(9))
                {
                    mCurrentPopUp.mPosition = new Vector2(GameLoop.DisplayWidth - GameLoop.DisplayWidth * (float)0.3, GameLoop.DisplayHeight * 1 / 4f);
                }
                else if (mCurrentPopUp.mLifeSpan < TimeSpan.FromSeconds(10))
                {
                    mCurrentPopUp.mPosition += Vector2.Multiply(new Vector2(GameLoop.DisplayWidth * (float)0.3, 0), (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    mCurrentPopUp = null;
                }
            }

        }

        /// <summary>
        /// Update game objects in quad-tree.
        /// Updates path finding and object actions.
        /// </summary>
        /// <param name="gameTime">Game Time.</param>
        internal void Update(GameTime gameTime)
        {
            GameLoop.Settings.UpdateAchievements();


            TimeSumResourceSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds; //add time since last update
            TimeSumAction += (float)gameTime.ElapsedGameTime.TotalSeconds; //add time since last update
            DataManager.Update(gameTime);

            // Actions for all marked Objects:
            if (GameLoop.InputManager.mRightMouseButtonJustReleased && DataManager.MarkedPlayerObjects.Count != 0)
            {
                {
                    // Start Movement
                    Pathfinder.StartMovement(DataManager.MarkedPlayerObjects, GameLoop.InputManager.mCurrentMouseGamePosition);

                    // Add objects to target list.
                    mTargetArea = new Rectangle((int)GameLoop.InputManager.mCurrentMouseGamePosition.X, (int)GameLoop.InputManager.mCurrentMouseGamePosition.Y, 1, 1);
                    mTargetList = DataManager.mUnits.GetEntriesInRectangle(mTargetArea);
                    foreach (var gameObject in DataManager.MarkedPlayerObjects)
                    {
                        gameObject.mTargetObjects = mTargetList;
                    }
                }
            }

            var tempActiveFlocks = new HashSet<Flock>(DataManager.ActiveFlocks);
            foreach (var flock in tempActiveFlocks)
            {
                // Update all FLOCKS  (must be positioned after Pathfinder.StartMovement & before Pathfinder.ContinueMovement!)
                Pathfinder.NextMovement(flock);

                var tempFlockObjects = new HashSet<GameObject>(flock.mFlockObjects);
                foreach (var gameObject in tempFlockObjects)
                {
                    if (gameObject.State == GameObject.ObjectState.Moving)
                    {
                        Pathfinder.ContinueMovement(gameObject);
                    }
                    else // Remove gameObject from Flock if its movement-action was overridden.
                    {
                        Pathfinder.EndMovement(gameObject);
                    }
                }
            }

            // EXECUTE PLAYER and ENEMY ACTIONS
            if (TimeSumAction > TimeIntervalAction) { 
                mListStop = mPeListPointer + 10; // Update Object actions for 10 Objects per frame
                if (mListStop >= mPlayerEnemyList.Count - 1)
                {
                    mListStop = mPlayerEnemyList.Count - 1;
                }
                for (var i = mPeListPointer; i <= mListStop; i++) //mListPointer
                {
                    ActionManager.ActionSelector(mPlayerEnemyList[i]);
                    mPeListPointer += 1;
                }
                if (mPeListPointer >= mPlayerEnemyList.Count - 1)
                {
                    mPeListPointer = 0;
                }

                TimeSumAction = 0;
            }
            // EXECUTE BULLET and EXPLOSION ACTIONS
            for (var i = 0; i <= mBulletExplosionList.Count - 1; i++) //mListPointer
            {
                ActionManager.ActionSelector(mBulletExplosionList[i]);
            }

            // Check Actions/interactions for all Bullets and explosions
            for (var i = 0; i <= mDnaList.Count - 1; i++) //mListPointer
            {
                ActionManager.ActionSelector(mDnaList[i]);
            }

            // slowly remove enemies on DayChange
            if (mDayChange && GameLoop.mTechDemo == false)
            {
                if (mEnemyList.Count == 0 && GameLoop.ObjectManager.DataManager.Crack.Health <= 0)
                {
                    mDayChange = false;
                }
                else
                {
                    const float declineHealth = 0.5f;
                    foreach (var gameObject in mEnemyList)
                    {
                        gameObject.Health -= declineHealth;
                    }
                }

                if (GameLoop.ObjectManager.DataManager.Crack.Health > 1)
                {
                    GameLoop.ObjectManager.DataManager.Crack.Health -= 1;
                }
            }


            foreach (var gameObject in DataManager.mUnits.GetAllEntries())
            {
                gameObject.Update(gameTime);

                // if gameObjects are set to "null" they are not added to the tree any more
                // and can be removed by the garbage collector.
                if (!(gameObject.ObjectAge < gameObject.ObjectLifetime))
                {
                    continue;
                }

                mTempTree.Add(gameObject);

                // create list with all open Walls and Gates. DO NOT MOVE this, walls / gates are NOT acting.
                if ((gameObject.Type == GameObject.ObjectType.Wall || gameObject.Type == GameObject.ObjectType.Gate) &&
                    !gameObject.IsColliding)
                {
                    mTempOpenWallGateList.Add(gameObject);
                }

                if (!gameObject.IsActing)
                {
                    continue;
                }

                // Add ACTING game Objects to respective lists (action Planning is based upon lists)
                switch (gameObject.Type)
                {
                    case GameObject.ObjectType.Player:
                        mTempPlayerEnemyList.Add(gameObject);
                        break;
                    case GameObject.ObjectType.Enemy:
                        mTempPlayerEnemyList.Add(gameObject);
                        mTempEnemyList.Add(gameObject);
                        break;
                    case GameObject.ObjectType.Bullet:
                    case GameObject.ObjectType.Explosion:
                        mTempBulletExplosionList.Add(gameObject);
                        break;
                    case GameObject.ObjectType.Dna:
                        mTempDnaList.Add(gameObject);
                        break;
                }
            }

            if (mGameObjectsFromQueue.Count != 0) 
            {
                foreach (var gameObject in mGameObjectsFromQueue)
                {
                    mTempTree.Add(gameObject);
                }
                mGameObjectsFromQueue = new List<GameObject>();
            }

            if (TimeSumResourceSpawn >= TimeIntervalResourceSpawn)
            {
                // Add random vegetation objects
                mMap.SpawnRenewableRes(1, true);
                TimeSumResourceSpawn = 0f; // reset TimeCounter
            }

            DataManager.mUnits = mTempTree;
            mPlayerEnemyList = mTempPlayerEnemyList;
            mEnemyList = mTempEnemyList;
            mBulletExplosionList = mTempBulletExplosionList;
            mDnaList = mTempDnaList;
            mOpenWallGateList = mTempOpenWallGateList;
            mTempTree = new QuadTree(DataManager.mUnits.mPosition, 4, 0);
            mTempBulletExplosionList = new List<GameObject>();
            mTempPlayerEnemyList = new List<GameObject>();
            mTempEnemyList = new List<GameObject>();
            mTempDnaList = new List<GameObject>();
            mTempOpenWallGateList = new List<GameObject>();
        }

        /// <summary>
        /// Distance (int) between two given game objects (uses ObjectCenter)
        /// </summary>
        internal static int ObjectDistance(GameObject object1, GameObject object2)
        {
            var dist = (int)Math.Sqrt(
                Math.Pow(object1.ObjectCenter.X - object2.ObjectCenter.X, 2) +
                Math.Pow(object1.ObjectCenter.Y - object2.ObjectCenter.Y, 2));
            return dist;
        }


        // Serialization
        internal void Seri(string path = "Save_DataManager.xml")
        {
            Serialization.Serialize(path, DataManager);
        }

        internal void Deseri(string path = "Save_DataManager.xml")
        {
            DataManager = (dynamic)Serialization.Deserialize(path, DataManager);
        }

        internal void AddPopup(PopUp p)
        {
            mPopupList.Add(p);
        }
    }
}