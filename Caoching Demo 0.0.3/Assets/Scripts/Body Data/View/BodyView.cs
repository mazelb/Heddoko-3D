/** 
* @file BodyView.cs
* @brief Contains the BodyView class and functionalities required to execute it The view for a body . 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Utils.DebugContext;
using Assets.Scripts.Utils.DebugContext.logging;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;



namespace Assets.Scripts.Body_Data.view
{
    /// <summary>
    /// BodyView class: Contains the view for a body and allows the body to fetch data from a pipeline fed buffer on a frame by frame basis
    /// </summary>
    public class BodyView : MonoBehaviour
    {
        public delegate void BodyFrameUpdated(BodyFrame vNewFrame);
        public event BodyFrameUpdated BodyFrameUpdatedEvent;
        //private BodyFrameBuffer mBuffer;
        private BodyFrameBuffer mBuffer;
        [SerializeField]
        private Body mAssociatedBody;
        private BodyFrame mCurreBodyFrame;
        [SerializeField]
     private bool mIsPaused;

        public bool IsPaused
        {
            get { return mIsPaused; }
            set { mIsPaused = true; }

        }
        [SerializeField]
        private bool mStartUpdating;

         
        /// <summary>
        /// Internally set the Body associated to this view. Class property that returns the Body associated with this view.
        /// </summary>
        public Body AssociatedBody
        {
            get
            {
                return mAssociatedBody;
            }
            internal set
            {
                mAssociatedBody = value;
            }
        }

        /// <summary>
        /// StartUpdating allows the  needed to start pulling data from the buffer in order to update the associated body
        /// </summary>
        /// <returns>boolean if started updating.</returns>
        public bool StartUpdating
        {
            get { return mStartUpdating; }
            set
            {
                if (value)
                {
                    mStartUpdating = false;
                }
                mStartUpdating = value;
            }
        }

        public BodyFrameBuffer Buffer
        {
            get { return mBuffer; }
        }

        /// <summary>
        /// Initialize the view with the frame buffer
        /// </summary>
        /// <param name="vAssociatedBody">the body associated to the view</param>
        /// <param name="vBuffer">the frame buffer to update from</param>
        public void Init(Body vAssociatedBody, BodyFrameBuffer vBuffer)
        {
            this.mBuffer = vBuffer;
            this.mAssociatedBody = vAssociatedBody;
        }

        /// <summary>
        /// Reset the initial frame
        /// </summary>
        /// <param name="vBodyFrame">the body frame to reset to</param>
        public void ResetInitialFrame(BodyFrame vBodyFrame = null)
        {
          
            if (mAssociatedBody != null)
            {
                BodyFrame vTempBodyFrame = null;

                if (vBodyFrame == null)
                {
                    vTempBodyFrame = mAssociatedBody.CurrentBodyFrame;
                }
                else
                {
                    vTempBodyFrame = vBodyFrame;
                }

                AssociatedBody.SetInitialFrame(vTempBodyFrame);
                UpdateViewTracking(vTempBodyFrame);
            }
        }

        /// <summary>
        /// Update view tracking
        /// </summary>
        /// <param name="vBodyFrame">the body frame to update to</param>
        public void UpdateViewTracking(BodyFrame vBodyFrame)
        {
            Profiler.BeginSample("UPDATE VIEW TRACKING");
            AssociatedBody.UpdateBody(vBodyFrame);
            Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vDic = Body.GetTracking(AssociatedBody);

            if (vDic != null)
            {
                Body.ApplyTracking(AssociatedBody, vDic); 
            }
            
            if (BodyFrameUpdatedEvent != null && vBodyFrame!= null)
            {
                Profiler.BeginSample("BODY FRAME UPDATE EVENT");
                BodyFrameUpdatedEvent(vBodyFrame);
                Profiler.EndSample();
            }
            Profiler.EndSample();

        }

        /// <summary>
        /// pause the current frame
        /// </summary>
        public void PauseFrame()
        {
            if (mAssociatedBody != null)
            {
                AssociatedBody.PauseThread();
                mIsPaused = !mIsPaused;
            }
        }

        /// <summary>
        /// Automatically called by Unity when the app is exited. Cleans up tasks and unhooks event listeners
        /// </summary>
        void OnApplicationQuit()
        {
            if (AssociatedBody != null)
            {
                AssociatedBody.StopThread();
                AssociatedBody.UnhookBrainpackListeners();
            }
        }

        /// <summary>
        /// Automatically called by Unity and if conditions are set, will update the associated body with body frame data fetched from mBuffer.
        /// </summary>
        private void Update()
        {
          
            if (StartUpdating)
            {
                if (mIsPaused)
                {
                    return;
                }

                if (mBuffer != null && mBuffer.Count > 0 && AssociatedBody.RenderedBody != null)
                {
                    BodyFrame vBodyFrame = mBuffer.Dequeue();
                    DebugLogger.Instance.LogMessage(LogType.FrameRenderingStart, "Start timestamp: " + vBodyFrame.Timestamp);
                    if (AssociatedBody.InitialBodyFrame == null)
                    {
                        ResetInitialFrame(vBodyFrame);
                    }
                    UpdateViewTracking(vBodyFrame);
                    DebugLogger.Instance.LogMessage(LogType.FrameRenderingFinish, "Finish timestamp: " + vBodyFrame.Timestamp);

                }
            }
         }

        /// <summary>
        /// Handles inputs related to the body view
        /// </summary>
        private void InputHandler()
        {
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.ResetFrame))
            {
                ResetInitialFrame();
            }
        }

        /// <summary>
        /// Automatically called by Unity when the game object awakes. In this case, look for the debug gameobject in the scene 
        /// </summary>
        private void Awake()
        {

        }

        /// <summary>
        /// Clears the buffer
        /// </summary>
        public void ClearBodyBuffer()
        {
            if (mBuffer != null)
            {
                mBuffer.Clear();
            }
        }
    }
}
