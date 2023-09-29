
using ECTS.Objects.GameObjects;


namespace ECTS.Objects.ObjectActions
{
    public sealed class Destroy
    {
        internal Destroy()
        {
        }
        /// <summary>
        /// Destroy object with axe stroke. Reduces target object health with each call.
        /// </summary>
        /// <param name="actingObject">GameObject performing an action</param>
        /// <param name="targetObject">Target Object = Object being hit.</param>
        public void AxeStroke(GameObject actingObject, GameObject targetObject)
        {
            if (actingObject.StrikingDistance > ObjectManager.ObjectDistance(actingObject, targetObject)  // use Striking distance in the future if possible ==> grid problem.
                && targetObject.Health > 0f)
            {
                targetObject.Health -= 10 / targetObject.DefenseStrength * actingObject.AttackStrength;
            }
            else
            {
                actingObject.State = GameObject.ObjectState.Idle;
            }
        }
    }
}
