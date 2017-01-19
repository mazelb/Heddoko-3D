// /**
// * @file SensorContainer.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using heddoko;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Body_Data.View
{
    public class SensorContainer : MonoBehaviour
    {
        [SerializeField]
        private List<SensorTransform> mSensorTransformList = new List<SensorTransform>();

        private ParticlePoolManager mRedPool;
        private ParticlePoolManager mWhitePool;
        public ParticleSystemDisabler WhitePref;
        public ParticleSystemDisabler RedPref;
        public Action<int> SensorSelected;
        public Action<int> SensorDeselected;

        void Awake()
        {
            mRedPool = new ParticlePoolManager(18, RedPref);
            mWhitePool = new ParticlePoolManager(18, WhitePref);
            List<SensorTransform> vSortedList = mSensorTransformList.OrderBy(o => o.SensorPos).ToList();
            mSensorTransformList = vSortedList;
        }
        void OnEnable()
        {

            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SensorClicked += SetHighlightToObject;
                mSensorTransformList[i].SetPools(mRedPool, mWhitePool);
            }
        }


        public void ChangeState(int vSensorIndex, bool vCalStatus, bool vMagneticTransience)
        {
            mSensorTransformList[vSensorIndex].UpdateState(vCalStatus, vMagneticTransience);
        }

        private void SetHighlightToObject(int vArg0)
        {
            //If it's highlighted, show all objects
            if (mSensorTransformList[vArg0].IsHighlighted)
            {
                //remove the highlight from this object
                mSensorTransformList[vArg0].HighlightObject(false);
                for (int i = 0; i < mSensorTransformList.Count; i++)
                {
                    mSensorTransformList[i].Show();
                }
                if (SensorDeselected != null)
                {
                    SensorDeselected(vArg0);
                }
            }
            else
            {
                for (int i = 0; i < mSensorTransformList.Count; i++)
                {
                    mSensorTransformList[i].Hide();
                }
                mSensorTransformList[vArg0].Show();
                mSensorTransformList[vArg0].HighlightObject(true);
                if (SensorSelected != null)
                {
                    SensorSelected(vArg0);
                }
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SensorClicked -= SetHighlightToObject;
            }
        }


        public void Hide()
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].Hide();
            }
        }


        public void Show()
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].Show();
            }
        }

        /// <summary>
        /// Set the layer of the gameobject
        /// </summary>
        /// <param name="vCurrLayerMask"></param>
        public void SetLayer(LayerMask vCurrLayerMask)
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SetLayer(vCurrLayerMask);
            }
            mRedPool.SetLayer(vCurrLayerMask);
            mWhitePool.SetLayer(vCurrLayerMask);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SetSensorsSpatialPosition(-1);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                SetSensorsSpatialPosition(1);
            }
        }

        /// <summary>
        /// Sets the spatial position of the sensors according to a passed in direction
        /// </summary>
        /// <param name="vDirection"></param>
        public void SetSensorsSpatialPosition(int vDirection)
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].ChangePosition(vDirection);
            }
        }

        public void UpdateSensorOrientation(Packet vPacket)
        {
            var vImuFrames = vPacket.fullDataFrame.imuDataFrame;
            for (int vI = 0; vI < vImuFrames.Count; vI++)
            {
                int vIdx = (int)vImuFrames[vI].imuId;
                mSensorTransformList[vIdx].UpdateRotation(vImuFrames[vI]);
            }

            //uint calStable = (dataFrame.sensorMask >> 19) & 0x01;
            //uint magTransient = (dataFrame.sensorMask >> 20) & 0x01;
        }


    }
}