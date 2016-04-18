/** 
* @file CacheResults.cs
* @brief Contains the CacheResults class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// A cached result
    /// </summary> 
    /// <typeparam name="T2"></typeparam>
    public class CacheResults<T2>
    {
        //private Dictionary<T1,T2> mCache = new Dictionary<T1, T2>();
        private Dictionary<string, T2> mCache = new Dictionary<string, T2>();
        private SortedDictionary<DateTime, string> mAccessTime = new SortedDictionary<DateTime, string>();

        private int mCapacity = 2000;
        public const int GMinCapacity = 50;

        /// <summary>
        /// the capacity of the cache. If the cache is set below the minimum capacity,
        /// it is forced to the minimum capacity
        /// </summary>
        public int Capacity
        {
            get { return mCapacity; }
            set
            {
                int vVerification = value;
                if (vVerification < GMinCapacity)
                {
                    vVerification = GMinCapacity;
                }
                mCapacity = vVerification;
            }
        }


        /// <summary>
        /// Inserts a value into the cache. Verifies if capacity has been reached
        /// pops the element that was the least recently accessed
        /// </summary>
        /// <param name="vKey">the key</param>
        /// <param name="vValue">the value</param> 
        public void Insert(string vKey, T2 vValue)
        {
            //check if capacity has been reached
            if (mCache.Count == Capacity - 1)
            {
                //remove element
                //Find which elements is the oldest
                //  private SortedDictionary<DateTime,T1> mAccessTime = new SortedDictionary<DateTime,T1>();
                DateTime vFirstElement = mAccessTime.First().Key;
                string vFoundElem = mAccessTime[vFirstElement];
                //remove the element from the sorted dictionary
                mAccessTime.Remove(vFirstElement);
                //remove it from the cache
                mCache.Remove(vFoundElem);
            }
            DateTime vNow = DateTime.Now;
            mAccessTime.Add(vNow, vKey);
            mCache.Add(vKey, vValue);

        }

        /// <summary>
        /// Does the cache contain the key?
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        public bool Contains(string vKey)
        {
            return mCache.ContainsKey(vKey);
        }

 /*       /// <summary>
        /// Finds a partial result from the cache
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        public List<T2> FindPartialResults(string vKey)
        {

        }*/


    }
}
