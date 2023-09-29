using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.controllable_units;
using ECTS.Objects.GameObjects.weapons;
using ECTS.Objects.ObjectActions;
using Microsoft.Xna.Framework;

namespace ECTS.Objects
{
    [DataContract]
    public sealed class ActionManager
    {
        private readonly Random mRand = new Random();
        private GameLoop GameLoop { get; }
        private Harvest Harvest { get; }
        private Repair Repair { get; }
        public Destroy Destroy { get; }
        public Fight Fight { get; }
        public Approach Approach { get; }


        private Vector2 mVectorObjectToTarget;      // vector from acting object to target object position (used for face direction)


        /// <summary>
        /// Manages object actions (like harvesting, fighting...)
        /// </summary>
        public ActionManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;

            Harvest = new Harvest(GameLoop);
            Repair = new Repair(GameLoop);
            Fight = new Fight(GameLoop);
            Destroy = new Destroy();
            Approach = new Approach(GameLoop);


        }

        /// <summary>
        /// Select action for an acting object. Determine what this object does with a given list of interacting objects.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        public void ActionSelector(GameObject actingObject)
        {

            if (actingObject is Worker)
            {
                WorkerAction(actingObject);
            }
            if (actingObject is Engineer)
            {
                EngineerAction(actingObject);
            }
            if (actingObject is FighterClose || actingObject is FighterDistance)
            {
                FighterAction(actingObject);
            }
            if (actingObject.Type == GameObject.ObjectType.Enemy)
            {
                EnemyAction(actingObject);
            }
            if (actingObject.Type == GameObject.ObjectType.Bullet)
            {
                BulletAction(actingObject);
            }
            if (actingObject is GrenadePlayerExplosion || actingObject is GrenadeEnemyExplosion)
            {
                ExplosionAction(actingObject);
            }
            if (actingObject.Type == GameObject.ObjectType.Dna)
            {
                DnaAction(actingObject);
            }

        }

        // ACTIONS FOR SPECIFIC OBJECT TYPES

        /// <summary>
        /// Determine actions for a worker.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void WorkerAction(GameObject actingObject)
        {
            if (actingObject.State != GameObject.ObjectState.Moving)
            {
                // FIND objects within action area
                var interactingObjects =
                    GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(actingObject.ActionArea);

                // ACT on objects
                if (interactingObjects.Count > 0)
                {
                    // SORT objects by distance
                    foreach (var interactingObject in interactingObjects)
                    {
                        interactingObject.SortingDistance =
                            ObjectManager.ObjectDistance(actingObject, interactingObject);
                    }

                    interactingObjects.Sort((x, y) => x.SortingDistance.CompareTo(y.SortingDistance));

                    // ACT on closest object
                    foreach (GameObject targetObject in interactingObjects)  // Loop -> freeze
                    {
                        // HARVESTING
                        if ((targetObject.Type == GameObject.ObjectType.Tree ||
                            targetObject.Type == GameObject.ObjectType.Stone ||
                            targetObject.Type == GameObject.ObjectType.Metal ||
                            targetObject.Type == GameObject.ObjectType.Food) && targetObject.IsInteracting)
                        {
                            // approach closest object if out of striking (=harvesting) distance
                            if (actingObject.StrikingDistance <
                                ObjectManager.ObjectDistance(actingObject, targetObject))
                            {
                                Approach.ApproachObject(actingObject, targetObject);
                            }
                            else
                            { // harvest if close enough
                                FaceTarget(actingObject, targetObject);
                                actingObject.State = GameObject.ObjectState.Harvesting;
                                if (actingObject.mLastObjectState != GameObject.ObjectState.Harvesting)
                                {
                                    GameLoop.RenderManager.SoundManager.PlaySound("hammering");
                                    actingObject.mLastObjectState = actingObject.State;
                                }
                                Harvest.HarvestObject(actingObject, targetObject);
                                break; // only harvest one object at a time.
                            }
                        }
                    }
                } // if there are no interacting objects become idle.
                else if (interactingObjects.Count == 0)
                {
                    actingObject.State = GameObject.ObjectState.Idle;
                }
            }
            actingObject.mLastObjectState = actingObject.State;
        }

        /// <summary>
        /// Determine actions for an engineer.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void EngineerAction(GameObject actingObject)
        {
            if (actingObject.State != GameObject.ObjectState.Moving)
            {
                // FIND objects within action area
                var interactingObjects =
                    GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(actingObject.ActionArea);
                // ACT on objects
                if (interactingObjects.Count > 0)
                {
                    // SORT objects by distance
                    foreach (var interactingObject in interactingObjects)
                    {
                        interactingObject.SortingDistance =
                            ObjectManager.ObjectDistance(actingObject, interactingObject);
                    }
                    interactingObjects.Sort((x, y) => x.SortingDistance.CompareTo(y.SortingDistance));

                    // REPAIR closest Wall or Spaceship (if defined as target before)
                    foreach (GameObject targetObject in interactingObjects)
                    {
                        if (actingObject.mTargetObjects.Count > 0 &&
                            actingObject.mTargetObjects.Contains(targetObject) &&
                            (targetObject.Type == GameObject.ObjectType.Spaceship || targetObject.Type == GameObject.ObjectType.Wall))
                        {
                            FaceTarget(actingObject, targetObject);
                            Repair.RepairObject(actingObject, targetObject);
                            break;
                        }
                        // become Idle if there is no object which can be repaired.
                        actingObject.State = GameObject.ObjectState.Idle;
                    }
                }// if there are no interacting objects become idle.
                else if (interactingObjects.Count == 0)
                {
                    actingObject.State = GameObject.ObjectState.Idle;
                }
            }
        }

        /// <summary>
        /// Determine actions for a Fighter (distant and close).
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void FighterAction(GameObject actingObject)
        {
            if (actingObject.State != GameObject.ObjectState.Moving)
            {
                // FIND ENEMIES within action area
                var interactingObjects = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangleByType(actingObject.ActionArea,
                    GameObject.ObjectType.Enemy);
                if (interactingObjects.Count > 0)
                {
                    // SORT enemies by distance
                    foreach (var interactingObject in interactingObjects)
                    {
                        interactingObject.SortingDistance =
                            ObjectManager.ObjectDistance(actingObject, interactingObject);
                    }
                    interactingObjects.Sort((x, y) => x.SortingDistance.CompareTo(y.SortingDistance));

                    // FIGHT closest enemy
                    foreach (GameObject targetObject in interactingObjects)
                    {
                        // Fighting
                        if (targetObject.Health > 0)
                        {
                            actingObject.mCurrentTargetObject = targetObject;
                            FaceTarget(actingObject, targetObject);
                            actingObject.State = GameObject.ObjectState.Fighting;
                            // Shooting is managed by Fighter Object itself because it has to be coordinated with animation.
                            break;
                        }
                        actingObject.State = GameObject.ObjectState.Idle;
                    }
                }// if there are no enemies become idle.
                else if (interactingObjects.Count == 0)
                {
                    actingObject.State = GameObject.ObjectState.Idle;
                }
            }
        }

        private void DnaAction(GameObject actingObject)
        {
            // FIND OBJECTS within action area
            var interactingObjects = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangleByType(actingObject.ActionArea, GameObject.ObjectType.Player);
            if (interactingObjects.Count > 0)
            {
                foreach (GameObject targetObject in interactingObjects)
                {
                    //UPGRADE Level and other Properties depending on interacting player character
                    if (targetObject.Type == GameObject.ObjectType.Player)
                    {
                        targetObject.Health = 100;
                        if (targetObject.Level >= 2)
                        {
                            GameLoop.ObjectManager.DataManager.mWastedDna += 1;
                            GameLoop.RenderManager.SoundManager.PlaySound("dna_wasted");
                        }
                        else
                        {
                            targetObject.Level += 1;
                            targetObject.Speed += 1;
                            targetObject.AttackStrength *= 1.5f;
                            targetObject.DefenseStrength *= 2;
                            targetObject.ChangeFace("Front");
                            GameLoop.ObjectManager.DataManager.mWastedDna = 0;
                            GameLoop.RenderManager.SoundManager.PlaySound("dna_collect");
                        }

                        GameLoop.ObjectManager.DataManager.mCollectedDna += 1;
                        GameLoop.Settings.mCollectedDna += 1;

                        actingObject.ObjectAge = actingObject.ObjectLifetime;
                        actingObject.IsActing = false;
                        actingObject.IsInteracting = false;
                        actingObject.ChangeFace("Dead");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determine actions for a Bullet1 object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void BulletAction(GameObject actingObject)
        {
            // FIND OBJECTS within action area
            var interactingObjects = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(actingObject.ActionArea);
            
            if (interactingObjects.Count > 0)
            {
               foreach (GameObject targetObject in interactingObjects)
               {
                    // Destroy Target
                    if (targetObject.Type == GameObject.ObjectType.Enemy && targetObject.Health > 0)
                    {
                        targetObject.Health -= 10 / targetObject.DefenseStrength * actingObject.AttackStrength;
                        targetObject.UnderAttack = true;
                        targetObject.TimeSinceAttack = 0;
                        targetObject.AttackingUnit = actingObject.AttackingUnit; // communicate attacking object (Player)
                        actingObject.ObjectAge = actingObject.ObjectLifetime;
                        actingObject.IsActing = false;
                        actingObject.IsInteracting = false;
                        actingObject.ChangeFace("Dead");
                    }
                    if (targetObject.Type == GameObject.ObjectType.Wall && targetObject.Health > 0)
                    {
                        actingObject.ObjectAge = actingObject.ObjectLifetime;
                    }
               }
            }
        }

        /// <summary>
        /// Determine actions for a Bullet1 object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void ExplosionAction(GameObject actingObject)
        {
            // FIND OBJECTS within action area
            var interactingObjects = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(actingObject.ActionArea);
            if (interactingObjects.Count > 0)
            {
                foreach (GameObject targetObject in interactingObjects)
                {
                    // Damage Target
                    if (actingObject is GrenadePlayerExplosion && targetObject.Type == GameObject.ObjectType.Enemy &&
                        targetObject.Health > 0)
                    {
                        targetObject.Health -= 10 / targetObject.DefenseStrength * actingObject.AttackStrength;
                        targetObject.UnderAttack = true;
                        targetObject.AttackingUnit = actingObject.AttackingUnit;
                        targetObject.TimeSinceAttack = 0;
                    }

                    if (actingObject is GrenadeEnemyExplosion && (targetObject.Type == GameObject.ObjectType.Player || 
                                                                  targetObject.Type == GameObject.ObjectType.Wall || 
                                                                  targetObject.Type == GameObject.ObjectType.Spaceship) &&
                        targetObject.Health > 0)
                    {
                        targetObject.Health -= 10 / targetObject.DefenseStrength * actingObject.AttackStrength;
                        targetObject.UnderAttack = true;
                        targetObject.TimeSinceAttack = 0;
                    }
                    // Each grenade only acts once on all objects within its range.
                    actingObject.IsActing = false;
                }
            }
        }

        /// <summary>
        /// Determine actions for an enemy. 
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void EnemyAction(GameObject actingObject)
        {
            // Destroy one primary Target from MainTargetList after each other
            // If Monsters are attacked, they always attack back (if enemy can be reached).

            // IF UNDER ATTACK defend yourself against all objects within your range.

            // 1) ATTACKING Unit is within striking distance
            if (actingObject.UnderAttack && actingObject.State != GameObject.ObjectState.Fighting
                                         && actingObject.AttackingUnit != null
                                         && actingObject.AttackingUnit.Health > 0 // does attacking object still have health?
                                        && actingObject.StrikingDistance > ObjectManager.ObjectDistance(actingObject, actingObject.AttackingUnit)) 
            {
                
                actingObject.mCurrentTargetObject = actingObject.AttackingUnit;
                FaceTarget(actingObject, actingObject.mCurrentTargetObject);
                actingObject.State = GameObject.ObjectState.Fighting;
                return;
                //2) ATTACKING UNIT out of reach
            }

            if (actingObject.UnderAttack && actingObject.State != GameObject.ObjectState.Fighting
                && actingObject.AttackingUnit != null
                && actingObject.AttackingUnit.Health > 0  // does attacking object still have health?
                && actingObject.StrikingDistance < ObjectManager.ObjectDistance(actingObject, actingObject.AttackingUnit)
            ) 
            {
                // APPROACH if all walls and gates are closed.
                if (GameLoop.ObjectManager.mOpenWallGateList != null && GameLoop.ObjectManager.mOpenWallGateList.Count > 0)
                {
                    actingObject.mCurrentTargetObject = actingObject.AttackingUnit;
                    Approach.ApproachObject(actingObject, actingObject.mCurrentTargetObject);
                    return;
                }
            }

            // Do nothing if not idle or cases above are false. 
            if (actingObject.State != GameObject.ObjectState.Idle)
            {
                return;
            }
            // TARGET LIST NOT EMPTY, NOT UNDER ATTACK
            if (!actingObject.UnderAttack && actingObject.mObjectTargetList.Count > 0)
            {
                FightDestroyInteractingObjects(actingObject, actingObject.mObjectTargetList);
            }
            // TARGET LIST EMPTY, NOT UNDER ATTACK
            else
            {// FIND Relevant Objects within action area 
                var interactingObjects = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(actingObject.ActionArea);
                if (interactingObjects.Count > 0)
                {
                    // SORT enemies by distance
                    foreach (var interactingObject in interactingObjects)
                    {
                        interactingObject.SortingDistance =
                            ObjectManager.ObjectDistance(actingObject, interactingObject);
                    }
                    interactingObjects.Sort((x, y) => x.SortingDistance.CompareTo(y.SortingDistance));

                    FightDestroyInteractingObjects(actingObject, interactingObjects);
                }
            }
        }

        /// <summary>
        /// Fights or destroys closest object depending on object type.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="interactingObjects">interacting objects which have to be fought</param>
        private void FightDestroyInteractingObjects(GameObject actingObject, List<GameObject> interactingObjects)
        {
            foreach (GameObject targetObject in interactingObjects)
            {

                // FIGHTING
                if (targetObject.Type == GameObject.ObjectType.Player && targetObject.Health > 0)
                {
                    FaceTarget(actingObject, targetObject);

                    if (actingObject.StrikingDistance <
                        ObjectManager.ObjectDistance(actingObject, targetObject))
                    {
                        if (Approach.ApproachObject(actingObject, targetObject) == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        actingObject.mCurrentTargetObject = targetObject;
                        actingObject.State = GameObject.ObjectState.Fighting;
                    }

                    break;
                }
                // DESTROYING
                if ((targetObject.Type == GameObject.ObjectType.Wall ||
                     targetObject.Type == GameObject.ObjectType.Spaceship) && targetObject.Health > 0)
                {
                    FaceTarget(actingObject, targetObject);

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
                        if (Approach.ApproachObject(actingObject, targetObject) == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        actingObject.mCurrentTargetObject = targetObject;
                        actingObject.State = GameObject.ObjectState.Destroying;
                    }
                    break;
                }

                actingObject.State = GameObject.ObjectState.Idle;
            }
        }

        /// <summary>
        /// Changes face of acting Object so that it is facing the target object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">GameObject on which actions are performed</param>
        private void FaceTarget(GameObject actingObject, GameObject targetObject)
        {
            mVectorObjectToTarget = new Vector2(targetObject.Position.X - actingObject.Position.X,
                targetObject.Position.Y - actingObject.Position.Y);

            actingObject.ChangeFace(Pathfinder.Pathfinder.FaceDirection(mVectorObjectToTarget));
        }
    }


}