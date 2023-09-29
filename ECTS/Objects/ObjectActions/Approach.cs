using System.Collections.Generic;
using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.ObjectActions
{
    /// <summary>
    /// Functions for sending an actingObject to a targetObject position.
    /// </summary>
    public sealed class Approach
    {
        private GameLoop GameLoop { get; }
        private List<GameObject> mActingObjectList;
        private Vector2 mTargetObjectPosition;

        public Approach(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }
        /// <summary>
        /// Send object to position of target object using the pathfinder.
        /// Returns true if a path can be found.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Destination</param>
        public bool ApproachObject(GameObject actingObject, GameObject targetObject )
        {
            mActingObjectList = new List<GameObject> { actingObject };
            mTargetObjectPosition.X = targetObject.ObjectCenter.X;
            mTargetObjectPosition.Y = targetObject.ObjectCenter.Y;
            if (targetObject.Type == GameObject.ObjectType.Wall && actingObject.Type == GameObject.ObjectType.Enemy)
            {
                mTargetObjectPosition = targetObject.mMonsterAttackWallPosition;
            }
            return GameLoop.ObjectManager.Pathfinder.StartMovement(mActingObjectList, mTargetObjectPosition);
        }
    }
}
