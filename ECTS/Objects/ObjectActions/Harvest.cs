using System.Runtime.Serialization;
using ECTS.Objects.GameObjects;

namespace ECTS.Objects.ObjectActions
{
    [DataContract]
    public sealed class Harvest
    {
        private GameLoop GameLoop { get; }
        public Harvest(GameLoop gameLoop)
        {
            GameLoop = gameLoop;

        }
        /// <summary>
        /// harvesting function for any object with health
        /// </summary>
        /// <param name="actingObject">Acting object.</param>
        ///  <param name="targetObject">Harvested object.</param>
        public void HarvestObject(GameObject actingObject, GameObject targetObject)
        {
            if (targetObject.Health > 0 && targetObject.DefenseStrength > 0)  // Prevent possible division trough 0
            {
                var healthDiff = actingObject.AttackStrength / (10 * targetObject.DefenseStrength);

                targetObject.Health -= healthDiff;

                if (targetObject.Type == GameObject.ObjectType.Tree)
                {
                    GameLoop.ObjectManager.DataManager.ResourceList[2] += healthDiff;
                    GameLoop.ObjectManager.DataManager.mCollectedWood += healthDiff;
                    GameLoop.Settings.mCollectedWood += healthDiff;
                }
                else if (targetObject.Type == GameObject.ObjectType.Food)
                {
                    GameLoop.ObjectManager.DataManager.ResourceList[3] += healthDiff / 4f;
                    GameLoop.ObjectManager.DataManager.mCollectedFood += healthDiff / 4f;
                    GameLoop.Settings.mCollectedFood += healthDiff / 4f;
                }
                else if (targetObject.Type == GameObject.ObjectType.Stone)
                {
                    GameLoop.ObjectManager.DataManager.ResourceList[1] += healthDiff;
                    GameLoop.ObjectManager.DataManager.mCollectedStone += healthDiff;
                    GameLoop.Settings.mCollectedStone += healthDiff;
                }
                else if (targetObject.Type == GameObject.ObjectType.Metal)
                {
                    GameLoop.ObjectManager.DataManager.ResourceList[0] += healthDiff;
                    GameLoop.ObjectManager.DataManager.mCollectedMetal += healthDiff;
                    GameLoop.Settings.mCollectedMetal += healthDiff;
                }
            } else if (targetObject.Health <= 0) // if harvesting is finished for this object, set active object to idle
            {
                    actingObject.State = GameObject.ObjectState.Idle;
                    targetObject.ChangeFace("Dead");
                    targetObject.IsInteracting = false;
            }
        }
    }
}
