using System;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    public interface IAnalysisFramesSetfda
    {
        /// <summary>
        /// Max frames reached event. 
        /// First parameter : the number of collected frames
        /// Second parameter: the number of max frames.
        /// </summary>
        event Action<int,int> MaxFramesReachedEvent;
    }
}