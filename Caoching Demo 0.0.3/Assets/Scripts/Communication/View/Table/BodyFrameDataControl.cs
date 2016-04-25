/** 
* @file BodyFrameDataControl.cs
* @brief Contains the BodyFrameDataControl class 
* @date April 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
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
        public int MaxItems = 15;
        public Button PauseButton;
        private bool IsPaused=false;

        public void Start()
        {
            mBody = BodiesManager.Instance.GetBodyFromUUID("BrainpackPlaceholderBody");
            mBody.View.BodyFrameUpdatedEvent += BodyFrameUpdatedEvent;
            PauseButton.onClick.AddListener(()=> IsPaused = !IsPaused);
        }

        /// <summary>
        /// When a new body frame is updated
        /// </summary>
        /// <param name="vNewFrame"></param>
        private void BodyFrameUpdatedEvent(BodyFrame vNewFrame)
        {
            if (!IsPaused)
            {
                if (DataList.DataSource.Count < MaxItems)
                {
                    DataList.DataSource.Add(vNewFrame);
                }
                else
                {
                    DataList.DataSource.RemoveAt(0);
                    DataList.DataSource.Add(vNewFrame);
                }
            }
        }

        public void OnApplicationQuit()
        {
            mBody.View.BodyFrameUpdatedEvent -= BodyFrameUpdatedEvent;
        }
    }
}