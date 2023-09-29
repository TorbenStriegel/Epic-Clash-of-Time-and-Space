using System.Collections.Generic;
using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;

namespace ECTS.Pathfinder
{
    public sealed class Flocking
    {
        private readonly GameLoop mGameLoop;
        public Flocking(GameLoop gameLoop)
        {
            mGameLoop = gameLoop;
        }

        /// <summary>
        /// Simulate flocking in a given group of objects
        /// Compute direction, velocity and movement of given objects, regulated by 6 rules
        /// </summary>
        /// <param name="toMove">List of objects in the flock</param>
        /// <param name="leader">Position of a leading object or a place the flock should move towards. default is an empty vector.</param>
        /// <param name="bounds">Optional boundary the flock should not leave, default is an empty rectangle</param>
        public void CalculateDirectionSpeedVector(HashSet<GameObject> toMove, Vector2 leader = new Vector2(), Rectangle bounds = new Rectangle())
        {
            var limit = 0;
            // Find lowest speed in given object group
            foreach (var obj in toMove)
            {
                if (limit == 0)
                {
                    limit = obj.Speed;
                }
                else if (limit > obj.Speed)
                {
                    limit = obj.Speed;
                }
            }
            // Change direction vector of each 
            foreach (var obj in toMove)
            {
                Vector2 v1, v2, v3, v4, v5, c, pv;
                var count = toMove.Count;
                c = CentreOfMass(obj, toMove);
                pv = PerceivedVelocity(obj, toMove);
                v1 = MoveTowardsOtherObjects(obj, c) / (count * limit);
                v2 = Avoiding(obj) / (count * limit);
                if (float.IsNaN(v2.X) || float.IsNaN(v2.Y) || float.IsInfinity(v2.X) || float.IsInfinity(v2.Y))
                {
                    if (float.IsNaN(obj.DirectionSpeedVector.X) || float.IsNaN(obj.DirectionSpeedVector.Y))
                    {
                        obj.DirectionSpeedVector = Vector2.One;
                    }
                    else
                    {
                        obj.DirectionSpeedVector.Normalize();
                    }
                    v2 = obj.DirectionSpeedVector / (count * limit);
                }
                v3 = MatchVelocity(obj, pv) / (count * limit);
                if (leader != Vector2.Zero)
                {
                    v4 = MoveTowardsLeader(obj, leader) / (count * limit);
                }
                else
                {
                    v4 = Vector2.Zero;
                }

                if (bounds != Rectangle.Empty)
                {
                    v5 = BoundPosition(obj, bounds);
                }
                else
                {
                    v5 = Vector2.Zero;
                }

                obj.DirectionSpeedVector = obj.DirectionSpeedVector + v1 + v2 + v3 + v4 + v5;
                LimitSpeed(obj, limit);

                if (float.IsNaN(obj.DirectionSpeedVector.X) || float.IsInfinity(obj.DirectionSpeedVector.X))
                {
                    obj.DirectionSpeedVector = Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// Rule 1 of flocking simulations
        /// Objects should move towards their flock
        /// </summary>
        /// <param name="obj">An object in a flock</param>
        /// <param name="center">The center of the group of objects in which obj is moving</param>
        /// <returns>The direction the object must turn to in order to reach the center of its group</returns>
        private Vector2 MoveTowardsOtherObjects(GameObject obj, Vector2 center)
        {
            var pos = new Vector2(obj.Position.X, obj.Position.Y);

            return (center - pos) / 10;
        }

        /// <summary>
        /// Rule 2 of flocking simulations
        /// Objects should avoid other objects and obstacles
        /// </summary>
        /// <param name="obj">An object in the flock</param>
        /// <returns>A vector that steers the object away from all objects it is too close to.</returns>
        private Vector2 Avoiding(GameObject obj)
        {
            Vector2 avoidance = Vector2.Zero;
            Vector2 pos = new Vector2(obj.Position.X, obj.Position.Y);
            int sightRadius = 20;
            Rectangle sightField = new Rectangle((int) obj.ObjectCenter.X - sightRadius - 16, (int) obj.ObjectCenter.Y - sightRadius - 16, 32 + 2 * sightRadius, 32 + 2 * sightRadius);
            foreach (var col in mGameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(sightField))
            {
                if (col != obj && (col.IsColliding || col.Type == GameObject.ObjectType.Player || col.Type == GameObject.ObjectType.Enemy || col.Type == GameObject.ObjectType.Gate)) 
                {
                    var colPos = new Vector2(col.Position.X, col.Position.Y);
                    var dist = colPos - pos;
                    if (dist.Length() <= 20)
                    {
                        if ((int) dist.Length() == 0)
                        {
                            obj.DirectionSpeedVector.Normalize();
                            dist += obj.DirectionSpeedVector;
                        }

                        dist *= 48 / dist.Length();
                        if (col.Type == GameObject.ObjectType.Wall || col.Type == obj.Type)
                        {
                            dist *= 3;
                        }
                    }

                    if (col.IsColliding && col.Type != obj.Type)
                    {
                        dist *= 2;
                    }
                    avoidance -= dist;
                }
            }

            if (float.IsNaN(avoidance.X) || float.IsInfinity(avoidance.X) || float.IsNaN(avoidance.Y) ||
                float.IsInfinity(avoidance.Y))
            {
                obj.DirectionSpeedVector.Normalize();
                avoidance = obj.DirectionSpeedVector;
            }
            return avoidance / 15;
        }

        /// <summary>
        /// Rule 3 of flocking simulations
        /// Objects should match their velocity with nearby objects
        /// </summary>
        /// <param name="obj">A given Object of the flock</param>
        /// <param name="perVel">Perceived Velocity of given obj compared to the rest of the flock</param>
        /// <returns>A small portion of the perceived Velocity relative to the objects is returned</returns>
        private Vector2 MatchVelocity(GameObject obj, Vector2 perVel)
        {
            return perVel - obj.DirectionSpeedVector;
        }

        /// <summary>
        /// Optional Rule 4
        /// Objects should move towards / follow a leading object
        /// </summary>
        /// <param name="obj">An object in the flock</param>
        /// <param name="leadPos">The position the object should move towards</param>
        /// <returns>A Vector that represents the direction the object needs to move in to follow the leader or to reach its destination.</returns>
        private Vector2 MoveTowardsLeader(GameObject obj, Vector2 leadPos)
        {
            var posVector = new Vector2(obj.Position.X, obj.Position.Y);
            return (leadPos - posVector) / 3;
        }

        /// <summary>
        /// Optional Rule 5
        /// Objects should stay within a given rectangle, for example the game world
        /// </summary>
        /// <param name="obj">An object that should stay in the given bounds.</param>
        /// <param name="bounds">A rectangle representing the valid positions for the object.</param>
        /// <returns>A Vector that makes the object move back into the valid bounds if it is out of them, else 0.</returns>
        private Vector2 BoundPosition(GameObject obj, Rectangle bounds)
        {
            Vector2 direction = Vector2.Zero;
            int bounce = 10;
            if (obj.Position.Left < bounds.Left)
            {
                direction.X = bounce;
            }
            else if (obj.Position.Right > bounds.Right)
            {
                direction.X = -bounce;
            }

            if (obj.Position.Bottom < bounds.Bottom)
            {
                direction.Y = bounce;
            }
            else if (obj.Position.Top > bounds.Top)
            {
                direction.Y = -bounce;
            }

            return direction;
        }

        /// <summary>
        /// Optional Rule 6
        /// Objects should not move faster than a maximum speed
        /// Function limits a given objects speed and direction vector to a given speed limit
        /// </summary>
        /// <param name="obj">Object of the flock that should be limited in speed</param>
        /// <param name="limit">max speed the obj should move at</param>
        private void LimitSpeed(GameObject obj, int limit)
        {
            float len = obj.DirectionSpeedVector.Length();
            if (len > limit || len < limit / 2.0)
            {
                obj.DirectionSpeedVector = obj.DirectionSpeedVector / len * limit;
            }
        }

        /// <summary>
        /// Computes the relative center of a flock, excluding the position of the given object.
        /// </summary>
        /// <param name="obj">Object the relative center should be computed for.</param>
        /// <param name="others">the list of all objects in the flock.</param>
        /// <returns>the relative center as a Vector</returns>
        private Vector2 CentreOfMass(GameObject obj, HashSet<GameObject> others)
        {
            Vector2 center = Vector2.Zero;
            foreach (var other in others)
            {
                if (others.Count == 1)
                {
                    return new Vector2(other.Position.X, other.Position.Y);
                }

                if (other != obj)
                {
                    center += new Vector2(other.Position.X, other.Position.Y);
                }
            }

            center = center / (others.Count - 1);
            return center;
        }

        /// <summary>
        /// Computes the velocity of the flock as perceived by a given object.
        /// </summary>
        /// <param name="obj">Object from a flock</param>
        /// <param name="others">List of all objects in the flock</param>
        /// <returns>A Vector of the relative velocity / direction vectors of the flock.</returns>
        private Vector2 PerceivedVelocity(GameObject obj, HashSet<GameObject> others)
        {
            Vector2 perVel = Vector2.Zero;
            foreach (var other in others)
            {
                if (others.Count == 1)
                {
                    return other.DirectionSpeedVector;
                }

                if (obj != other)
                {
                    perVel += other.DirectionSpeedVector;
                }
            }

            perVel = perVel / (others.Count - 1);
            return perVel;
        }
    }
}