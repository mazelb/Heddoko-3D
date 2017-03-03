using System;
using System.Collections.Generic;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    public interface IAnalysisFramesSet
    {
        int MaxFramesCount { get; set; }

        /// <summary>
        /// adds a tpose analysis frame with a given index
        /// <remarks>the index must of a valid type</remarks>
        /// </summary>
        /// <param name="vIndex">The given index of the analysis frame</param>
        /// <param name="vFrame">The frame to add</param>
        void Add(int vIndex, TPosedAnalysisFrame vFrame);

        /// <summary>
        /// Removes and returns a frame at the given index
        /// </summary>
        /// <param name="vIndex">the index to remove the item from</param>
        /// <returns></returns>
        TPosedAnalysisFrame Remove(int vIndex);

        /// <summary>
        /// Removes and returns a frame at the given index
        /// </summary>
        /// <param name="vIndex">the index to remove the item from</param>
        /// <returns></returns>
        TPosedAnalysisFrame Get(int vIndex);

        /// <summary>
        /// Replaces the frame at the given index and returns the old frame
        /// </summary>
        /// <param name="vIndex">the index of the element</param>
        /// <param name="vNewFrame">the new frames to replace the old frame with</param>
        /// <returns>the old frame</returns>
        TPosedAnalysisFrame Replace(int vIndex, TPosedAnalysisFrame vNewFrame);

        /// <summary>
        /// Does the set contain  a frame at the passed index?
        /// </summary>
        /// <param name="vIndex">the index to check</param>
        /// <returns></returns>
        bool ContainsFrameAt(int vIndex);

        /// <summary>
        /// Clears out the set
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets the max number of frames
        /// </summary>
        /// <param name="vCount"></param>
        void SetMaxFrameCount(int vCount);

        /// <summary>
        /// Returns the iterable list of the collection
        /// </summary>
        /// <returns></returns>
        List<TPosedAnalysisFrame> GetIterableList();

        /// <summary>
        /// Max frames reached event. 
        /// First parameter : the number of collected frames
        /// Second parameter: the number of max frames.
        /// </summary>
        event Action<int, int> MaxFramesReachedEvent;
    }
}