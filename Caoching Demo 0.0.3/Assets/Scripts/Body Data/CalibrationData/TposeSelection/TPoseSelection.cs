// /**
// * @file TPoseSelection.cs
// * @brief Contains the TPoseSelection
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date September 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData.TposeSelection
{
 
    /// <summary>
    /// A model representing a selection of TPoseIndices
    /// </summary>
    public class TPoseSelection
    {
        private int mPoseIndex;
        private int mIndexRight;
        private int mIndexLeft; 
        private List<Action> mLeftActionListeners= new List<Action>(); 
        private List<Action> mRightActionListeners= new List<Action>(); 
        private List<Action> mTposeActionListeners= new List<Action>();
        public int PoseIndex
        {
            get { return mPoseIndex;}
            set
            {
                mPoseIndex = value;
                if (mTposeActionListeners.Count >0 )
                {
                    foreach (var vAction in mTposeActionListeners)
                    {
                        vAction();
                    }
                }
            }
        }
        public int PoseIndexRight
        {
            get { return mIndexRight; }
            set
            {
                mIndexRight = value;
                if (mRightActionListeners.Count > 0)
                {
                    foreach (var vAction in mRightActionListeners)
                    {
                        vAction();
                    }
                }
            }
        }
        public int PoseIndexLeft
        {
            get { return mIndexLeft; }
            set
            {
                mIndexLeft = value;
                if (mLeftActionListeners.Count > 0)
                {
                    foreach (var vAction in mLeftActionListeners)
                    {
                        vAction();
                    }
                }
            }
        }

        public void RemoveAllListeners()
        {
            mLeftActionListeners.Clear();
            mRightActionListeners.Clear();
            mTposeActionListeners.Clear();
        }

        public void AddLeftIndexListener(Action vAction)
        {
            mLeftActionListeners.Add(vAction);
        }

        public void AddRightIndexListener(Action vAction)
        {
            mRightActionListeners.Add(vAction);
        }

        public void AddMainIndexListener(Action vAction)
        {
            mTposeActionListeners.Add(vAction);

        }
    }
}