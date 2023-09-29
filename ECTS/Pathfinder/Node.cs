using Microsoft.Xna.Framework;

namespace ECTS.Pathfinder
{
    /// <summary>
    /// Node Class that is used for the world map representation in the Grid class and the AStar algorithm
    /// </summary>
    public sealed class Node : IHeapItem<Node>
    {
        public bool mWalkable;
        public Vector2 mWorldPosition;
        public readonly int mGridX;
        public readonly int mGridY;

        public int mGCost;
        public int mHCost;
        public Node mParent;

        /// <summary>
        /// Create Instance of class Node
        /// </summary>
        /// <param name="walkable">Boolean that sets if the node is blocked or walkable.</param>
        /// <param name="worldPos">The WorldPosition of the upper left corner of the Node.</param>
        /// <param name="gridX">The nodes x position in the Grid.</param>
        /// <param name="gridY">The nodes y position in the Grid.</param>
        public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
        {
            mWalkable = walkable;
            mWorldPosition = worldPos;
            mGridX = gridX;
            mGridY = gridY;
        }

        /// <summary>
        /// Computes the FCost from the values of GCost and HCost (Distance from Starting point, estimated Distance from the Target)
        /// </summary>
        private int FCost
        {
            get => mGCost + mHCost; 
        }

        /// <summary>
        /// Get or Set the Nodes HeapIndex
        /// Used in Class Heap
        /// </summary>
        public int HeapIndex { get; set; }

        /// <summary>
        /// Compare the Node with the given nodeToCompare by FCost or if FCost is identical, by HCost.
        /// </summary>
        /// <param name="other">Node the comparison should be made with.</param>
        /// <returns></returns>
        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = mHCost.CompareTo(other.mHCost);
            }
            return -compare;
        }
    }
}
