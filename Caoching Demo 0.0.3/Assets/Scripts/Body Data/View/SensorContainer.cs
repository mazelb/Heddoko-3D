// /**
// * @file SensorContainer.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class SensorContainer
    {
        [SerializeField]
        private List<SensorTransform> mSensorTransformList = new List<SensorTransform>();

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

        public void SetLayer(LayerMask vCurrLayerMask)
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SetLayer(vCurrLayerMask);
            }
        }
    }
}