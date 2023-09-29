using ECTS.Objects.GameObjects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ECTS.Data
{
    [DataContract]
    public sealed class QuadTree
    {
        // List of entries in current tree. Not including deeper entries.
        [DataMember]  private List<GameObject> mEntries;
        // Max amount of Entries.
        [DataMember]  private readonly int mCapacity;
        // Current amount of Entries.
        [DataMember]  public int mCount;
        // Position inherited by current tree.
        [DataMember]  public Rectangle mPosition;
        [DataMember]  private bool mIsDivided;
        //0 = northeast. 1 = northwest. 2 = southwest. 3 = southeast
        [DataMember]  private QuadTree[] mSubTrees;
        // Current depth
        [DataMember]  private int mDepth;
        private const int MaxDepth = 10;

        public QuadTree(Rectangle pos, int capacity, int depth)
        {
            mCapacity = capacity;
            mCount = 0;
            mPosition = pos;
            mEntries = new List<GameObject>();
            mIsDivided = false;
            mDepth = depth;
        }

        // Adds an Entry to the QuadTree. If necessary it gets subdivided. Returns false if entry not inside QuadTrees position.
        public bool Add(GameObject entry)
        {
            // Check for valid Entry
            if (!mPosition.Intersects(entry.Position))
            {
                return false;
            }
            // Add to current Tree
            if (mCount < mCapacity || mDepth > MaxDepth)
            {
                mEntries.Add(entry);
                mCount++;
                return true;
            }
            // Add to Subtree

            if (!mIsDivided)
            {
                SubDivide();
                foreach (var gameObject in mEntries)
                {
                    AddToRightSubTree(gameObject);
                }

                mEntries = null;
            }

            if (!AddToRightSubTree(entry))
            {
                return false;
            }

            mCount++;
            return true;

        }

        private bool AddToRightSubTree(GameObject entry)
        {
            if (mSubTrees[0].Add(entry))
            {
                return true;
            }

            if (mSubTrees[1].Add(entry))
            {
                return true;
            }

            if (mSubTrees[2].Add(entry))
            {
                return true;
            }

            if (mSubTrees[3].Add(entry))
            {
                return true;
            }
            return false;
        }

        private void SubDivide()
        {
            mSubTrees = new QuadTree[4];
            mSubTrees[0] = new QuadTree(new Rectangle(mPosition.X + mPosition.Width * 1 / 2 -1,
                    mPosition.Y + mPosition.Height * 1 / 2 - 1,
                    mPosition.Width / 2 + 2,
                    mPosition.Height / 2 + 2),
                mCapacity,
                mDepth + 1);
            mSubTrees[1] = new QuadTree(new Rectangle(mPosition.X,
                    mPosition.Y + mPosition.Height * 1 / 2 - 1,
                    mPosition.Width / 2,
                    mPosition.Height / 2 + 2),
                mCapacity,
                mDepth + 1);
            mSubTrees[2] = new QuadTree(new Rectangle(mPosition.X,
                    mPosition.Y,
                    mPosition.Width / 2,
                    mPosition.Height / 2),
                mCapacity,
                mDepth + 1);
            mSubTrees[3] = new QuadTree(new Rectangle(mPosition.X + mPosition.Width * 1 / 2 - 1,
                    mPosition.Y,
                    mPosition.Width / 2 + 2,
                    mPosition.Height / 2),
                mCapacity,
                mDepth + 1);
            mIsDivided = true;
        }

        // Returns all Entries inside given Area
        public List<GameObject> GetEntriesInRectangle(Rectangle selection)
        {
            List<GameObject> entries = new List<GameObject>();

            if (!mIsDivided)
            {
                entries.AddRange(mEntries.Where(entry => selection.Intersects(entry.Position)));
            }

            if (mIsDivided)
            {
                var offsetX = mPosition.Width;
                var offsetY = mPosition.Height;
                var extendedSelection = new Rectangle(selection.X - offsetX, selection.Y - offsetY, selection.Width + 2 * offsetX, selection.Height + 2 * offsetY);
                foreach (var subTree in mSubTrees)
                {
                    if (extendedSelection.Intersects(subTree.mPosition))
                    {
                        entries.AddRange(subTree.GetEntriesInRectangle(selection));
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Returns all INTERACTING gameObjects inside given Area with one of the specified objectTypes.
        /// </summary>
        /// <param name="selection">Area with returned objects.</param>
        /// <param name="objectType">List with objectTypes to be returned</param>
        public List<GameObject> GetEntriesInRectangleByType(Rectangle selection, GameObject.ObjectType objectType) 
        {
            List<GameObject> entries = new List<GameObject>();

            if (!mIsDivided)
            {   
                entries.AddRange(mEntries.Where(entry => selection.Intersects(entry.Position) && 
                                                         entry.Type == objectType &&
                                                         entry.IsInteracting));
            }

            if (mIsDivided)
            {
                var offsetX = mPosition.Width;
                var offsetY = mPosition.Height;
                var extendedSelection = new Rectangle(selection.X - offsetX, selection.Y - offsetY, selection.Width + 2 * offsetX, selection.Height + 2 * offsetY);
                foreach (var subTree in mSubTrees)
                {
                    if (extendedSelection.Intersects(subTree.mPosition))
                    {
                        entries.AddRange(subTree.GetEntriesInRectangleByType(selection, objectType));
                    }
                }
            }

            return entries;
        }


        /// <summary>
        /// Goes through whole quadtree without checking for intersection.
        /// </summary>
        /// <returns>all game objects saved in tree.</returns>
        public List<GameObject> GetAllEntries()
        {
            if (!mIsDivided)
            {
                return mEntries;
            }

            List<GameObject> lst = new List<GameObject>();
            foreach (var subTree in mSubTrees)
            {
                lst.AddRange(subTree.GetAllEntries());
            }

            return lst;
        }
    }
}
