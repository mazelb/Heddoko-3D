// /**
// * @file AirplaneSensorColor.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com  )
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// The air plane's wings
    /// </summary>
    public class AirplaneSensorColor : MonoBehaviour
    {

        public Color GoodState;
        public Color Neutral;
        public Color BadState;
        private SensorState mCurrState;
        private Material mCurrMat;
        private MeshRenderer[] mMeshes;

        private MeshRenderer[] Meshes
        {
            get
            {
                if (mMeshes == null)
                {
                    mMeshes = GetComponentsInChildren<MeshRenderer>();
                }
                return mMeshes;
            }
        }

        private Material Currmat
        {
            get
            {
                if (mCurrMat == null)
                {
                    mCurrMat = new Material(FirstMeshRenderChild.material);
                    Neutral = mCurrMat.color;
                    FirstMeshRenderChild.material = mCurrMat;
                    foreach (var vMeshRenderer in Meshes)
                    {
                        vMeshRenderer.material = mCurrMat;
                    }
                    
                }
                return mCurrMat;
            }
        }
        /// <summary>
        /// this will give us the first material to use a template for 
        /// </summary>
        public MeshRenderer FirstMeshRenderChild;
        void Awake()
        {
        }
        /// <summary>
        /// Reset the current view state to neutral
        /// </summary>
        public void ResetState()
        {
            SetState(SensorState.Neutral);
        }
        public void SetState(SensorState vState)
        {
            if (vState != mCurrState)
            {
                switch (vState)
                {
                    case SensorState.Good:
                        Currmat.color = GoodState;
                        break;
                    case SensorState.Bad:
                        Currmat.color = BadState;
                        break;
                    case SensorState.Neutral:
                        Currmat.color = Neutral;
                        break;
                } 
                mCurrState = vState;
            }
        }
        public enum SensorState
        {
            Neutral,
            Good,
            Bad
        }
    }

}