// /**
// * @file SensorTransform.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Deployment.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// copies the behaviour of an associated subsegment transform.
    /// </summary>
    public class SensorTransform : MonoBehaviour, IPointerClickHandler
    {
        public Transform OrientationToCopy;
        private bool mIsHidden = true;
        public AxisViewContainer Axis;
        public GameObject[] ListenerLayerobjects;
        public Vector3[] Positions;
        private int mSpatialPos = 0;
        public int SensorPos;
        public UnityAction<int> SensorClicked;
        
        public GameObject[] HighlightedGameObjects;
        private bool mIsHighlighted;
        public bool IsHighlighted
        {
            get { return mIsHighlighted; }
        }
        /// <summary>
        /// Hide the sensor
        /// </summary>
        public void Hide()
        {
            mIsHidden = true;
            HighlightObject(false);
            gameObject.SetActive(false);
        }

        public void HighlightObject(bool vFlag)
        {
            for (int i = 0; i < HighlightedGameObjects.Length; i++)
            {
                HighlightedGameObjects[i].SetActive(vFlag);
            }
            mIsHighlighted = vFlag;
        }

        public void Show()
        {
            mIsHidden = false;
            HighlightObject(false);
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (!mIsHidden)
            {
                transform.rotation = OrientationToCopy.rotation;
            }
        }

        /// <summary>
        /// Changes the spatial position of the sensor
        /// </summary>
        /// <param name="vDirection"></param>
        public void ChangePosition(int vDirection)
        {

            mSpatialPos += vDirection;
            if (mSpatialPos <0)
            {
                mSpatialPos = Positions.Length - 1;
            }

            if (mSpatialPos >= Positions.Length)
            {
                mSpatialPos = 0;
            }    
            transform.localPosition = Positions[mSpatialPos];
        }

        /// <summary>
        /// Set the layer mask that the object is on.
        /// </summary>
        /// <param name="vCurrLayerMask"></param>

        public void SetLayer(LayerMask vCurrLayerMask)
        {
            gameObject.layer = vCurrLayerMask;
            Axis.SetLayer (vCurrLayerMask);
            if (ListenerLayerobjects != null)
            {
                for (int i = 0; i < ListenerLayerobjects.Length; i++)
                {
                    ListenerLayerobjects[i].layer = vCurrLayerMask;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (SensorClicked != null)
            {
                SensorClicked(SensorPos);
            }
        }
    }
}