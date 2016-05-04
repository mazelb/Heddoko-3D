/*@file CircularQueue.cs
* @brief Contains the CircularQueue<T> class class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace HeddokoLib.adt
{

    /**
    * BodyFrameThread class 
    * @brief The CircularQueue class accepts a generic type and operates asynchrounously, that is data is consumed only as quickly as the consumer
    *        can consume it, and produced when a producer produces it.  When inserting a new item, the oldest value will be overwritten, however 
    *       this can be alleviated by setting the AllowOverflow property to false. Enqueuing a filled queue will result in a
    *      CircularQueueOverflowException, so the programmer needs to handle this exception(perhaps wait until the queue is cleared?)
    */
    public class CircularQueue<T> : IEnumerable<T>, ICollection<T>
    {
        private int mCapacity; //how large is the queue?
        private int mCount;
        private int mHeadIndex;
        private int mTailIndex;
        private T[] mQueue;
        private const int mDefaultCapacity = 64; //default size to use in the parameterles constructor 
        #region properties
        public bool AllowOverflow { get; set; }
        // private object mQueueLock = new object();
        private ReaderWriterLock mLock = new ReaderWriterLock();
        /// <summary>
        /// lock placed on Count to prevent the count from being manipulated by multiple threads
        /// </summary>
        private object mCountLock = new object();
        public int Capacity
        {
            get
            {
                return mCapacity;
            }
            set
            {
                mLock.AcquireWriterLock(250);
                if (value == mCapacity)
                    return;
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException("Trying to resize the queue to a size smaller than the current count of " + Count);
                }
                var temp = new T[value];
                if (Count > 0)
                {
                    CopyTo(temp);
                }
                mQueue = temp;
                mCapacity = value;
                mLock.ReleaseWriterLock();
            }
        }

        public int Count
        {
            get
            {
                return mCount;
            }
            private set
            {
                lock (mCountLock)
                {
                    mCount = value;
                }
            }
        }

        public T[] CirclQueue
        {
            get
            {
                return mQueue;
            }
        }

        #endregion
        #region constructors
        /**
        * CircularQueue()
        * @brief Default constructor, sets the capacity to 64 and doesn't allow overflow
        * @param  
        * @return a new CircularQueueObject
        */
        public CircularQueue()
            : this(mDefaultCapacity, false)
        {

        }
        /**
        * CircularQueue(bool vAllowOverflow)
        * @brief Constructor, sets the capacity to 64, and allow overflow is set according to the parameter passed in 
        * @param  allowOverflow
        * @return a new CircularQueueObject
        */
        public CircularQueue(bool vAllowOverflow)
            : this(mDefaultCapacity, vAllowOverflow)
        {

        }
        /**
        * CircularQueue(bool vCapacity)
        * @brief Constructor, sets the capacity according to the paramater passed in and doesn't allow overflow 
        * @param  capacity
        * @return a new CircularQueueObject
        */
        public CircularQueue(int vCapacity)
            : this(vCapacity, false)
        {

        }

        /**
        * CircularQueue(int vCapacity, bool vAllowOverflow)
        * @brief Constructor, sets the capacity according to the paramater passed in and sets overflow according to the allowOverflow parameter passed in
        * @param  capacity, allowOverflow
        * @return a new CircularQueueObject
        */
        public CircularQueue(int vCapacity, bool vAllowOverflow)
        {
            if (vCapacity < 0)
            {
                throw new EmptyCircularQueueException();
            }
            this.mCapacity = vCapacity;
            Count = 0;
            mHeadIndex = 0;
            mTailIndex = 0;
            mQueue = new T[vCapacity];
            AllowOverflow = vAllowOverflow;
        }
        #endregion
        /**
        * Contains(T vObj)
        * @brief Does the circular queue contain the object?
        * @param  obj
        * @return false if  object isn't found
        */
        public bool Contains(T vObj)
        {
            int vQueueIndex = mHeadIndex;
            var vComparer = EqualityComparer<T>.Default;
            for (int i = 0; i < Count; i++, vQueueIndex++)
            {
                if (vQueueIndex == mCapacity)
                {
                    vQueueIndex = 0;
                }
                if (vObj == null && mQueue[vQueueIndex] == null)
                {
                    return true;
                }
                else if ((mQueue[vQueueIndex] != null) && vComparer.Equals(mQueue[vQueueIndex], vObj))
                {
                    return true;
                }
            }
            return false;
        }
        /**
        * Clear()
        * @brief  clears the circular queue 
        * @param  
        * @return  
        */
        public void Clear()
        {
            mLock.AcquireWriterLock(250);
            Count = 0;
            mHeadIndex = 0;
            mTailIndex = 0;
            for (int i = 0; i < mQueue.Length; i++)
            {
                mQueue[i] = default(T);
            }
            mLock.ReleaseWriterLock();
        }
        /**
        * Enqueue(T[] vA)
        * @brief  Enqueues an array of the same type as contained in the circular queue
        * @param  T[] vA
        * @return  number of items that were placed in the circular queue 
        */
        public int Enqueue(T[] vA)
        {
            return Enqueue(vA, 0, vA.Length);
        }
        /**
        * Enqueue(T[] tA, int offset, int length)
        * @brief  Enqueues an array of the same type as contained in the circular queue, starting from the offset, for a length of the array
        * @note If the AllowOverflow flag is set to false and the number of element that want to be placed in the queue is greater than the 
        * number of available slots then this method will throw a CircularQueueOverflowException
        * @param    tA,  offset, length
        * @return  number of items that were placed in the circular queue 
        */
        public int Enqueue(T[] vA, int vOffset, int vLength)
        {
            if (!AllowOverflow && vLength > Capacity - Count)
            {
                throw new CircularQueueOverflowException();
            }

            int startIndex = vOffset;
            for (int i = 0; i < vLength; i++, mTailIndex++, startIndex++)
            {
                if (mTailIndex == mCapacity)
                {
                    mTailIndex = 0;
                }
                mQueue[mTailIndex] = vA[startIndex];
            }

            Count = Math.Min(Count + vLength, mCapacity);
            return Count;
        }
        /**
        * Enqueue(T vItem)
        * @brief  Enqueues an item in the circular queue
        * @note If the AllowOverflow flag is set to false and the circular queue is full, then a CircularQueueOverflowException is thrown
        * @param item
        * @return 
        */
        public void Enqueue(T vItem)
        {
            if (!AllowOverflow && IsFull())
            {
                throw new CircularQueueOverflowException();
            }

            try
            {
                mLock.AcquireWriterLock(250);
                {
                    mQueue[mTailIndex] = vItem;
                    //Verify that the tail index isn't at capacity, otherwise set it zero so it points to the beginning of the array
                    if (++mTailIndex == mCapacity)
                    {
                        mTailIndex = 0;
                    }

                    //only increment the count if its less  than capacity
                    if (Count < Capacity)
                    {
                        Count++;
                    }
                }

            }


            catch (Exception)
            {


            }
            finally
            {
                mLock.ReleaseLock();
            }


        }
        /**
        * IsFull( )
        * @brief Checks if the circular queue is full  
        * @param 
        * @return true if capacity is reached
        */
        public bool IsFull()
        {
            return Count == mCapacity;
        }

        /**
        * Skip(int nAmount)
        * @brief Moves the head of circular buffer by nAmount
        * @note if the amount is larger than the number of actual elements in the buffer, then wrap the skip by count -1
        * @param nAmount
        * @return  
        */
        public void Skip(int nAmount)
        {
            mHeadIndex += nAmount;
            if (mHeadIndex >= Count)
            {
                mHeadIndex -= Count - 1; //wrap
            }
        }


        /**
        * Dequeue(T[] tA)
        * @brief Reference an array of type T and Dequeue the contents of the circular queue into this array 
        * @param  A
        * @return  the number of elements that were dequeued
        */
        public int Dequeue(ref T[] tA)
        {
            return Dequeue(ref tA, 0, tA.Length);
        }
        /**
        *  Dequeue(ref T[] tA, int offset, int length)
        * @brief Reference an array of type T and Dequeue the contents of the circular queue into this array, starting at the offset for a set length
        * @param tA,offset,length
        * @return the number of elements that were dequeued
        */
        public int Dequeue(ref T[] tA, int offset, int length)
        {
            int actualCount = Math.Min(length, Count);
            int startIndex = offset;
            //   lock (mQueueLock)
            //     {
            try
            {
                mLock.AcquireWriterLock(250);
                //    lock (mQueueLock)
                {
                    for (int i = 0; i < actualCount; i++, mHeadIndex++, startIndex++)
                    {
                        if (mHeadIndex == mCapacity)
                        {
                            mHeadIndex = 0;
                        }
                        tA[startIndex] = mQueue[mHeadIndex];
                    }
                    Count -= actualCount;
                }

            }
            catch
            {

            }
            finally
            {
                mLock.ReleaseLock();
            }

            //  }

            return actualCount;
        }

        /**
        * Dequeue()
        * @brief Dequeues an element from the circular queue 
        * @note Throws an EmptyCircularQueueException when empty.
        * @param  
        * @return the element that was dequeued
        */
        public T Dequeue()
        {
            var vItem = default(T);
            try
            {
                mLock.AcquireWriterLock(250);

                if (Count == 0)
                {
                    throw new EmptyCircularQueueException();
                }

                var vOverWritten = default(T);
                // lock (mQueueLock)
                //   {
                vItem = mQueue[mHeadIndex];
                mQueue[mHeadIndex] = default(T);
                if (++mHeadIndex == mCapacity)
                {
                    mHeadIndex = 0;
                }
                Count--;
            }
            catch
            {

            }
            finally
            {
                mLock.ReleaseLock();
            }

            //  }

            return vItem;
        }
        /**
        * Peek()
        * @brief Peek at the head of the circular queue, without moving the head
        * @note Throws an EmptyCircularQueueException when empty.
        * @return the element that was peeked
        */
        public T Peek()
        {
            if (Count == 0)
            {
                throw new EmptyCircularQueueException();
            }
            var item = mQueue[mHeadIndex];
            return item;
        }

        /// <summary>
        /// Resize the buffer with a new capactiy. 
        /// </summary>
        /// <param name="vNewCapacity"></param>
        public void Resize(int vNewCapacity)
        {

        }
        #region copy buffered queue to array 
        /**
        * CopyTo(  T[] vA)
        * @brief  Copy the circular queue into the requested array tA without modifying the circular queue
        * @param  T[] vA: the array to copy the CircularQueue into
        */
        public void CopyTo(T[] vA)
        {
            CopyTo(vA, 0);
        }
        /**
        * CopyTo(  T[] vA, int vIndex)
        * @brief  Copy the circular queue into the requested array tA without modifying the circular queue
        * @param  T[] vA: : the array to copy the CircularQueue into ,int vIndex: the index where to start copying into
        */
        public void CopyTo(T[] vA, int vIndex)
        {
            CopyTo(vA, vIndex, Count);
        }
        /**
        * CopyTo(  T[] vA, int vArrayIndex, int vLength)
        * @brief  Copy the circular queue into the requested array tA without modifying the circular queue
        * @param  T[] vA: : the array to copy the CircularQueue into ,int vIndex: the index where to start copying into, 
        * vLength the length of the circular queue to copy
        * @note: Will throw an argument out of range exception if requested length is larger than the number of elements in the queue
        */
        public void CopyTo(T[] vA, int vArrayIndex, int vLength)
        {
            if (vLength > Count)
            {
                throw new ArgumentOutOfRangeException("Length requested is too large");
            }

            int queueindex = mHeadIndex;
            // lock (mQueueLock)
            //    {
            try
            {
                mLock.AcquireWriterLock(250);
                for (int i = 0; i < Count; i++, queueindex++, vArrayIndex++)
                {
                    if (queueindex == mCapacity)
                    {
                        queueindex = 0;
                    }
                    vA[vArrayIndex] = mQueue[queueindex];
                }
            }
            catch
            {

            }
            finally
            {
                mLock.ReleaseLock();
            }

            //   }

        }

        #endregion
        /**
        * GetEnumerator()
        * @brief  An IEnumerator object that can be used to iterate through the CircularQueue.
        * @return An IEnumerator object
        */
        public IEnumerator<T> GetEnumerator()
        {
            int vQueueIndex = mHeadIndex;
            for (int i = 0; i < Count; i++, vQueueIndex++)
            {
                if (vQueueIndex == mCapacity)
                {
                    vQueueIndex = 0;
                }
                yield return mQueue[vQueueIndex];
            }
        }
        /**
        * ToArray()
        * @brief  returns the CircularQueue as an array.
        * @return An array representation of the circular queue , starting at the head index and terminating at the 
        * tail index
        */
        public T[] ToArray()
        {
            var vT = new T[Count];
            CopyTo(vT);
            return vT;
        }

        /**
        * Count
        * @brief Returns the count of the circular queue
        * @return the total count of element in the circular queue
        */
        int ICollection<T>.Count
        {
            get
            {
                return Count;
            }
        }
        /**
        * IsReadOnly
        * @brief Returns if the collection is readonly
        * @return false
        */
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /**
        * Add(T vItem)
        * @brief Adds an item to the collection
        * @param T vItem: The item to be added
        */
        void ICollection<T>.Add(T vItem)
        {
            Enqueue(vItem);
        }
        /**
        * Remove(T item)
        * @brief removes  an item from the collection
        * @param T vItem: The item to be removed
        * @return returns if the item was successfully removed
        */
        bool ICollection<T>.Remove(T item)
        {
            if (Count == 0)
            {
                return false;
            }
            Dequeue();
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class EmptyCircularQueueException : Exception
    {

    }

    public class CircularQueueOverflowException : Exception
    {

    }
}
