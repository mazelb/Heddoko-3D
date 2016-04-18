/** 
* @file AnalysisSubViewCamControl.cs
* @brief Contains the AnalysisSubViewCamControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using UnityEngine; 
using UnityEngine.UI;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// the camera controls inside an analysis subview
    /// </summary>
  public  class AnalysisSubViewCamControl: AbstractCameraControl
    {
        private int mCamIdx = 0;
        private bool mIsMoving = false;
        public Button MoveToNextPosButton;
        public Button MoveToPrevPosButton;
        private bool mCanZoomIn = false;
        public override void MoveToNextPosition()
        {
            int vNextPos = mCamIdx + 1;
            if (!mIsMoving)
            {
                StopAllCoroutines();
                StartCoroutine(MoveToPos(vNextPos));
            }
        }

        public override void MoveToPrevPosition()
        {
            int vNextPos = mCamIdx - 1;
            if (!mIsMoving)
            {
                StopAllCoroutines();
                StartCoroutine(MoveToPos(vNextPos));
            } 
        }

        private IEnumerator MoveToPos(int vNextPos)
        {
            Vector3 vCurrentPos = AssociatedCamera.transform.position;
            Vector3 vCenter = Target.position;//- Vector3.up;
            mIsMoving = true;
            //check the next pos
            if (vNextPos < 0)
            {
                vNextPos = mPositionsAroundTarget.Length-1;
            }
            else if (vNextPos >= mPositionsAroundTarget.Length)
            {
                vNextPos = 0;
            }

            Vector3 vStartRelativeCenter = vCurrentPos - vCenter;
            Vector3 vRelEndCenter = mPositionsAroundTarget[vNextPos] - vCenter;
           // float vStartTime = Time.time;
            float vCurrentTime = 0;
            while (mIsMoving)
            {
                LookAtTarget();
                vCurrentTime += Time.fixedDeltaTime*0.5f;//Time.time - vStartTime;
                float vPercentage = vCurrentTime / MoveSpeed;
                //smoothSlerp
                vPercentage = vPercentage*vPercentage*vPercentage*(vPercentage*(6f*vPercentage - 15f) + 10f);
                AssociatedCamera.transform.position = Vector3.Slerp(vStartRelativeCenter, vRelEndCenter, vPercentage);
                AssociatedCamera.transform.position += vCenter;
                if (vPercentage > 1)
                {
                    mIsMoving = false;
                }
                yield return null;
            }
            mCamIdx = vNextPos;

        }

        /// <summary>
        /// Release buttons and set them to null
        /// </summary>
        public void ReleaseButtons()
        {
            mCamIdx = 0;
            mIsMoving = false;
            MoveToNextPosButton.onClick.RemoveAllListeners();
            MoveToPrevPosButton.onClick.RemoveAllListeners();
            MoveToPrevPosButton = null;
            MoveToNextPosButton = null;
        }

        public void EnableZoom()
        {
     
            mCanZoomIn = true;
        }
     

        public void DisableZoom( )
        {
           
            mCanZoomIn = false;
        }

        public float zoomSpeed = 0.01f;
        void Update()
        {
            if (mCanZoomIn)
            {
                if(Input.GetKey(KeyCode.KeypadPlus))
                {
                    AssociatedCamera.orthographicSize -= zoomSpeed;
                }
               else if (Input.GetKey(KeyCode.KeypadMinus))
                {
                    AssociatedCamera.orthographicSize += zoomSpeed;
                }
                //clamp
                if (AssociatedCamera.orthographicSize <= 0.2f)
                {
                    AssociatedCamera.orthographicSize = 0.2f;
                }
                if (AssociatedCamera.orthographicSize > 4)
                {
                    AssociatedCamera.orthographicSize = 4;
                }
            }
        }
    }
}
