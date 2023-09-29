using System;
using System.Collections.Generic;
using ECTS.Data;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.controllable_units;
using ECTS.Objects.GameObjects.uncontrollable_units.crack;
using ECTS.Objects.GameObjects.uncontrollable_units.walls;
using Microsoft.Xna.Framework;

namespace ECTS.Objects
{
    /// <summary>
    /// creates and controls enemy
    /// </summary>
    public sealed class AiManager
    {
        /// <summary>
        /// List of lists: first object in the list is target and the other objects in the list are the attackers
        /// </summary>
        private List<List<GameObject>> mTargetsAttackersFlockList;
        private GameLoop GameLoop { get; }
        private readonly Random mRand = new Random();
        internal int mStrategyNumber;
        private int mDifficultyValue;                   // current difficulty value
        private int mGradient;                          // the more difficult the higher the value
        internal List<GameObject> mWalls;                // list of all walls
        private List<GameObject> mAllPlayers;
        private List<GameObject> mDamagedObjects;
        private List<GameObject> mObjectsInsideBase;    // all objects inside the base
        private List<GameObject> mObjectsOutsideBase;   // all objects outside the base
        private Vector2 mAiSpawn;
        private bool mIsObjectOutsideBase;
        private bool mIgnoreObjectsOutsideBase;         // if spaceship >= 90% repaired, ignore objects outside the base
        private bool mIsGapInWall;                      // is the health of the wall = 0, can smb step throw it?
        private bool mIsWallBroken;                     // is the wall damaged
        
        internal AiManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            mDifficultyValue = 1;
            mAiSpawn = new Vector2();
            SaveDifficulty();
        }

        internal void SaveDifficulty()
        {
            mDifficultyValue = 1;
    
            // which difficulty was set at the beginning
            switch (GameLoop.ObjectManager.DataManager.Difficulty)
            {
                case DataManager.GameDifficulty.Easy:
                    mGradient = 2;
                    break;
                case DataManager.GameDifficulty.Medium:
                    mGradient = 5;
                    break;
                case DataManager.GameDifficulty.Hard:
                    mGradient = 8;
                    break;
                default:
                    mGradient = 3;
                    break;
            }
        }

        /// <summary>
        /// selects the strategy, creates troop and selects for each monster the target
        /// </summary>
        internal void Ai()
        {
            // increase difficulty value
            if (mGradient == 0)
            {
                SaveDifficulty();
            }

            mStrategyNumber = StrategySelection();              // selects strategy
            PlaceAi();
            var troop = CreateTroop(mStrategyNumber);           // creates troop
            mTargetsAttackersFlockList = new List<List<GameObject>>();
            mIsObjectOutsideBase = false;
            SelectTargetsForMember(troop, mStrategyNumber);     // selects target for every troop member

            mDifficultyValue += mGradient;
        }

        /// <summary>
        /// selects the ai strategy
        /// </summary>
        /// <returns>returns the number of the strategy -> GDD</returns>
        private int StrategySelection()
        {
            InitializeVariables();

            return GameLoop.ObjectManager.DataManager.Spaceship.Health >= 90 ? DestroySpaceship() : NotDestroySpaceship();
        }

        private void InitializeVariables()
        {
            mAllPlayers = new List<GameObject>();
            mObjectsOutsideBase = new List<GameObject>();
            mDamagedObjects = new List<GameObject>();
            mWalls = new List<GameObject>();

            var allObjects = GameLoop.ObjectManager.DataManager.mUnits.GetAllEntries();
            mObjectsInsideBase = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(new Rectangle(
                GameLoop.ObjectManager.mMap.mBaseCornerRectangle.X, GameLoop.ObjectManager.mMap.mBaseCornerRectangle.Y,
                GameLoop.ObjectManager.mMap.mBaseCornerRectangle.Width, GameLoop.ObjectManager.mMap.mBaseCornerRectangle.Height));

            // checks condition of the wall
            mIsGapInWall = false;
            mIsWallBroken = false;
            mIgnoreObjectsOutsideBase = false;

            foreach (var gameObject in mObjectsInsideBase)
            {
                // no corners -> can't walk throw corners -> useless
                if (gameObject is WallHorizontal || gameObject is WallVertical)
                {
                    mWalls.Add(gameObject);

                    if (Math.Abs(gameObject.Health) <= 0 && Math.Abs(gameObject.Health) >= 0)
                    {
                        mIsGapInWall = true;
                        mIsWallBroken = true;
                        break;
                    }

                    if (gameObject.Health < 100)
                    {
                        mIsWallBroken = true;
                    }
                }
            }

            foreach (var gameObject in allObjects)
            {
                if (gameObject is Worker || gameObject is Engineer || gameObject is FighterClose ||
                    gameObject is FighterDistance)
                {
                    mAllPlayers.Add(gameObject);

                    mObjectsOutsideBase.Add(gameObject);

                    if (gameObject.Health < 100 && mIsGapInWall)
                    {
                        mDamagedObjects.Add(gameObject);
                    }
                }
            }

            foreach (var wall in mWalls)
            {
                if (wall.Health < 100)
                {
                    mDamagedObjects.Add(wall);
                }
            }

            var objectsInBase = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(new Rectangle(
                GameLoop.ObjectManager.mMap.mBaseCornerRectangle.X + 48, GameLoop.ObjectManager.mMap.mBaseCornerRectangle.Y + 48,
                GameLoop.ObjectManager.mMap.mBaseCornerRectangle.Width - 96, GameLoop.ObjectManager.mMap.mBaseCornerRectangle.Height - 96));
            foreach (var gameObject in objectsInBase)
            {
                mObjectsOutsideBase.Remove(gameObject);
            }
        }

        private int DestroySpaceship()
        {
            // spaceship >= 90
            mIgnoreObjectsOutsideBase = true;

            if (mIsGapInWall) // possible to step throw the wall
            {
                // destroy spaceship with close fighters
                return 5;
            }

            // destroy spaceship with distance fighters
            return 6;
        }

        private int NotDestroySpaceship()
        {
            // can monsters walk throw the wall
            if (mIsGapInWall)
            {
                // when there are players 
                if (mDamagedObjects.Count > 0)
                {
                    // kill damaged players
                    return 3;
                }

                if (mAllPlayers.Count > 0)
                {
                    // kill players or destroy spaceship
                    return mRand.Next(4, 7);
                }

                // no players left -> destroy spaceship
                return 5;
            }

            if (mIsWallBroken)
            {
                // destroy broken wall
                return 3;
            }

            // destroy fixed wall
            return 2;
        }

        /// <summary>
        /// creates troop according to the strategy number
        /// </summary>
        /// <param name="strategy">number between 1 and 9</param>
        /// <returns></returns>
        private List<GameObject> CreateTroop(int strategy)
        {
            if (strategy < 2 && strategy > 6)
            {
                strategy = StrategySelection();
            }

            switch (strategy)
            {
                case 1:
                    return CreateCloseFighters();
                case 5:
                    return CreateCloseFighters();
                case 2:
                    return CreateDistanceFighters();
                case 6:
                    return CreateDistanceFighters();
                default:        // strategy 3, 4
                    return CreateRandomFighters();
            }
        }

        private void PlaceAi()
        {
            Rectangle intersectionRectangle;
            bool isPlaceEmpty;

            do
            {
                mAiSpawn.X = mRand.Next(100, GameLoop.ObjectManager.DataManager.mWorldRectangle.Width - 100 - 397);
                mAiSpawn.Y = mRand.Next(100, GameLoop.ObjectManager.DataManager.mWorldRectangle.Height - 100 - 256);

                intersectionRectangle = Rectangle.Intersect(new Rectangle((int) mAiSpawn.X - 200, (int) mAiSpawn.Y - 200, 397 + 400, 256 + 400),
                    GameLoop.ObjectManager.mMap.mBaseCornerRectangle);

                isPlaceEmpty = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(new Rectangle((int)mAiSpawn.X, 
                    (int)mAiSpawn.Y, 397, 256)).Count == 0;

            } while (!intersectionRectangle.IsEmpty || !isPlaceEmpty);

            GameLoop.ObjectManager.DataManager.Crack = new Crack(GameLoop)
                {Position = new Rectangle((int) mAiSpawn.X, (int) mAiSpawn.Y, 397, 256)};

            GameLoop.ObjectManager.mGameObjectsFromQueue.Add(GameLoop.ObjectManager.DataManager.Crack);
        }

        /// <summary>
        /// creates a troop of distance fighters
        /// </summary>
        /// <returns>list of monster frogs</returns>
        private List<GameObject> CreateDistanceFighters()
        {
            var aiTroops = new List<GameObject>();
            int amount;

            if (mDifficultyValue == 1) // on monster in first night
            {
                amount = 1;
            }
            else
            {
                amount = mDifficultyValue / GameLoop.ObjectManager.mMonsterValue[0];
            }

            for (var i = 0; i < amount; i++)
            {
                GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new MonsterFrog(GameLoop)
                { Position = new Rectangle(mRand.Next((int)mAiSpawn.X, (int)(mAiSpawn.X + 397)), 
                    mRand.Next((int)mAiSpawn.Y, (int)(mAiSpawn.Y + 256)), 48, 27)});
                aiTroops.Add(GameLoop.ObjectManager.mGameObjectsFromQueue[GameLoop.ObjectManager.mGameObjectsFromQueue.Count - 1]);
            }

            return aiTroops;
        }

        /// <summary>
        /// creates a troop of close fighters
        /// </summary>
        /// <returns>list of monster pigs</returns>
        private List<GameObject> CreateCloseFighters()
        {
            var aiTroops = new List<GameObject>();
            var amount = mDifficultyValue / GameLoop.ObjectManager.mMonsterValue[1];
            for (var i = 0; i < amount; i++)
            {
                GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new MonsterPig(GameLoop)
                { Position = new Rectangle(mRand.Next((int)mAiSpawn.X, (int)(mAiSpawn.X + 397)),
                    mRand.Next((int)mAiSpawn.Y, (int)(mAiSpawn.Y + 256)), 48, 52)});
                aiTroops.Add(GameLoop.ObjectManager.mGameObjectsFromQueue[GameLoop.ObjectManager.mGameObjectsFromQueue.Count - 1]);
            }
            return aiTroops;
        }

        /// <summary>
        /// creates a troop of random fighters
        /// </summary>
        /// <returns>List of monster frogs and pigs</returns>
        private List<GameObject> CreateRandomFighters()
        {
            var aiTroops = new List<GameObject>();
            var rand = new Random();
            var amount = rand.Next(0, (int)(mDifficultyValue / 2f));  // creates a number between 0 and mDifficultyValue
            var leftDifficulty = mDifficultyValue - amount * GameLoop.ObjectManager.mMonsterValue[0];
            for (var i = 0; i < amount; i++)
            {
                GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new MonsterFrog(GameLoop)
                { Position = new Rectangle(mRand.Next((int)mAiSpawn.X, (int)(mAiSpawn.X + 397)),
                    mRand.Next((int)mAiSpawn.Y, (int)(mAiSpawn.Y + 256)), 48, 27)});
                aiTroops.Add(GameLoop.ObjectManager.mGameObjectsFromQueue[GameLoop.ObjectManager.mGameObjectsFromQueue.Count - 1]);
            }
            amount = leftDifficulty / GameLoop.ObjectManager.mMonsterValue[1];
            for (var i = 0; i < amount; i++)
            {
                GameLoop.ObjectManager.mGameObjectsFromQueue.Add(new MonsterPig(GameLoop)
                { Position = new Rectangle(mRand.Next((int)mAiSpawn.X, (int)(mAiSpawn.X + 397)),
                    mRand.Next((int)mAiSpawn.Y, (int)(mAiSpawn.Y + 256)), 48, 52)});
                aiTroops.Add(GameLoop.ObjectManager.mGameObjectsFromQueue[GameLoop.ObjectManager.mGameObjectsFromQueue.Count - 1]);
            }

            return aiTroops;
        }

        /// <summary>
        /// selects the target of each troop member
        /// </summary>
        /// <param name="troop">distance, close or random monsters</param>
        /// <param name="strategy">strategy number -> GDD</param>
        private void SelectTargetsForMember(List<GameObject> troop, int strategy)
        {
            if (strategy < 1 || strategy > 6)
            {
                mStrategyNumber = StrategySelection();              // selects strategy
            }
            if (troop == null || troop.Count == 0)
            {
                mDifficultyValue = 5;
                PlaceAi();
                troop = CreateTroop(mStrategyNumber);        // creates troop
            }

            // case 1:
            // players outside the base are unimportant, if spaceship is nearly finished
            if (mObjectsOutsideBase.Count > 0 && !mIgnoreObjectsOutsideBase)
            {
                mIsObjectOutsideBase = true;

                foreach (var gameObject in troop)
                {
                    var index = mRand.Next(0, mObjectsOutsideBase.Count);
                    gameObject.mObjectTargetList.Add(mObjectsOutsideBase[index]);

                    AddAttackerToFlockList(gameObject, mObjectsOutsideBase[index]);
                    GameLoop.ObjectManager.ActionManager.Fight.FightObject(gameObject,
                        mObjectsOutsideBase[index]);
                }
            }

            switch (strategy)
            {
                case 2: // destroy parts of wall in groups
                {
                    for (var fourGroup = 0; fourGroup < troop.Count / 4 + 1; fourGroup++) // group size = 4
                    {
                        var wallIndex = mRand.Next(0, mWalls.Count - 1);
                        for (var member = 0; 4 * fourGroup + member < troop.Count; member++)
                        {
                            if (member < 2) // first troop
                            {
                                troop[4 * fourGroup + member].mObjectTargetList.Add(mWalls[wallIndex]);

                                AddAttackerToFlockList(troop[4 * fourGroup + member], mWalls[wallIndex]);

                                if (!mIsObjectOutsideBase) // no objects outside the base
                                {
                                    GameLoop.ObjectManager.ActionManager.Fight.FightObject(troop[4 * fourGroup + member],
                                        mWalls[wallIndex]);
                                }
                            }
                            else
                            {
                                // second troop
                                troop[4 * fourGroup + member].mObjectTargetList.Add(mWalls[wallIndex + 1]);

                                if (!mIsObjectOutsideBase) // no objects outside the base
                                {
                                    AddAttackerToFlockList(troop[4 * fourGroup + member], mWalls[wallIndex + 1]);
                                }
                                else
                                {
                                    GameLoop.ObjectManager.ActionManager.Fight.FightObject(troop[4 * fourGroup + member],
                                        mWalls[wallIndex + 1]);
                                }
                            }
                        }
                    }
                    break;
                }
                case 3: // kill damaged players
                {
                    foreach (var gameObject in troop)
                    {
                        var index = mRand.Next(0, mDamagedObjects.Count);
                        gameObject.mObjectTargetList.Add(mDamagedObjects[index]);

                        AddAttackerToFlockList(gameObject, mDamagedObjects[index]);

                        if (!mIsObjectOutsideBase)
                        {
                            GameLoop.ObjectManager.ActionManager.Fight.FightObject(gameObject, mDamagedObjects[index]);
                        }
                    }
                    break;
                }
                case 4: // kill random players
                {
                    foreach (var gameObject in troop)
                    {
                        var index = mRand.Next(0, mAllPlayers.Count);
                        gameObject.mObjectTargetList.Add(mAllPlayers[index]);

                        if (mIsObjectOutsideBase)
                        {
                            AddAttackerToFlockList(gameObject, mAllPlayers[index]);
                        }
                        else
                        {
                            GameLoop.ObjectManager.ActionManager.Fight.FightObject(gameObject, mAllPlayers[index]);
                        }
                    } 
                    break;
                }
                case 5: // destroy spaceship
                {
                    foreach (var gameObject in troop)
                    {
                        AddAttackerToFlockList(gameObject, GameLoop.ObjectManager.DataManager.Spaceship);
                        gameObject.mObjectTargetList.Add(GameLoop.ObjectManager.DataManager.Spaceship);
                        GameLoop.ObjectManager.ActionManager.Fight.FightObject(gameObject,
                            GameLoop.ObjectManager.DataManager.Spaceship);
                    }

                    return;
                    }
                case 6: // destroy spaceship
                { 
                    foreach (var gameObject in troop)
                    {
                        AddAttackerToFlockList(gameObject, GameLoop.ObjectManager.DataManager.Spaceship);
                        gameObject.mObjectTargetList.Add(GameLoop.ObjectManager.DataManager.Spaceship); 
                        GameLoop.ObjectManager.ActionManager.Fight.FightObject(gameObject, 
                            GameLoop.ObjectManager.DataManager.Spaceship);
                    }

                    return;
                }
            }
            
            foreach (var gameObject in troop)   // add spaceship -> last target for KI
            {
                gameObject.mObjectTargetList.Add(GameLoop.ObjectManager.DataManager.Spaceship);
            }
        }

        private void AddAttackerToFlockList(GameObject attacker, GameObject target)
        {
            foreach (var targetList in mTargetsAttackersFlockList)
            {
                if (target == targetList[0] && !targetList.Contains(attacker)) // target is in target list
                {
                    targetList.Add(attacker);
                    break;
                }

                // target is not in target flock list
                mTargetsAttackersFlockList.Add(new List<GameObject> { target, attacker });
                break;
            }

            // target flock list is empty -> add target and flock
            mTargetsAttackersFlockList.Add(new List<GameObject> { target, attacker });
        }
    }
}