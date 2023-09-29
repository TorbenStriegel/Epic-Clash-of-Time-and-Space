using System;
using System.Collections.Generic;
using System.Linq;
using ECTS.Data;
using ECTS.Objects.GameObjects;
using ECTS.Objects.GameObjects.controllable_units;
using ECTS.Objects.GameObjects.uncontrollable_units.spaceship;
using Microsoft.Xna.Framework;

namespace ECTS.Pathfinder
{
    /// <summary>
    /// Responsible for Movement-calculation and -execution
    /// </summary>
    internal sealed class Pathfinder
    {
        private readonly GameLoop mGameLoop;
        internal AStar AStar { get; }
        internal Flocking Flocking { get; }

        internal Pathfinder(GameLoop gameLoop, DataManager dataManager)
        {
            mGameLoop = gameLoop;
            AStar = new AStar(12f, new Vector2(4800f, 4800f), dataManager.mUnits);
            Flocking = new Flocking(mGameLoop);
        }

        /// <summary>
        /// Initiates Movement Procedure for given GameObject-List and sets all needed parameters.
        /// </summary>
        /// <param name="markedGameObjects"></param>
        /// <param name="mousePosition"></param>
        /// <returns>true, if valid movement path was found</returns>
        internal bool StartMovement(List<GameObject> markedGameObjects, Vector2 mousePosition)
        {
            var massCenter = CentreOfMass(markedGameObjects);
            var queue = new Queue<Vector2>(AStar.FindPath(massCenter, mousePosition));
            if (queue.Count == 0)
            {
                return false;
            }

            var flock = new Flock
            {
                mPathQueue = queue,
                mFlockObjects = new HashSet<GameObject>(markedGameObjects),
                mTarget = mousePosition
            };

            foreach (var gameObject in markedGameObjects)  // Update gameObjects State and mFlock
            {
                if (gameObject.Speed == 0)  // Test to make sure markedGameObjects are moveable
                {
                    flock.mFlockObjects.Remove(gameObject);
                    continue;
                }
                gameObject.mFlock?.RemoveObjFromFlock(gameObject, mGameLoop);
                gameObject.mFlock = flock;
                gameObject.State = GameObject.ObjectState.Moving;
            }

            mGameLoop.ObjectManager.DataManager.ActiveFlocks.Add(flock);
            return true;
        }

        /// <summary>
        /// Responsible for continuing movement (setting new position) procedure of single gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        internal void ContinueMovement(GameObject gameObject)
        {

            var newPos = CalcOffset(gameObject.ObjectCenter, gameObject.DirectionSpeedVector);  // Calculate new position

            gameObject.ObjectCenter = newPos;
            gameObject.ChangeFace(FaceDirection(gameObject.DirectionSpeedVector));   // Change Sprite (direction)
        }

        /// <summary>
        /// Calculates next Movement direction for flock or stops flock movement when target is reached
        /// </summary>
        /// <param name="flock"></param>
        internal void NextMovement(Flock flock)
        {
            var target = flock.mPathQueue.Peek();
            var distToTarget = Vector2.Distance(CentreOfMass(flock.mFlockObjects), target);
            const int step = 40;
            // Prepare new Movement or End Movement
            if (distToTarget < step) // distance to target very small / target reached
            {
                flock.mPathQueue.Dequeue(); // remove current target from queue
            }

            // Fix/Enhancement for MicroKi: MonsterFrogs will stop in Range to targetObject (however not if the real target is the spaceship while walking target is a wall)
            var gameObject = flock.mFlockObjects.First();
            if (!(gameObject.mCurrentTargetObject is Spaceship1) && gameObject is MonsterFrog && Vector2.Distance(flock.mTarget, gameObject.ObjectCenter) < Math.Abs(gameObject.StrikingDistance - 20)) // distance to target very small / target reached
            {
                EndMovementFlock(flock);
                return;
            }

            if (flock.mPathQueue.Count == 0) // EndMovement for all Objects in Flock if EndTarget is reached
            {
                EndMovementFlock(flock);
            }
            else
            {
                flock.Update();
            }
        }

        /// <summary>
        /// Stops movement procedure (only) for given GameObject and resets all parameters
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="idleState">Set state of gameObject to Idle</param>
        internal void EndMovement(GameObject gameObject, bool idleState = false)
        {
            gameObject.DirectionSpeedVector = Vector2.Zero;
            gameObject.mFlock?.RemoveObjFromFlock(gameObject, mGameLoop);
            gameObject.mFlock = null;
            if (idleState)
            {
                gameObject.State = GameObject.ObjectState.Idle;
            }
        }

        /// <summary>
        /// Stops movement procedure for given Flock and resets the corresponding parameters of all objects in the flock
        /// </summary>
        /// <param name="flock"></param>
        internal void EndMovementFlock(Flock flock)
        {
            mGameLoop.ObjectManager.DataManager.ActiveFlocks.Remove(flock);

            foreach (var gameObject in flock.mFlockObjects)
            {
                gameObject.DirectionSpeedVector = Vector2.Zero;
                gameObject.State = GameObject.ObjectState.Idle;
                gameObject.mFlock = null;
            }
        }

        /// <summary>
        /// Returns given Rectangle while its position is changed by the values of the given Vector2 or replaced by the Vector2 Values if replace=true
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        private static Vector2 CalcOffset(Vector2 position, Vector2 direction, bool replace = false)
        {
            if (replace)
            {
                position.X = (int)direction.X;
                position.Y = (int)direction.Y;
            }
            else
            {
                position += direction;
            }
            return position;
        }

        /// <summary>
        /// Returns new FaceDirection depending on given movement-direction (Vector2) 
        /// </summary>
        /// <param name="directionVector"></param>
        /// <returns string="direction"></returns>
        public static string FaceDirection(Vector2 directionVector)
        {
            string str;
            directionVector.Normalize();  // unit vector
            double[] radList = { 0.785, 2.356 };  // areas that divide the unit circle
            var phi = Math.Atan2(-directionVector.Y, directionVector.X);  // phi in range (0 to pi) or (-pi to 0)
            if (phi >= radList[0] && phi <= radList[1])
            {
                str = "Back";
            }
            else if (phi > radList[1] || phi <= -radList[1])
            {
                str = "Left";
            }
            else if (phi > -radList[1] && phi < -radList[0])
            {
                str = "Front";
            }
            else
            {
                str = "Right";
            }
            return str;
        }

        /// <summary>
        /// Computes the relative center of a list of objects
        /// </summary>
        /// <param name="others">the list of all objects.</param>
        /// <returns>the relative center as a Vector</returns>
        private static Vector2 CentreOfMass(IReadOnlyCollection<GameObject> others)
        {
            var center = Vector2.Zero;
            foreach (var other in others)
            {
                center += new Vector2(other.ObjectCenter.X, other.ObjectCenter.Y);
            }
            center /= others.Count;
            return center;
        }
    }
}
