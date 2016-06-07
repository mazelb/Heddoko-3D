// /**
// * @file EventSet.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;

namespace HeddokoLib.adt
{
    /// <summary>
    /// this class is to provide type safety and code maintainability when using EventSet
    /// </summary>
    public sealed class EventKey : Object
    {
        
    }

    /// <summary>
    /// A Data structure that holds a set of event handlers
    /// </summary>
    public class EventSet
    {
        /// <summary>
        /// The set of events, mapped by their specifed event keys
        /// </summary>
        private readonly Dictionary<EventKey, Delegate> mEventSet = new Dictionary<EventKey, Delegate>();

        /// <summary>
        /// Adds an event handler by the specified key
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vHandler"></param>
        public void Add(EventKey vKey, Delegate vHandler)
        {
            Delegate vDel;
            mEventSet.TryGetValue(vKey, out vDel);
            mEventSet[vKey] =Delegate.Combine(vDel,vHandler);
        }

        /// <summary>
        /// Removes an event handler by the specified key
        /// </summary> 
        /// <param name="vKey">The key</param>
        /// <param name="vHandler">The event handler</param>
        public void Remove(EventKey vKey, Delegate vHandler)
        {
            Delegate vDel;
            if (mEventSet.TryGetValue(vKey, out vDel))
            {
                vDel = Delegate.Remove(vDel,vHandler);
            }

            //after remove, verify if there are no more elements to be found at the specified key. 
            if (vDel != null)
            {
                mEventSet[vKey] = vDel;
            }
            else
            {
                mEventSet.Remove(vKey);
            }
        }

        /// <summary>
        /// Raise the event for the specified key
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vSender"></param>
        /// <param name="vEventArgs"></param>
        public void Raise(EventKey vKey, Object vSender, EventArgs vEventArgs)
        {
            Delegate vDel;
            mEventSet.TryGetValue(vKey, out vDel);
            if (vDel != null)
            {
                vDel.DynamicInvoke(vSender, vEventArgs);
            }
        }


    }
}