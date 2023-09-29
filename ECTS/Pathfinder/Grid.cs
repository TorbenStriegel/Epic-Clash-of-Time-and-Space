using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ECTS.Data;
using ECTS.Objects.GameObjects;

namespace ECTS.Pathfinder
{
    /// <summary>
    /// Responsible for representing the world map in a way the AStar algorithm can use
    /// </summary>
    public sealed class Grid
    {
        private readonly QuadTree mQuadTree;
        private readonly Vector2 mGridWorldSize;
        private Node[,] mGrid;

        private readonly float mNodeDiameter;
        private readonly int mGridSizeX, mGridSizeY;
        private const float Buf = 0.5f;

        /// <summary>
        /// Create a new instance of class grid.
        /// </summary>
        /// <param name="nodeDiameter">Diameter of the single nodes in the grid.</param>
        /// <param name="gridWorldSize">The size of the world that is represented by the grid.</param>
        /// <param name="tree">The Quad-tree that holds the information about the objects in the world.</param>
        public Grid(float nodeDiameter, Vector2 gridWorldSize, QuadTree tree)
        {
            mQuadTree = tree;
            mNodeDiameter = nodeDiameter;
            mGridWorldSize = gridWorldSize;
            // Compute GridSize by parting given WorldSize into NodeDiameter sized pieces
            mGridSizeX = (int) (mGridWorldSize.X / mNodeDiameter + Buf);
            mGridSizeY = (int) (mGridWorldSize.Y / mNodeDiameter + Buf);
            CreateGrid();
        }

        /// <summary>
        /// Return the maximum size of the Grid.
        /// </summary>
        public int MaxSize
        {
            get => mGridSizeX * mGridSizeY;
        }

        /// <summary>
        /// Create the Grid used for the algorithm and fill it with nodes.
        /// Use the quad-tree to determine which nodes are walkable.
        /// </summary>
        private void CreateGrid()
        {
            mGrid = new Node[mGridSizeX, mGridSizeY];

            for (int x = 0; x < mGridSizeX; x++)
            {
                for (int y = 0; y < mGridSizeY; y++)
                {
                    Vector2 worldPoint = Vector2.Zero + Vector2.UnitX * (x * mNodeDiameter) +
                                          Vector2.UnitY * (y * mNodeDiameter);
                    bool walkable = CheckForObstacle(worldPoint);
                    mGrid[x, y] = new Node(walkable, worldPoint, x, y);
                }
            }
        }

        /// <summary>
        /// Function that returns a list of all the Nodes that are adjacent to the given node in the grid.
        /// </summary>
        /// <param name="node">Node the neighbors are wanted for.</param>
        /// <returns>List of all neighboring nodes.</returns>
        public List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = node.mGridX + x;
                    int checkY = node.mGridY + y;

                    if (checkX >= 0 && checkX < mGridSizeX && checkY >= 0 && checkY < mGridSizeY)
                    {
                        neighbors.Add(mGrid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Check if there is an obstacle in the world at a given point in the world.
        /// </summary>
        /// <param name="point">World Point of a Node in the Grid.</param>
        /// <returns>True, if there are now objects in the nodes rectangle, false if there are some.</returns>
        private bool CheckForObstacle(Vector2 point)
        {
            int x = (int)(point.X + Buf);
            int y = (int) (point.Y + Buf);
            int rectangleWidth = (int)(mNodeDiameter + Buf);
            int rectangleHeight = (int)(mNodeDiameter + Buf);
            List<GameObject> objects = mQuadTree.GetEntriesInRectangle(new Rectangle(x, y, rectangleWidth, rectangleHeight));
            // If objects is empty there is no obstacle in the Node at position point
            if (objects.Count != 0)
            {
                foreach (GameObject i in objects)
                {
                    if (i.IsColliding)  
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Function that is given a point in the world and returns the corresponding node from the grid.
        /// </summary>
        /// <param name="worldPos">Vector2 of the world point.</param>
        /// <returns>returns the Node that contains the given point.</returns>
        public Node NodeFromWorldPoint(Vector2 worldPos)
        {
            // compute a the position in percent relative to the world size
            float percentX = worldPos.X / mGridWorldSize.X;
            float percentY = worldPos.Y / mGridWorldSize.Y;
            // Make sure the values are in between 0 and 1
            percentX = Math.Min(Math.Max(percentX, 0f), 1f);
            percentY = Math.Min(Math.Max(percentY, 0f), 1f);
            // get the Coordinates in Grid
            int x = (int) ((mGridSizeX - 1) * percentX + Buf);
            int y = (int) ((mGridSizeY - 1) * percentY + Buf);
            return mGrid[x, y];
        }

        /// <summary>
        /// Function to update the walkable Status of all Nodes that are intersecting with a given rectangle.
        /// </summary>
        /// <param name="objectPos">A rectangle of which the status changed. (Object Rectangle).</param>
        public void UpdateNodesInRectangle(Rectangle objectPos)
        {
            Node upperLeft = NodeFromWorldPoint(new Vector2(objectPos.X, objectPos.Y));
            Node lowerRight = NodeFromWorldPoint(new Vector2(objectPos.Right, objectPos.Bottom));
            // Iterate through all the nodes that intersect the given Rectangle
            for (int x = upperLeft.mGridX; x <= lowerRight.mGridX; x++)
            {
                for (int y = upperLeft.mGridY; y <= lowerRight.mGridY; y++)
                {
                    // Check for each node if it is now walkable
                    mGrid[x, y].mWalkable = CheckForObstacle(mGrid[x, y].mWorldPosition);
                }
            }
        }
    }

}
