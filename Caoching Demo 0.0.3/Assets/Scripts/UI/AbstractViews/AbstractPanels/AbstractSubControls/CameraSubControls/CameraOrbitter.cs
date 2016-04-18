/** 
* @file CameraOrbitter.cs
* @brief Contains the CameraOrbitter class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls
{
    /// <summary>
    /// Camera orbitter class that allows a camera to orbit around a target
    /// </summary>
    public class CameraOrbitter : AbstractSubControl, IEquatable<CameraOrbitter>
    {
        public Camera Camera;
        private Guid mId = Guid.NewGuid();
        public Transform Target;
        public LayerMask TargetsLayer;
        [SerializeField]
        private OrbitterState mCurrentState = OrbitterState.Idle;
        public float Distance = 10.0f;

        public float XSpeed = 250.0f;
        public float YSpeed = 120.0f;
        public float ZSpeed = 120.0f;

        public float YMinLimit = -20;
        public float YMaxLimit = 80;

        private double mX = 0.0;
        private double mY = 0.0;
        private double mZ = 0.0;
        private Ray mRay;
        private RaycastHit mRaycastHit;
        private static SubControlType sType = SubControlType.CameraOrbitSubControl;
        [SerializeField]
        private bool mIsEnabled;
        public bool IsEnabled { get { return mIsEnabled; } set { mIsEnabled = value; } }


        void Awake()
        {
            Camera = GetComponent<Camera>();
            if (Target != null)
            {
                string vLayer = LayerMask.LayerToName(Target.gameObject.layer);
                TargetsLayer = LayerMask.GetMask(vLayer);
            }

            CameraOrbitterCentralObserver.Add(this);

        }
        // Use this for initialization
        void Start()
        {
            var angles = transform.eulerAngles;
            mX = angles.y;
            mY = angles.x;
            //mZ = Distance;
            mZ = Vector3.Distance(transform.position, Target.position);

            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsEnabled)
            {
                CameraObitterFSM();
            }
        }

        /// <summary>
        /// A finite state machine for the orbitter
        /// </summary>
        void CameraObitterFSM()
        {
            mRay = Camera.ScreenPointToRay(Input.mousePosition);
            bool vMouseOver = Physics.Raycast(mRay, 11000, TargetsLayer); 
            switch (mCurrentState)
            {

                case OrbitterState.Idle:
                    if (vMouseOver)
                    {
                        mCurrentState = OrbitterState.MouseOverTarget;
                    }
                    break;
                case OrbitterState.MouseOverTarget:

                    if (!vMouseOver)
                    {
                        mCurrentState = OrbitterState.Idle;
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        mCurrentState = OrbitterState.MouseClicked;
                        CameraOrbitterCentralObserver.MouseClickedNotification(this);
                    }
                    break;
                case OrbitterState.MouseClicked:
                    {
                        if (Input.GetMouseButton(0))
                        {
                            MoveCamera();
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            mCurrentState = OrbitterState.MouseReleased;
                        }
                    }
                    break;

                case OrbitterState.MouseReleased:
                    {
                        mCurrentState = OrbitterState.Idle;
                        CameraOrbitterCentralObserver.MouseReleasedNotification(this);
                    }
                    break;

            }
        }
        /// <summary>
        /// Disables the CameraOrbitter and sets it state to idle
        /// </summary>
        public void DisableOrbitAction()
        {
            mCurrentState = OrbitterState.Idle;
            enabled = false;
        }
        /// <summary>
        /// Enables the CameraOrbitter.
        /// </summary>
        public void EnableOrbitAction()
        {
            enabled = false;
        }

        private void MoveCamera()
        {
            if (Target)
            {
                mX += Input.GetAxis("Mouse X") * XSpeed * 0.02;
                mY -= Input.GetAxis("Mouse Y") * YSpeed * 0.02;
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    mZ -= Input.GetAxis("Mouse ScrollWheel") * ZSpeed * 0.02;
                }
                else
                {
                    mZ += Input.GetAxis("Mouse ScrollWheel") * ZSpeed * 0.02;
                }

                mY = ClampAngle((float)mY, YMinLimit, YMaxLimit);

                var rotation = Quaternion.Euler((float)mY, (float)mX, 0);
                Vector3 vec = new Vector3(0.0f, 0.0f, (float)-mZ);
                var position = rotation * vec + Target.position;

                transform.rotation = rotation;
                transform.position = position;
            }
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// object equals override
        /// </summary>
        /// <param name="vObj"></param>
        /// <returns></returns>
        public override bool Equals(object vObj)
        {
            bool vResult = false;

            if (vObj != null && vObj is CameraOrbitter)
            {
                CameraOrbitter vOrbitter = (CameraOrbitter)vObj;
                vResult = mId.Equals(vOrbitter.mId);
            }
            return vResult;
        }

        public override int GetHashCode()
        {
            return mId.GetHashCode();
        }



        private enum OrbitterState
        {
            Idle,
            MouseOverTarget,
            MouseClicked,
            MouseReleased
        }
        public bool Equals(CameraOrbitter vOther)
        {
            bool vResult = false;
            if (vOther != null)
            {
                vResult = mId.Equals(vOther.mId);
            }
            return vResult;
        }

        public override SubControlType SubControlType
        {
            get { return sType; }
        }


        public override void Disable()
        {
            IsEnabled = false;
        }

        public override void Enable()
        {
            IsEnabled = true;
        }
    }
}
