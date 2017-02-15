// /**
// * @file LiveAnalysisFramesSet.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 02 2017
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels;

namespace Assets.Scripts.Frames_Pipeline
{
    /// <summary>
    /// A concrete impletation of IAnalysisFramesSet, to be used with a live session
    /// </summary>
    public class LiveAnalysisFramesSet : IAnalysisFramesSet
    {
    //    private Dictionary<int,TPosedAnalysisFrame> mCollection = new Dictionary<int,TPosedAnalysisFrame>(); 
        private List<TPosedAnalysisFrame > mCollection = new List<TPosedAnalysisFrame>();
        public int MaxFramesCount { get; set; }
        public void Add(int vIndex, TPosedAnalysisFrame vFrame)
        {
           mCollection.Add( vFrame);
        }

        public TPosedAnalysisFrame Remove(int vIndex)
        {
            
            return null;
        }

        public TPosedAnalysisFrame Get(int vIndex)
        {
            if (mCollection.Count == 0)
            {
                return null;
            }
            else
            {
                return mCollection[mCollection.Count - 1];
            }
        }

        public TPosedAnalysisFrame Replace(int vIndex, TPosedAnalysisFrame vNewFrame)
        {
            return null;

        }

        public bool ContainsFrameAt(int vIndex)
        {
            return false;
        }

        public void Clear()
        {
            mCollection.Clear();
        }

        public void SetMaxFrameCount(int vCount)
        {
          
        }

        public List<TPosedAnalysisFrame> GetIterableList()
        {
            return mCollection;
        }

        public event Action<int, int> MaxFramesReachedEvent;
    }
}