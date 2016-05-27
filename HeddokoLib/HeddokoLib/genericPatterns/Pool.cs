/**
* @file Pool.cs
* @brief Contains the Pool class
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/
using System;
using System.Collections.Generic;

namespace HeddokoLib.genericPatterns
{
    /// <summary>
    /// A generic pool.
    /// </summary>
    /// <typeparam name="T">Must be an IReleasableResource</typeparam>
    public class Pool<T> where T: IReleasableResource
    {
        private Func<  T> mFactory; 
         private List<T> mAvailable= new List<T>();
        private List<T> mInUse = new List<T>();  
 
        /// <summary>
        /// Constructor. Needs a factory function for object instantiation
        /// </summary>
        /// <param name="vSize"></param>
        /// <param name="vFactory"></param>
        /// <param name="vLoadingMode"></param>
         public Pool(int vSize, Func<  T> vFactory, LoadingMode vLoadingMode )
        {
            if (vSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vSize), vSize, " argument must be greater than zero");
            }
            if (vFactory == null)
            {
                throw new NullReferenceException(nameof(vFactory));
            }

             mFactory = vFactory;
         
            if (vLoadingMode == LoadingMode.Eager)
            {
                PreloadItems(vSize);
            }
        }

        /// <summary>
        /// Returns an object from the pool
        /// </summary>
        /// <returns></returns>
        public T Get()
        {  
            //retrieve objects from the available list and place
            //them in unavailable.
            if (mAvailable.Count > 0)
            {
                T vPooledObj = mAvailable[0];
                mInUse.Add(vPooledObj);
                mAvailable.RemoveAt(0);
                return vPooledObj;
            }
            else
            {
                T vPooledObj = mFactory();
                mInUse.Add(vPooledObj);
                return vPooledObj;
            }
        }
        /// <summary>
        /// Releases an object back into the available pool.
        /// </summary>
        /// <param name="vItem"></param>
        public void Release(T vItem)
        {
            mAvailable.Add(vItem);
            mInUse.Remove(vItem);
            vItem.Cleanup();
        }

        /// <summary>
        /// Preload items
        /// </summary>
        /// <param name="vSize">the number of items to preload</param>
        private void PreloadItems(int vSize)
        {
            for (int i = 0; i < vSize; i++)
            {
                T vItem = mFactory();
             } 
        }
        /// <summary>
        /// The Loading mode that a pool has
        /// </summary>
        public enum LoadingMode
        {
            /// <summary>
            /// Preload the pool with the object T
            /// </summary>
            Eager,
            /// <summary>
            /// Create an object at first contact
            /// </summary>
            Lazy
        }


 


    }
}