using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Components;
using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;

namespace ECTS.Pathfinder
{
    /// <summary>
    /// Class to manage Flocks.
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class Flock
    {
        [DataMember] internal Queue<Vector2> mPathQueue;
        [DataMember] internal HashSet<GameObject> mFlockObjects;
        [DataMember] internal Vector2 mTarget;

        /// <summary>
        /// Removes gameObject from the Flock it currently is assigned to.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="gameLoop"></param>
        internal void RemoveObjFromFlock(GameObject gameObject, GameLoop gameLoop)
        {
            if (mFlockObjects.Contains(gameObject))
            {
                mFlockObjects.Remove(gameObject);
                if (mFlockObjects.Count == 0)
                {
                    gameLoop.ObjectManager.DataManager.ActiveFlocks.Remove(this);
                }
            }
        }

        /// <summary>
        /// Updates the movement-parameters for all gameObjects in this flock.
        /// </summary>
        internal void Update()
        {
            Global.mGameLoop.ObjectManager.Pathfinder.Flocking.CalculateDirectionSpeedVector(mFlockObjects,
                mPathQueue.Peek());
        }

        /// <summary>
        /// Calculate new Path for Flock in Case there is an obstacle or similar problems on current path.
        /// </summary>
        /// <param name="gameLoop">current gameLoop</param>
        internal void ChangePath(GameLoop gameLoop)
        {
            var start = Center(mFlockObjects);
            mPathQueue = new Queue<Vector2>(gameLoop.ObjectManager.Pathfinder.AStar.FindPath(start, mTarget));
            if (mPathQueue.Count == 0)
            {
                gameLoop.ObjectManager.Pathfinder.EndMovementFlock(this);
            }
        }

        /// <summary>
        /// Calculate Center position of flock.
        /// </summary>
        /// <param name="others">List of all objects in Flock</param>
        /// <returns>Vector2 of the center position</returns>
        private Vector2 Center(IReadOnlyCollection<GameObject> others)
        {
            Vector2 center = Vector2.Zero;
            foreach (var other in others)
            {
                center += other.ObjectCenter;
            }

            center /= others.Count;
            return center;
        }
    }

    
}