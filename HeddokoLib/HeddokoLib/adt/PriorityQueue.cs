/*@file PriorityQueue.cs
* @brief Contains the PriorityQueue class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;

namespace HeddokoLib.adt
{
    /// <summary>
    /// A generic heap, with a list based implementation
    /// </summary>
    /// <typeparam name="T">A HeapItem</typeparam>
    public class PriorityQueue<T> where T : IPriorityQueueItem<T>
    {
        private List<T> mStoredValues;

        public PriorityQueue()
        {
            //Initialize the array that will hold the values
            mStoredValues = new List<T>();

            //Fill the first cell in the array with an empty value
            mStoredValues.Add(default(T));
        }

        /// <summary>
        /// Gets the number of values stored within the Priority Queue
        /// </summary>
        public int Count
        {
            get { return mStoredValues.Count - 1; }
        }

        /// <summary>
        /// Returns the value at the head of the Priority Queue without removing it.
        /// </summary>
        public T Peek()
        {
            if (Count == 0)
            {
                return default(T);
            }
            return mStoredValues[1];

        }

        /// <summary>
        /// Adds a value to the Priority Queue
        /// </summary>
        public void Add(T vItem)
        {
            mStoredValues.Add(vItem);
            BubbleUp(mStoredValues.Count - 1);
        }

        /// <summary>
        /// Returns the minimum value inside the Priority Queue
        /// </summary>
        public T RemoveFirstItem()
        {
            if (Count == 0)
            {
                return default(T); //queue is empty
            }

            T vMinVal = mStoredValues[1];
            if (mStoredValues.Count > 2)
            {
                T vLatValue = mStoredValues[mStoredValues.Count - 1];
                mStoredValues.RemoveAt(mStoredValues.Count - 1);
                mStoredValues[1] = vLatValue;
                BubbleDown(1);
            }
            else
            {
                mStoredValues.RemoveAt(1);
            }

            return vMinVal;

        }

        /// <summary>
        /// Restores the heap-order property between child and parent values going up towards the head
        /// </summary>
        protected void BubbleUp(int vStartIdx)
        {
            int vCellIdx = vStartIdx;

            //Bubble up as long as the parent is greater
            while (IsParentBigger(vCellIdx))
            {
                //Get values of parent and child
                T vParentVal = mStoredValues[vCellIdx / 2];
                T vChildVal = mStoredValues[vCellIdx];

                //Swap the values
                mStoredValues[vCellIdx / 2] = vChildVal;
                mStoredValues[vCellIdx] = vParentVal;

                vCellIdx /= 2; //go up parents
            }
        }

        /// <summary>
        /// Restores the heap-order property between child and parent values going down towards the bottom
        /// </summary>
        protected virtual void BubbleDown(int vStartIdx)
        {
            int vCellIdx = vStartIdx;

            //Bubble down as long as either child is smaller
            while (IsLeftChildSmaller(vCellIdx) || IsRightChildSmaller(vCellIdx))
            {
                int vChildIdx = CompareChild(vCellIdx);

                if (vChildIdx == -1)
                {
                    //Swap values
                    T vParentVal = mStoredValues[vCellIdx];
                    T vLeftChildVal = mStoredValues[2 * vCellIdx];

                    mStoredValues[vCellIdx] = vLeftChildVal;
                    mStoredValues[2 * vCellIdx] = vParentVal;

                    vCellIdx = 2 * vCellIdx;
                }
                else if (vChildIdx == 1)
                {
                    //Swap values
                    T parentValue = mStoredValues[vCellIdx];
                    T rightChildValue = mStoredValues[2 * vCellIdx + 1];

                    mStoredValues[vCellIdx] = rightChildValue;
                    mStoredValues[2 * vCellIdx + 1] = parentValue;

                    vCellIdx = 2 * vCellIdx + 1;
                }
            }
        }

        /// <summary>
        /// Returns if the value of a parent is greater than its child
        /// </summary>
        protected bool IsParentBigger(int vChildCellIdx)
        {
            if (vChildCellIdx == 1)
            {
                return false;
            }

            return mStoredValues[vChildCellIdx / 2].CompareTo(mStoredValues[vChildCellIdx]) > 0;
        }


        public void Clear()
        {
            mStoredValues.Clear();
            mStoredValues.Add(default(T));
        }
        /// <summary>
        /// Returns whether the left child cell is smaller than the parent cell.
        /// Returns false if a left child does not exist.
        /// </summary>
        protected virtual bool IsLeftChildSmaller(int vParentIdx)
        {
            if (2 * vParentIdx >= mStoredValues.Count)
            {
                return false;
            }

            return mStoredValues[2 * vParentIdx].CompareTo(mStoredValues[vParentIdx]) < 0;

        }

        /// <summary>
        /// Returns whether the right child cell is smaller than the parent cell.
        /// Returns false if a right child does not exist.
        /// </summary>
        protected virtual bool IsRightChildSmaller(int vParentIdx)
        {
            if (2 * vParentIdx + 1 >= mStoredValues.Count)
            {
                return false; //out of bounds
            }

            return mStoredValues[2 * vParentIdx + 1].CompareTo(mStoredValues[vParentIdx]) < 0;
        }

        /// <summary>
        /// Compares the children cells of a parent cell. -1 indicates the left child is the smaller of the two,
        /// 1 indicates the right child is the smaller of the two, 0 inidicates that neither child is smaller than the parent.
        /// </summary>
        protected virtual int CompareChild(int parentCell)
        {
            bool vLeftChildSmaller = IsLeftChildSmaller(parentCell);
            bool vRightChildIsSmaller = IsRightChildSmaller(parentCell);

            if (vLeftChildSmaller || vRightChildIsSmaller)
            {
                if (vLeftChildSmaller && vRightChildIsSmaller)
                {
                    //Figure out which of the two is smaller
                    int vLeftChildIdx = 2 * parentCell;
                    int vRightChildIdx = 2 * parentCell + 1;

                    T vLeftItem = mStoredValues[vLeftChildIdx];
                    T vRightItem = mStoredValues[vRightChildIdx];

                    //Compare the values of the children
                    if (vLeftItem.CompareTo(vRightItem) <= 0)
                    {
                        return -1;
                    }

                    return 1;

                }
                if (vLeftChildSmaller)
                {
                    return -1;
                }

                return 1;

            }

            return 0;

        }

    }

    /// <summary>
    /// An interface used by items that need to be in a Priority Queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once UnusedTypeParameter
    public interface IPriorityQueueItem<T> : IComparable
    {
        int HeapIndex { get; set; }
    }

}
