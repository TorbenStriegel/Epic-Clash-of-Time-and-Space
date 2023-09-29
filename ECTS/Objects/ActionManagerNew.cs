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
    public sealed class ActionManagerNew
    {
        private GameLoop GameLoop { get; }
        private Harvest Harvest { get; }
        private Repair Repair { get; }
        private Destroy Destroy { get; }
        private Fight Fight { get; }
        private Approach Approach { get; }


        private Vector2 mVectorObjectToTarget;      // vector from acting object to target object position (used for face direction)


        /// <summary>
        /// Manages object actions (like harvesting, fighting...)
        /// </summary>
        public ActionManagerNew(GameLoop gameLoop)
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
                var targetObject =
                    GameLoop.ObjectManager.DataManager.mUnits.GetNearestGameObject(actingObject.ActionArea, actingObject);

                // ACT on objects
                if (targetObject != null)
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
                        }
                    }
                } // if there are no interacting objects become idle.
                else
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
            if (actingObject.State == GameObject.ObjectState.Moving)
            {
                return;
            }

            // FIND objects within action area
            var targetObject =
                GameLoop.ObjectManager.DataManager.mUnits.GetNearestGameObject(actingObject.ActionArea, actingObject);
            // ACT on objects
            if (targetObject != null)
            {
                if (actingObject.mTargetObjects.Count > 0 &&
                    actingObject.mTargetObjects.Contains(targetObject) &&
                    (targetObject.Type == GameObject.ObjectType.Spaceship || targetObject.Type == GameObject.ObjectType.Wall))
                {
                    FaceTarget(actingObject, targetObject);
                    Repair.RepairObject(actingObject, targetObject);
                }
                // become Idle if there is no object which can be repaired.
                actingObject.State = GameObject.ObjectState.Idle;
            }// if there are no interacting objects become idle.
            else 
            {
                actingObject.State = GameObject.ObjectState.Idle;
            }
        }

        /// <summary>
        /// Determine actions for a Fighter (distant and close).
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void FighterAction(GameObject actingObject)
        {
            if (actingObject.State == GameObject.ObjectState.Moving)
            {
                return;
            }

            // FIND ENEMIES within action area
            var targetObject = GameLoop.ObjectManager.DataManager.mUnits.GetNearestGameObjectByType(actingObject.ActionArea,
                actingObject,
                GameObject.ObjectType.Enemy);
            if (targetObject != null)
            {
                // Fighting
                if (targetObject.Health > 0)
                {
                    actingObject.mCurrentTargetObject = targetObject;
                    FaceTarget(actingObject, targetObject);
                    actingObject.State = GameObject.ObjectState.Fighting;
                    // Shooting is managed by Fighter Object itself because it has to be coordinated with animation.
                }
                actingObject.State = GameObject.ObjectState.Idle;
            }// if there are no enemies become idle.
            else 
            {
                actingObject.State = GameObject.ObjectState.Idle;
            }
        }

        private void DnaAction(GameObject actingObject)
        {
            // FIND OBJECTS within action area
            var targetObject = GameLoop.ObjectManager.DataManager.mUnits.GetNearestGameObjectByType(actingObject.ActionArea, actingObject, GameObject.ObjectType.Player);
            if (targetObject == null)
            {
                return;
            }
            //UPGRADE Level and other Properties depending on interacting player character

            GameLoop.RenderManager.SoundManager.PlaySound("dna_collect");
            targetObject.Health = 100;

            if (targetObject.Level < 2)
            {
                targetObject.Level += 1;
                targetObject.Speed += 2;
                targetObject.AttackStrength *= 2;
                targetObject.DefenseStrength *= 2;
                targetObject.ChangeFace("Front");
            }
            actingObject.ObjectAge = actingObject.ObjectLifetime;
            actingObject.IsActing = false;
            actingObject.IsInteracting = false;
            actingObject.ChangeFace("Dead");
        }

        /// <summary>
        /// Determine actions for a Bullet1 object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void BulletAction(GameObject actingObject)
        {
            // FIND OBJECTS within action area
            var targetObject = GameLoop.ObjectManager.DataManager.mUnits.GetNearestGameObjectByType(actingObject.ActionArea, actingObject, GameObject.ObjectType.Enemy);
            if (targetObject == null)
            {
                return;
            }

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

        /// <summary>
        /// Determine actions for a Bullet1 object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void ExplosionAction(GameObject actingObject)
        {
            // FIND OBJECTS within action area
            var interactingObjects = GameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(actingObject.ActionArea);
            if (interactingObjects.Count <= 0)
            {
                return;
            }

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

                if (actingObject is GrenadeEnemyExplosion && (targetObject.Type == GameObject.ObjectType.Player
                                                              || targetObject.Type == GameObject.ObjectType.Spaceship) &&
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

        /// <summary>
        /// Determine actions for an enemy.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        private void EnemyAction(GameObject actingObject)
        {
            // Destroy one primary Target from MainTargetList after each other
            // Enemy interrupt and defend themselves when attacked.

            // IF UNDER ATTACK defend yourself against all objects within your range.
            if (actingObject.UnderAttack && actingObject.State != GameObject.ObjectState.Moving)
            {
                if (actingObject.AttackingUnit != null)
                {
                    FightAttackingUnit(actingObject, actingObject.AttackingUnit);

                }

            } // If Idle, check for targets in mainTargetList
            else if (actingObject.State == GameObject.ObjectState.Idle)
            {
                if (actingObject.mObjectTargetList.Count > 0)
                {
                    FightDestroyInteractingObjects(actingObject, actingObject.mObjectTargetList);
                }
                else
                {// FIND Relevant Objects within action area Todo: Simplify this with new function from Jan?
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
        }

        /// <summary>
        /// Approaches attacking player unit (if possible) and fights it.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// /// <param name="attackingUnit">Attacking player unit</param>
        private void FightAttackingUnit(GameObject actingObject, GameObject attackingUnit)
        {
            // FIGHTING
            if (attackingUnit.Health > 0)
            {
                FaceTarget(actingObject, attackingUnit);

                if (actingObject.StrikingDistance <
                    ObjectManager.ObjectDistance(actingObject, attackingUnit))
                {
                    Approach.ApproachObject(actingObject, attackingUnit);
                }
                else
                {
                    actingObject.mCurrentTargetObject = attackingUnit;
                    actingObject.State = GameObject.ObjectState.Fighting;
                }
            }
        }

        /// <summary>
        /// Fights or destroys closest object depending on object type.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// /// <param name="interactingObjects">interacting objects which have to be fought</param>
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
            }
        }

        /// <summary>
        /// Changes face of acting Object so that it is facing the target object.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// /// <param name="targetObject">GameObject on which actions are performed</param>
        private void FaceTarget(GameObject actingObject, GameObject targetObject)
        {
            mVectorObjectToTarget = new Vector2(targetObject.Position.X - actingObject.Position.X,
                targetObject.Position.Y - actingObject.Position.Y);

            actingObject.ChangeFace(Pathfinder.Pathfinder.FaceDirection(mVectorObjectToTarget));
        }
    }


}
