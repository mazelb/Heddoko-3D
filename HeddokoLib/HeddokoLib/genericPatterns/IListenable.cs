 /**
 * @file IListenable.cs
 * @brief Contains the IListenable interface
 * @author Mohammed Haider( mohammed@ heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using System;

namespace HeddokoLib.genericPatterns
{
    public interface IListenable
    {
        /// <summary>
        /// Adds a listener
        /// </summary>
        /// <param name="vDel">The delegated listener</param>
        void AddListener(Delegate vDel);

        /// <summary>
        /// Removes a listener
        /// </summary>
        /// <param name="vDel">The delegated listener to remove</param>
        void RemoveListener(Delegate vDel);
    
        /// <summary>
        /// Notifies all listeners
        /// </summary>
         void Notify(object vSender,object vArgs);
    }
}