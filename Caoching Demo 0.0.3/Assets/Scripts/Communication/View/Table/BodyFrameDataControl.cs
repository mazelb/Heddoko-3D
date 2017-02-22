/** 
* @file BodyFrameDataControl.cs
* @brief Contains the BodyFrameDataControl class 
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Communication.View.Table
{
    /// <summary>
    /// Controls the Bodyframe data list view. Listens to connections events from the brainpack controller
    /// </summary>
    public class BodyFrameDataControl : MonoBehaviour
    {

        private Body mBody;
        public BrainpackDataList DataList;
        public const int GMaxItems = 250;
        public Button PauseButton;
        public bool IsPaused = false;
        private List<BodyFrame> mBodyFrameCache = new List<BodyFrame>();


        public void Start()
        {
            PauseButton.onClick.AddListener(() => IsPaused = !IsPaused);
        }

        /// <summary>
        /// When a new body frame is updated
        /// </summary>
        /// <param name="vNewFrame"></param>
        private void BodyFrameUpdatedEvent(BodyFrame vNewFrame)
        {
            if (!IsPaused)
            {
                // add a new frame to the data souce
                DataList.DataSource.Add(vNewFrame);
                mBodyFrameCache.Add(vNewFrame);
                //Optimization. run this once the number of items of the data source list is greater than a predefined max items
                if (DataList.DataSource.Count > GMaxItems)
                {
                    int vMaxVisibleItems = DataList.MaxDisplayCount;
                    DataList.DataSource.Clear();
                    IEnumerable<BodyFrame> vEnumerable = mBodyFrameCache.GetRange(mBodyFrameCache.Count - vMaxVisibleItems,
                        vMaxVisibleItems);
                    DataList.DataSource.AddRange(vEnumerable);
                    mBodyFrameCache.Clear();
                }

                DataList.ScrollToIndex(DataList.DataSource.Count - 1);

            }
        }



        public void OnApplicationQuit()
        {
            if (mBody != null)
            {
                mBody.View.BodyFrameUpdatedEvent -= BodyFrameUpdatedEvent;
            }
        }


        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            DataList.DataSource.Clear();
            mBodyFrameCache.Clear();
        }

        /// <summary>
        /// Set the data source body
        /// </summary>
        /// <param name="vNewBody"></param>
        public void SetBody(Body vNewBody)
        {
            if (gameObject.activeInHierarchy)
            {
                if (mBody != null)
                {
                    mBody.View.BodyFrameUpdatedEvent -= BodyFrameUpdatedEvent;
                }
                Clear();
                mBody = vNewBody;
                mBody.View.BodyFrameUpdatedEvent += BodyFrameUpdatedEvent;
            }
          
        }
    }
}