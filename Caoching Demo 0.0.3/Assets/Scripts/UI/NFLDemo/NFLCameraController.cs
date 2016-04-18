
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI.NFLDemo;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// this camera controller is to be used solely for the NFL tech crunch demo
    /// </summary>
    [Serializable]
    public class NFLCameraController : MonoBehaviour
    {
        public Camera Camera;
        public BezierCurve Curve;
        public CameraLookAt CamLookAt;
        [SerializeField]
        private float mLerpPercentage = 0;
        public float MovementSpeed = 2f;
        [SerializeField]
        private bool mFinishedMovingCam = true;
        /// <summary>
        /// Has the camera finished moving? public getter and private setter
        /// </summary>
        public bool FinishedMovingCam
        {
            get { return mFinishedMovingCam; }
            private set { mFinishedMovingCam = value; }
        }
        [SerializeField]
        private int mCurrCamIndex = 0;
        /// <summary>
        /// Getter that provides the current camera index, private setter
        /// </summary>
        public int CurrCamIndex
        {
            get
            {
                if (mCurrCamIndex >= Curve.pointCount)
                {
                    mCurrCamIndex = 0;
                }
                return mCurrCamIndex;
            }
            private set
            {
                mCurrCamIndex = value;
            }
        }

        private int mNextCamIndex = 1;
        /// <summary>
        /// Getter that provides the next camera index, private setter
        /// </summary>
        public int NextCamIndex
        {
            get
            {
                return mNextCamIndex;

            }
            private set
            {
                mNextCamIndex = value;
            }
        }

        private void Update()
        {

        }

        /// <summary>
        /// Move to next position
        /// </summary>
        public void MoveToNextPos()
        {
            FinishedMovingCam = false;
            StopAllCoroutines();
            StartCoroutine(MoveNext());
        }

        public void MoveTowardsStartPos()
        {
            FinishedMovingCam = false;

            StopAllCoroutines();
            StartCoroutine(MoveToStartPos());
        }
        IEnumerator MoveNext()
        {
            //Application.targetFrameRate = 60;
            float vStartTime = 0;
            float vCurrOrthoCam = Camera.orthographicSize;
            Vector3 vCurrentLookAtPos = CamLookAt.Target.position;
            CameraMovementPointSetting vNextPointSetting = null;
            while (true)
            {

                if (CurrCamIndex >= Curve.pointCount)
                {
                    CurrCamIndex = 0;
                }

                NextCamIndex = CurrCamIndex + 1;

                if (CurrCamIndex == Curve.pointCount - 1)
                {
                    NextCamIndex = 0;
                }

                vNextPointSetting =
                Curve[NextCamIndex].gameObject.GetComponent<CameraMovementPointSetting>();

                vStartTime += Time.fixedDeltaTime;
                mLerpPercentage = vStartTime / MovementSpeed;
                Vector3 vNewPosition = BezierCurve.GetPoint(Curve[CurrCamIndex], Curve[NextCamIndex], mLerpPercentage);
                float vNextOrthoSize = Mathf.Lerp(vCurrOrthoCam, vNextPointSetting.OrthographicSize, mLerpPercentage);
                Vector3 vNextLookAtPos = Vector3.Lerp(vCurrentLookAtPos, vNextPointSetting.LookAtTarget.position,
                    mLerpPercentage);

                Camera.transform.position = vNewPosition;
                Camera.orthographicSize = vNextOrthoSize;
                CamLookAt.TargetPos = vNextLookAtPos;
                CamLookAt.Target = vNextPointSetting.LookAtTarget;
                if (mLerpPercentage >= 1)
                {
                    CurrCamIndex++;
                    FinishedMovingCam = true;
                    mLerpPercentage = 0;
                    break;
                }

                yield return null;
            }
            vNextPointSetting.gameObject.SetActive(false);
            // Application.targetFrameRate = -1;
        }

        /// <summary>
        /// Interpolate towards the start position
        /// </summary>
        /// <returns></returns>
        IEnumerator MoveToStartPos()
        {
            bool vPreFollowPos = CamLookAt.UsePosition;
            CamLookAt.UsePosition = false;
       //     Application.targetFrameRate = 60;
            float vStartTime = 0;
            float vCurrOrthoCam = Camera.orthographicSize;
            Vector3 vCurrentLookAtPos = CamLookAt.Target.position;
            Vector3 vStartPosition = transform.position;
            CameraMovementPointSetting vNextPointSetting =
                 Curve[0].gameObject.GetComponent<CameraMovementPointSetting>();

            while (true)
            {
                vStartTime += Time.fixedDeltaTime;
                mLerpPercentage = vStartTime / (MovementSpeed);
                Vector3 vNewPosition = Vector3.Lerp(vStartPosition, Curve[0].position, mLerpPercentage);
                float vNextOrthoSize = Mathf.Lerp(vCurrOrthoCam, vNextPointSetting.OrthographicSize, mLerpPercentage);
                Vector3 vNextLookAtPos = Vector3.Lerp(vCurrentLookAtPos, vNextPointSetting.LookAtTarget.position,
                 mLerpPercentage);

                Camera.transform.position = vNewPosition;
                Camera.orthographicSize = vNextOrthoSize;
                CamLookAt.TargetPos = vNextLookAtPos;
                CamLookAt.Target = vNextPointSetting.LookAtTarget;
                if (mLerpPercentage >= 1)
                {
                    FinishedMovingCam = true;
                    mLerpPercentage = 0;
                    break;
                }

                yield return null;
            }
            CamLookAt.UsePosition = vPreFollowPos;
            //Application.targetFrameRate = -1;
            vNextPointSetting.gameObject.SetActive(false);
        }
        /// <summary>
        /// Returns an AnalysisView object that is held by a point on the curve. Null will be returned if an invalid index is given
        /// </summary>
        /// <param name="vIndex">the index of a point on the curve referenced by the current controller</param>
        /// <returns> returns an AnalysisView object held by a point on the curve</returns>
        public AnalysisView GetAnalysisView(int vIndex)
        {
            //invalid index
            if (vIndex >= Curve.length || vIndex < 0)
            {
                return null;
            }

            return Curve[CurrCamIndex].gameObject.GetComponent<AnalysisView>();
        }

        /// <summary>
        /// Returns an CameraMovementPointSetting object that is held by a point on the curve. Null will be returned if an invalid index is given
        /// </summary>
        /// <param name="vIndex">the index of a point on the curve referenced by the current controller</param>
        /// <returns> returns an CameraMovementPointSetting object held by a point on the curve</returns>
        public CameraMovementPointSetting GetPointAt(int vIndex)
        {
            if (vIndex >= Curve.length || vIndex < 0)
            {
                return null;
            }

            return Curve[vIndex].gameObject.GetComponent<CameraMovementPointSetting>();
        }
        /// <summary>
        /// Returns an CameraMovementPointSetting object that is held by a point on the curve. Null will be returned if an invalid index is given
        /// </summary>
        /// <param name="vIndex">the index of a point on the curve referenced by the current controller</param>
        /// <returns> returns an CameraMovementPointSetting object held by a point on the curve</returns>
        public CameraMovementPointSetting GetCurrentPointSetting()
        {
            return Curve[CurrCamIndex].gameObject.GetComponent<CameraMovementPointSetting>();
        }



        /// <summary>
        /// Reset the controller, parameters, and re-enables curve's points gameobject
        /// </summary>
        public void Reset()
        {
            StopAllCoroutines();
            mFinishedMovingCam = true;
            for (int i = 0; i < Curve.pointCount; i++)
            {
                Curve[i].gameObject.SetActive(true);
            }
            /*   for (int i = 0; i < Curve.length; i++)
               {

                   AnalysisView vAnalysisView = GetAnalysisView(i);
                   if (vAnalysisView != null)
                   {
                       vAnalysisView.Hide();
                   }
               }*/
            NextCamIndex = -1;
            CurrCamIndex = 0;
        }
    }
}
