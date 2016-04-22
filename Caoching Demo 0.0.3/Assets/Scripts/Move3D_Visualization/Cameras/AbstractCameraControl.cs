/** 
* @file AbstractCameraControl.cs
* @brief Contains the AbstractCameraControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Provides an abstract for camera controls
    /// </summary>
    public class AbstractCameraControl : MonoBehaviour
    {
        private Transform mTarget;
        protected Vector3[] mPositionsAroundTarget = new Vector3[4];
        private float mDistanceFromTarget;

        public float MoveSpeed { get; set; }
        public float DistanceFromTarget
        {
            get
            {
                return mDistanceFromTarget;
            }
            set { mDistanceFromTarget = value; }
        }

        public Camera AssociatedCamera { get; set; }
        public Transform Target
        {
            get { return mTarget; }
            set
            {
                mTarget = value;
                ResetPositionsAroundTarget();
            }
        }

        internal void ResetPositionsAroundTarget()
        {
            mPositionsAroundTarget[0] = mTarget.transform.position +
                                          (Vector3.forward.normalized * mDistanceFromTarget);
            mPositionsAroundTarget[1] = mTarget.transform.position -
                                        (Vector3.right.normalized * mDistanceFromTarget);
            mPositionsAroundTarget[2] = mTarget.transform.position -
                                        (Vector3.forward.normalized * mDistanceFromTarget);
            mPositionsAroundTarget[3] = mTarget.transform.position +
                                        (Vector3.right.normalized * mDistanceFromTarget);
        }
        /// <summary>
        /// sets a new position for the camera
        /// </summary>
        /// <param name="vNewPos"></param>
        public virtual void SetCameraPos(Vector3 vNewPos)
        {
            transform.position = vNewPos;
        }

        /// <summary>
        /// Forces a look at target
        /// </summary>
        public virtual void LookAtTarget()
        {
            if (Target != null)
            {
                transform.LookAt(Target);
            }
        }

        /// <summary>
        /// Moves the camera to the next position
        /// </summary>
        public virtual void MoveToNextPosition()
        {
            
        }

        /// <summary>
        /// Moves the camera to the previous position
        /// </summary>
        public virtual void MoveToPrevPosition()
        {
            
        }


    }
}
