using System;
using System.Collections.Generic;
using ECTS.Data;
using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.ObjectActions
{
    public sealed class Repair
    {
        /// <summary>
        /// Functions for repairing a targetObject.
        /// </summary>
        private GameLoop GameLoop { get; }
        public Repair(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }
        /// <summary>
        /// Repair target object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Object being hit.</param>
        /// // Resources: {Metal, Stone, Wood, Food}
        public void RepairObject(GameObject actingObject, GameObject targetObject)
        {
            if (targetObject.Health < 100)
            {
                
                if (targetObject.Type == GameObject.ObjectType.Wall && GameLoop.ObjectManager.DataManager.ResourceList[1] > 0
                                                                    && GameLoop.ObjectManager.DataManager.ResourceList[2] > 0)
                {
                    // MOVE ENGINEER AWAY FROM WALL (so that he doesn't get built in)
                    // Get Vector from Wall to center of Spaceship, normalize.

                    if (actingObject.mLastObjectState != GameObject.ObjectState.Repairing)
                    {
                        var r = new Random();
                        var spaceshipDirection =
                            Vector2.Subtract(targetObject.ObjectCenter, GameLoop.ObjectManager.DataManager.Spaceship.ObjectCenter);
                        spaceshipDirection.Normalize();
                        // Move engineer to the inner side of the wall.
                        actingObject.ObjectCenter = targetObject.ObjectCenter - 40 * spaceshipDirection;
                        // shift by random value
                        var actingObjectPosition = actingObject.Position;
                        actingObjectPosition.X += r.Next(-3, 3);
                        actingObjectPosition.Y += r.Next(-3, 3);
                        actingObject.Position = actingObjectPosition;
                    }
                    
                    actingObject.State = GameObject.ObjectState.Repairing;
                    actingObject.mLastObjectState = GameObject.ObjectState.Repairing;
                    targetObject.Health += actingObject.AttackStrength / 100;

                    GameLoop.ObjectManager.DataManager.ResourceList[1] -= actingObject.AttackStrength / 100f;
                    GameLoop.ObjectManager.DataManager.ResourceList[2] -= actingObject.AttackStrength / 100f;
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameLoop.ObjectManager.DataManager.ResourceList[i] < 0)
                        {
                            GameLoop.ObjectManager.DataManager.ResourceList[i] = 0;
                        }
                    }

                    // Become idle if one (or all) resource(s) is (are) empty
                    if (GameLoop.ObjectManager.DataManager.ResourceList[1] <= 0
                        || GameLoop.ObjectManager.DataManager.ResourceList[2] <= 0)
                    {
                        actingObject.State = GameObject.ObjectState.Idle;
                        actingObject.mTargetObjects = new List<GameObject>();
                        actingObject.mLastObjectState = GameObject.ObjectState.Idle;
                    }
                    return;
                }
                if (targetObject.Type == GameObject.ObjectType.Spaceship && GameLoop.ObjectManager.DataManager.ResourceList[0] > 0
                                                                              && GameLoop.ObjectManager.DataManager.ResourceList[1] > 0
                                                                              && GameLoop.ObjectManager.DataManager.ResourceList[2] > 0)
                {
                    actingObject.State = GameObject.ObjectState.Repairing;
                    var addedHealth = actingObject.AttackStrength / 10000f;
                    targetObject.Health += addedHealth;
                    GameLoop.ObjectManager.DataManager.mSpaceshipRepaired += addedHealth;
                    GameLoop.Settings.mSpaceshipRepaired += addedHealth;

                    // Needed resources depending on difficulty
                    switch (GameLoop.ObjectManager.DataManager.Difficulty)
                    {
                        case DataManager.GameDifficulty.Easy:
                            GameLoop.ObjectManager.DataManager.ResourceList[0] -= addedHealth * 40;
                            GameLoop.ObjectManager.DataManager.ResourceList[1] -= addedHealth * 40;
                            GameLoop.ObjectManager.DataManager.ResourceList[2] -= addedHealth * 40;
                            break;
                        case DataManager.GameDifficulty.Medium:
                            GameLoop.ObjectManager.DataManager.ResourceList[0] -= addedHealth * 60;
                            GameLoop.ObjectManager.DataManager.ResourceList[1] -= addedHealth * 60;
                            GameLoop.ObjectManager.DataManager.ResourceList[2] -= addedHealth * 60;
                            break;
                        case DataManager.GameDifficulty.Hard:
                            GameLoop.ObjectManager.DataManager.ResourceList[0] -= addedHealth * 100;
                            GameLoop.ObjectManager.DataManager.ResourceList[1] -= addedHealth * 100;
                            GameLoop.ObjectManager.DataManager.ResourceList[2] -= addedHealth * 100;
                            break;
                        default:
                            GameLoop.ObjectManager.DataManager.ResourceList[0] -= addedHealth * 100;
                            GameLoop.ObjectManager.DataManager.ResourceList[1] -= addedHealth * 100;
                            GameLoop.ObjectManager.DataManager.ResourceList[2] -= addedHealth * 100;
                            break;
                    }
                    
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameLoop.ObjectManager.DataManager.ResourceList[i] < 0)
                        {
                            GameLoop.ObjectManager.DataManager.ResourceList[i] = 0;
                        }
                    }

                    // Become idle if one (or all) resource(s) is (are) empty
                    if (GameLoop.ObjectManager.DataManager.ResourceList[0] <= 0
                        || GameLoop.ObjectManager.DataManager.ResourceList[1] <= 0
                        || GameLoop.ObjectManager.DataManager.ResourceList[2] <= 0)
                    {
                        actingObject.State = GameObject.ObjectState.Idle;
                        actingObject.mTargetObjects = new List<GameObject>();
                        actingObject.mLastObjectState = GameObject.ObjectState.Idle;
                    }
                    return;
                }
                // if repairing is finished for this object, set active object to idle
                actingObject.State = GameObject.ObjectState.Idle;
                
                
            }
        }

    }
}
