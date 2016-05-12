// /**
// * @file TechStarsCameraControllerClosedLoop.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 05 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    public class TechStarsCameraControllerClosedLoop : MonoBehaviour
    {
        public BezierCurve ClosedLoopCurve;
        private int mCurrPos = 0;
        private int mNextPos = 0;
        public float PathingSpeed = 2f;
        private float mPercentage = 0f;
        public Camera Camera;
        [SerializeField]
        private bool mIsMoving = false;
        public Transform LookAtTarget;
        public void GoToNextPos()
        {
            mPercentage = 0;
            mCurrPos++;
            if (mCurrPos >= 3)
            {
                mCurrPos = 0;
                
            }
            mNextPos = mCurrPos + 1;
            mIsMoving = true;
        }

        public void GoToStartPos()
        {
            mPercentage = 0;
            mNextPos = 0;
            mIsMoving = true;
        }
        public void GotoPrevPos()
        {
            mPercentage = 0;
            if (mCurrPos == 0)
            { 
                mNextPos = mCurrPos -1;
                mIsMoving = true;
                return;
            }
            mCurrPos--;
            if (mCurrPos <= 0)
            {
                mCurrPos = 3;
            }
            mNextPos = mCurrPos - 1;
            mIsMoving = true;
        }
 
        /// <summary>
        /// Subroutine to move the camera
        /// </summary>
        public void MoveNext()
        {
            mPercentage += Time.fixedDeltaTime * PathingSpeed;
            if (mPercentage > 1f)
            {
                mPercentage = 1f;
                mIsMoving = false;
            }
            if (mPercentage < 1f)
            {
                Vector3 vNewPos2 = BezierCurve.GetPoint(ClosedLoopCurve[mCurrPos], ClosedLoopCurve[mNextPos], mPercentage);

                Camera.transform.position = vNewPos2;
                Camera.transform.LookAt(LookAtTarget);
            }

        }

        private void FixedUpdate()
        {
            if (mIsMoving)
            {
                MoveNext();
            }
        }
    }
}