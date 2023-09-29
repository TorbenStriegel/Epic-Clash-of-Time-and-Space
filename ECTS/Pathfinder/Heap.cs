using System;

namespace ECTS.Pathfinder
{
    /// <summary>
    /// Implementation of a simple binary Heap
    /// </summary>
    /// <typeparam name="T">Type of the items in the heap.</typeparam>
    public sealed class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] mItems;
        private int mCurrentItemCount;

        /// <summary>
        /// Create a new heap.
        /// </summary>
        /// <param name="maxHeapSize">Set the maximum size of the heap.</param>
        public Heap(int maxHeapSize)
        {
            mItems = new T[maxHeapSize];
        }

        /// <summary>
        /// Function that adds a new item to the heap in the correct position.
        /// </summary>
        /// <param name="item">Item that should be added to the heap.</param>
        public void Add(T item)
        {
            item.HeapIndex = mCurrentItemCount;
            mItems[mCurrentItemCount] = item;
            SortUp(item);
            mCurrentItemCount++;
        }

        /// <summary>
        /// Function to remove the first item from the heap and restore Heap Properties.
        /// </summary>
        /// <returns>The first node from the heap.</returns>
        public T RemoveFirst()
        {
            var firstItem = mItems[0];
            mCurrentItemCount--;
            mItems[0] = mItems[mCurrentItemCount];
            mItems[0].HeapIndex = 0;
            SortDown(mItems[0]);
            return firstItem;
        }

        /// <summary>
        /// Return the number of Items in the Heap.
        /// </summary>
        public int Count
        {
            get => mCurrentItemCount;
        }

        /// <summary>
        /// Check if the given item is in the heap.
        /// </summary>
        /// <param name="item">Item the heap should be checked for.</param>
        /// <returns>True if item is in the heap, false if not.</returns>
        public bool Contains(T item)
        {
            return Equals(mItems[item.HeapIndex], item);
        }

        /// <summary>
        /// Function that restores the Heap Property by moving down the heap.
        /// </summary>
        /// <param name="item">Item from which the heap should be repaired downwards from.</param>
        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexL = item.HeapIndex * 2 + 1;
                int childIndexR = item.HeapIndex * 2 + 2;
                int swapIndex;

                if (childIndexL < mCurrentItemCount)
                {
                    swapIndex = childIndexL;

                    if (childIndexR < mCurrentItemCount)
                    {
                        if (mItems[childIndexL].CompareTo(mItems[childIndexR]) < 0)
                        {
                            swapIndex = childIndexR;
                        }
                    }

                    if (item.CompareTo(mItems[swapIndex]) < 0)
                    {
                        Swap(item, mItems[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Function that restores the Heap Property by moving up the heap.
        /// </summary>
        /// <param name="item">Item from which the heap should be repaired upwards from.</param>
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = mItems[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        /// <summary>
        /// Swap the position of two items in the Heap.
        /// </summary>
        /// <param name="itemA">First Item to swap.</param>
        /// <param name="itemB">Second Item to swap.</param>
        private void Swap(T itemA, T itemB)
        {
            mItems[itemA.HeapIndex] = itemB;
            mItems[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    /// <summary>
    /// Interface.
    /// Specify which Types T can be used in Heap.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHeapItem<in T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}