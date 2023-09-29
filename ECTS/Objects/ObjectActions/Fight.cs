using System;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.weapons;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.ObjectActions
{
    /// <summary>
    /// Functions for fighting a targetObject.
    /// </summary>
    public sealed class Fight
    {
        private GameLoop GameLoop { get; }
        private Vector2 mShootingDirection; // Direction for shooting
        private readonly Random mRand = new Random();

        public Fight(GameLoop gameLoop)
        {
            GameLoop = gameLoop;

        }

        /// <summary>
        /// Send actingObject to targetObject (using Pathfinder) and start fight or destruction when it is reached.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Object being hit.</param>
        public void FightObject(GameObject actingObject, GameObject targetObject)
        {
            if (targetObject.Health > 0)
            {
                if (GameLoop.AiManager.mStrategyNumber == 6) // destroy spaceship strategy
                {
                    if (actingObject.StrikingDistance <
                        ObjectManager.ObjectDistance(actingObject, targetObject))
                    { // cant reach spaceship
                        actingObject.mCurrentTargetObject = targetObject;
                        GameLoop.ObjectManager.ActionManager.Approach.ApproachObject(actingObject,
                            GameLoop.AiManager.mWalls[mRand.Next(0, GameLoop.AiManager.mWalls.Count)]);
                    }
                    else
                    {
                        actingObject.mCurrentTargetObject = targetObject;
                        actingObject.State = GameObject.ObjectState.Fighting;
                    }
                }

                if (actingObject.StrikingDistance <
                    ObjectManager.ObjectDistance(actingObject, targetObject))
                {
                    actingObject.mCurrentTargetObject = targetObject;
                    GameLoop.ObjectManager.ActionManager.Approach.ApproachObject(actingObject, targetObject);
                }
                else
                {
                    actingObject.mCurrentTargetObject = targetObject;
                    actingObject.State = GameObject.ObjectState.Fighting;
                }
            }
        }

        /// <summary>
        /// Destroy object with axe stroke. Reduces target object health with each call.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Object being hit.</param>
        public void AxeStroke(GameObject actingObject, GameObject targetObject)
        {
            if (targetObject.Health > 0f)
            {
                targetObject.Health -= 10 / targetObject.DefenseStrength * actingObject.AttackStrength;
            }
            else
            {
                actingObject.State = GameObject.ObjectState.Idle;
            }
        }

        /// <summary>
        /// Shoot Bullet object in the direction of target Object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Object being hit.</param>
        public void BulletShot(GameObject actingObject, GameObject targetObject)
        {
            mShootingDirection.X = targetObject.ObjectCenter.X - actingObject.GunMuzzle.X;
            mShootingDirection.Y = targetObject.ObjectCenter.Y - actingObject.GunMuzzle.Y;
            mShootingDirection.Normalize();
            GameLoop.ObjectManager.mTempTree.Add(new BulletPlayer(GameLoop)
            {
                Position = new Rectangle((int) actingObject.GunMuzzle.X, (int) actingObject.GunMuzzle.Y, 15, 15),
                mFlightDirection = mShootingDirection,
                AttackStrength = actingObject.AttackStrength,
                AttackingUnit = actingObject
            });
        }

        /// <summary>
        /// Shoot Grenade object in the direction of target Object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Object being hit.</param>
        public void GrenadeShot(GameObject actingObject, GameObject targetObject)
        {
            mShootingDirection.X = targetObject.ObjectCenter.X - actingObject.GunMuzzle.X;
            mShootingDirection.Y = targetObject.ObjectCenter.Y - actingObject.GunMuzzle.Y;
            mShootingDirection.Normalize();

            if (actingObject.Type == GameObject.ObjectType.Player)
            {
                GameLoop.ObjectManager.mTempTree.Add(new GrenadePlayer(GameLoop)
                {
                    Position = new Rectangle((int)actingObject.GunMuzzle.X, (int)actingObject.GunMuzzle.Y, 15, 15),
                    IsActing = false,
                    mFlightDirection = mShootingDirection,
                    AttackStrength = actingObject.AttackStrength,
                    mStartingPoint = actingObject.ObjectCenter,
                    mTravelDistance = Vector2.Distance(actingObject.ObjectCenter, targetObject.ObjectCenter),
                    AttackingUnit = actingObject
                });
            }
            if (actingObject.Type == GameObject.ObjectType.Enemy)
            {
                GameLoop.ObjectManager.mTempTree.Add(new GrenadeEnemy(GameLoop)
                {
                    Position = new Rectangle((int)actingObject.GunMuzzle.X, (int)actingObject.GunMuzzle.Y, 15, 15),
                    IsActing = false,
                    mFlightDirection = mShootingDirection,
                    AttackStrength = actingObject.AttackStrength,
                    mStartingPoint = actingObject.ObjectCenter,
                    mTravelDistance = Vector2.Distance(actingObject.ObjectCenter, targetObject.ObjectCenter)
                });
            }
        }
    }
}
