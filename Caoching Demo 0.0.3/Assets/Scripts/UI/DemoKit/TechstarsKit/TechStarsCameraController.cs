/**
* @file TechStarsCameraController.cs
* @brief Contains the 
* @author Mohammed Haider( 
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    public class TechStarsCameraController : MonoBehaviour
    {
        public Camera Camera;
        public bool PathForward;
        public bool PathBackward;
        public bool PathRight;
        private bool mIsOnRight;

        public float PathingSpeed = 2f;
        private float mPercentage = 0f;
        public Transform LookAtTarget;
        public BezierCurve PathToSide;
        public BezierCurve PathToright;
        private void MovePanelCameraTowardsView()
        {
            mPercentage += Time.fixedDeltaTime * PathingSpeed;
            if (mPercentage > 1f)
            {
                mPercentage = 1f;
            }
            if (mPercentage < 1f)
            {
                Vector3 vNewPos2 = PathToSide.GetPointAt(mPercentage);

                Camera.transform.position = vNewPos2;
                Camera.transform.LookAt(LookAtTarget);
            }

        }

        private void MoveCameraBackToRightPos()
        {

            mPercentage -= Time.fixedDeltaTime * PathingSpeed;
            if (mPercentage < 0f)
            {
                mPercentage = 0f;
            }
            if (mPercentage > 0)
            {
                Vector3 vNewPos2 = PathToright.GetPointAt(mPercentage);
                Camera.transform.position = vNewPos2;
                Camera.transform.LookAt(LookAtTarget);
            }
        }
        private void MoveCameraBackToOriginalPos()
        {
            mPercentage -= Time.fixedDeltaTime * PathingSpeed;
            if (mPercentage < 0f)
            {
                mPercentage = 0f;
            }
            if (mPercentage > 0)
            {
                Vector3 vNewPos2 = PathToSide.GetPointAt(mPercentage);
                Camera.transform.position = vNewPos2;
                Camera.transform.LookAt(LookAtTarget);
            }

        }
        void FixedUpdate()
        {
            if (PathForward)
            {
                MovePanelCameraTowardsView();
            }
            else if (PathBackward)
            {
                MoveCameraBackToOriginalPos();
            }
            else if(PathToright)
            {
                MoveCameraBackToRightPos();
            }
        }
    }
}