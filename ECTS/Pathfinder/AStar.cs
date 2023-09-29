using System;
using System.Collections.Generic;
using ECTS.Data;
using Microsoft.Xna.Framework;

namespace ECTS.Pathfinder
{
    /// <summary>
    /// Responsible for Calculating a valid path between to given points in world coordinates
    /// </summary>
    public sealed class AStar
    {
        public readonly Grid mGrid;

        /// <summary>
        /// Create a new instance of class AStar.
        /// </summary>
        /// <param name="nodeSize">Size the single nodes in the grid should have.</param>
        /// <param name="worldSize">The Size of the world to be used as base for the grid.</param>
        /// <param name="tree">QuadTree that is base for the world.</param>
        public AStar(float nodeSize, Vector2 worldSize, QuadTree tree)
        {
            mGrid = new Grid(nodeSize, worldSize, tree);
        }

        /// <summary>
        /// Find a path between to points in the world by using the A* Algorithm.
        /// </summary>
        /// <param name="startPos">Starting point of the wanted path.</param>
        /// <param name="targetPos">Target point of the wanted path.</param>
        /// <returns>List of all positions in the path that require a change in direction.</returns>
        public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            var startNode = mGrid.NodeFromWorldPoint(startPos);
            var targetNode = mGrid.NodeFromWorldPoint(targetPos);

            if (!targetNode.mWalkable)
            {
                targetNode = NextWalkableNode(startNode, targetNode);
            }
            
            Heap<Node> openSet = new Heap<Node>(mGrid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            List<Vector2> resultPath = new List<Vector2>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    resultPath = RetracePath(startNode, targetNode);
                    return resultPath;
                }

                foreach (Node neighbor in mGrid.GetNeighbors(currentNode))
                {
                    if (!neighbor.mWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMoveCostToNeighbor = currentNode.mGCost + GetDistance(currentNode, neighbor);
                    if (newMoveCostToNeighbor < neighbor.mGCost || !openSet.Contains(neighbor))
                    {
                        neighbor.mGCost = newMoveCostToNeighbor;
                        neighbor.mHCost = GetDistance(neighbor, targetNode);
                        neighbor.mParent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            return resultPath;
        }

        /// <summary>
        /// Given two Nodes, this Method retraces the path between them, assuming that FindPath was already used
        /// The returned path are the vectors of positions that require a change in direction.
        /// </summary>
        /// <param name="startNode">starting Node of the path.</param>
        /// <param name="endNode">ending Node of the path.</param>
        /// <returns>The path with start, end and all turning points in between.</returns>
        List<Vector2> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            List<Vector2> coorpath = new List<Vector2>();
            Node currentNode = endNode;
            // Retrace the complete Path by following the parent node of each node starting by the end node 
            // and reversing the list in the end
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.mParent;
            }
            path.Reverse();

            // in case there was no correct path found, return an empty list
            if (path.Count < 2)
            {
                return coorpath;
            }

            Node lastPos = path[0];
            Node currentPos = path[1];

            // In case the list only has two entries (start and end node are direct neighbors)
            if (currentPos == endNode)
            {
                coorpath.Add(endNode.mWorldPosition);
                return coorpath;
            }

            // Check for each new node if it lies in the same direction from the current node as the current does to the last
            // if so skip over the current node
            // if not, there is a change in direction in the path at the current nodes position, save this position into the
            // coorpath list.
            foreach (Node next in path)
            {
                
                // skip the first two nodes in the list
                if (next == path[0] || next == path[1])
                {
                    continue;
                }

                int xdiff = Math.Abs(lastPos.mGridX - currentPos.mGridX) - Math.Abs(currentPos.mGridX - next.mGridX);
                int ydiff = Math.Abs(lastPos.mGridY - currentPos.mGridY) - Math.Abs(currentPos.mGridY - next.mGridY);
                // if the next node doesn't lie in the same direction as the current one
                if (xdiff == 0 && ydiff != 0 || xdiff != 0 && ydiff == 0)
                {
                    coorpath.Add(currentPos.mWorldPosition);
                }

                if (next == endNode)
                {
                    coorpath.Add(endNode.mWorldPosition);
                }

                lastPos = currentPos;
                currentPos = next;

            }
            return coorpath;
        }

        /// <summary>
        /// Compute Distance between the two given Nodes.
        /// </summary>
        /// <param name="nodeA">First Node</param>
        /// <param name="nodeB">Second Node</param>
        /// <returns>Integer of the distance between those two.</returns>
        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Math.Abs(nodeA.mGridX - nodeB.mGridX);
            int dstY = Math.Abs(nodeA.mGridY - nodeB.mGridY);
            int diag = 14;
            int flat = 10;

            if (dstX > dstY)
            {
                return diag * dstY + flat * (dstX - dstY);
            }

            return diag * dstX + flat * (dstY - dstX);
        }

        /// <summary>
        /// Returns the next walkable Node from the given Node toChange.
        /// </summary>
        /// <param name="compareNode">Node to check the changed Node against (should be the starting pos of the path).</param>
        /// <param name="toChange">End Node of the Path, that is not walkable.</param>
        /// <returns>The next walkable Node.</returns>
        private Node NextWalkableNode(Node compareNode, Node toChange)
        {
            Node targetNew = toChange;
            int dist = GetDistance(compareNode, toChange);
            int count = 0;
            int lim = 100;
            while (!targetNew.mWalkable && count <= lim)
            {
                foreach (Node node in mGrid.GetNeighbors(targetNew))
                {
                    if (node.mWalkable)
                    {
                        targetNew = node;
                        continue;
                    }

                    int dist1 = GetDistance(toChange, node);
                    int dist2 = GetDistance(compareNode, node);
                    if (dist2 <= dist || dist1 <= dist)
                    {
                        targetNew = node;
                        dist = dist2;
                    }
                }

                count++;
            }
            return targetNew;
        }
    }
}