/**
* @file WriterReaderQueue.cs
* @brief Contains the WriterReaderQueue
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections.Generic;
using System.Threading;

namespace HeddokoLib.adt
{
    /// <summary>
    /// A Queue that uses a reader write lock for thread synchronization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WriterReaderQueue<T> : Queue<T>
    {
        private ReaderWriterLockSlim vReaderWriterLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Engages a write lock and enqueues an object
        /// </summary>
        /// <param name="vObj">the objec to enqueu</param>
        public  new void Enqueue(T vObj)
        {
            vReaderWriterLock.EnterWriteLock();
            base.Enqueue(vObj);
            vReaderWriterLock.ExitWriteLock();
        }

        /// <summary>
        /// Enter a reader lock and Dequeues an object and returns it back to the caller
        /// </summary>
        /// <returns>the object in the queeu</returns>
        public new T Dequeue()
        {
            T vObj;
            vReaderWriterLock.EnterReadLock();
            vObj = base.Dequeue();
            vReaderWriterLock.ExitReadLock();
            return vObj;
        }

        /// <summary>
        /// Enter a write lock and clear the queue
        /// </summary>
        public new void Clear()
        {
            vReaderWriterLock.EnterWriteLock();
            base.Clear();
            vReaderWriterLock.ExitWriteLock();
        }

        /// <summary>
        /// Enters a read lock and peeks at the beginning of the queue without remove the item
        /// </summary>
        /// <returns>the peeked object</returns>
        public new T Peek()
        {
            T vObj;
            vReaderWriterLock.EnterReadLock();
            vObj = base.Peek();
            vReaderWriterLock.ExitReadLock();
            return vObj;
        }
        public WriterReaderQueue(int capacity) : base(capacity)
        {
        }

        public new void CopyTo(T[] vArray,int vIndex)
        {
            vReaderWriterLock.EnterWriteLock();
             base.CopyTo(vArray,vIndex);
            vReaderWriterLock.ExitWriteLock();
        }

    }
}