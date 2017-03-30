// /**
// * @file AnalysisFramesUniqueCollection.cs
// * @brief Contains the AnalysisFramesUniqueCollection class
// * @author Mohammed Haider( mohammed@heddoko.com) 
// * @date 02 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;

namespace Assets.Scripts.Body_Pipeline.Analysis
{

    /// <summary>
    /// A collection set of AnalysisFrames.
    /// </summary>
    public class AnalysisFramesSet : IAnalysisFramesSet
    {
        private TPosedAnalysisFrame[] mCollection;
        public int MaxFramesCount { get; set; }
        public int TotalCollectedFrames { get; set; }
        /// <summary>
        /// Max frames reached event. 
        /// First parameter : the number of collected frames
        /// Second parameter: the number of max frames.
        /// </summary>
        public event Action<int,int> MaxFramesReachedEvent;
        /// <summary>
        /// Should be called on the basis that you know how many frames need to be collected
        /// </summary>
        /// <param name="vMaxFramesCount"></param>
        public AnalysisFramesSet(int vMaxFramesCount)
        {
            MaxFramesCount = vMaxFramesCount;
            mCollection = new TPosedAnalysisFrame[MaxFramesCount];
        }
        /// <summary>
        /// adds a tpose analysis frame with a given index
        /// <remarks>the index must of a valid type</remarks>
        /// </summary>
        /// <param name="vIndex">The given index of the analysis frame</param>
        /// <param name="vFrame">The frame to add</param>
        public void Add(int vIndex, TPosedAnalysisFrame vFrame)
        {
            if (!ValidateIndex(vIndex))
            {
                return;
            }
            if (mCollection[vIndex] == null && vFrame != null)
            {
                TotalCollectedFrames++;
            }
            mCollection[vIndex] = vFrame;
            if (TotalCollectedFrames == mCollection.Length)
            {
                if (MaxFramesReachedEvent != null)
                {
                    MaxFramesReachedEvent(TotalCollectedFrames,MaxFramesCount);
                }
            }

        }

        /// <summary>
        /// Removes and returns a frame at the given index
        /// </summary>
        /// <param name="vIndex">the index to remove the item from</param>
        /// <returns></returns>
        public TPosedAnalysisFrame Remove(int vIndex)
        {
            TPosedAnalysisFrame vFrame = Get(vIndex);
            mCollection[vIndex] = null;
            TotalCollectedFrames--;
            return vFrame;
        }
        /// <summary>
        /// Removes and returns a frame at the given index
        /// </summary>
        /// <param name="vIndex">the index to remove the item from</param>
        /// <returns></returns>
        public TPosedAnalysisFrame Get(int vIndex)
        {
            if (!ValidateIndex(vIndex))
            {
                return null;
            }
            TPosedAnalysisFrame vFrame = mCollection[vIndex];
            return vFrame;
        }

        /// <summary>
        /// Replaces the frame at the given index and returns the old frame
        /// </summary>
        /// <param name="vIndex">the index of the element</param>
        /// <param name="vNewFrame">the new frames to replace the old frame with</param>
        /// <returns>the old frame</returns>
        public TPosedAnalysisFrame Replace(int vIndex, TPosedAnalysisFrame vNewFrame)
        {
            var vItem = Remove(vIndex);
            mCollection[vIndex] = vNewFrame;
            return vItem;
        }

        /// <summary>
        /// Validates an index
        /// </summary>
        /// <param name="vIndex"></param>
        private bool ValidateIndex(int vIndex)
        {
            if (vIndex < 0)
            {
               // throw new InvalidOperationException("Index < 0, no such index ");
                return false;
            }
            if (vIndex >= MaxFramesCount)
            {
               // throw new InvalidOperationException("Index > than the max number of frames allowed, no such index ");
                return false;

            }
            return true;
        }

        /// <summary>
        /// Does the set contain  a frame at the passed index?
        /// </summary>
        /// <param name="vIndex">the index to check</param>
        /// <returns></returns>
        public bool ContainsFrameAt(int vIndex)
        {
            if (!ValidateIndex(vIndex))
            {
                return false;
            }
            var vFrame = mCollection[vIndex];
            return vFrame != null;
        }
        /// <summary>
        /// Clears out the set
        /// </summary>
        public void Clear()
        {
            for (int vI = 0; vI < mCollection.Length; vI++)
            {
                mCollection[vI] = null;
            }
        }

        /// <summary>
        /// Sets the max number of frames
        /// </summary>
        /// <param name="vCount"></param>
        public void SetMaxFrameCount(int vCount)
        {
            if (mCollection == null || mCollection.Length != vCount)
            {
                MaxFramesCount = vCount;
                mCollection = new TPosedAnalysisFrame[vCount];
            }
            else
            {
                mCollection = new TPosedAnalysisFrame[vCount];
            }
        }

        /// <summary>
        /// Returns the iterable list of the collection
        /// </summary>
        /// <returns></returns>
        public List<TPosedAnalysisFrame> GetIterableList()
        {
            return mCollection.Where(vX => vX != null).ToList();
        }


    }
}