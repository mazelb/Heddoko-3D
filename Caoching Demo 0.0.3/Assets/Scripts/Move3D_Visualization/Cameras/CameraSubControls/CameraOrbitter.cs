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
    public class CameraOrbitter : CameraSubControl, IEquatable<CameraOrbitter>
    {
        
        public float Distance = 10.0f;

        public float XSpeed = 250.0f;
        public float YSpeed = 120.0f;
        public float ZSpeed = 120.0f;

        public float YMinLimit = -20;
        public float YMaxLimit = 80;

        private double mX = 0.0;
        private double mY = 0.0;
        private double mZ = 0.0;
       
        private static SubControlType sType = SubControlType.CameraOrbitSubControl;
       


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
            Ray = Camera.ScreenPointToRay(Input.mousePosition);
            bool vMouseOver = Physics.Raycast(Ray, 11000, TargetsLayer);
            switch (CurrentState)
            {

                case InputState.Idle:
                    if (vMouseOver)
                    {
                        CurrentState = InputState.HoveringOverTarget;
                    }
                    break;
                case InputState.HoveringOverTarget:

                    if (!vMouseOver)
                    {
                        CurrentState = InputState.Idle;
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        CurrentState = InputState.InputDown;
                        CameraOrbitterCentralObserver.MouseClickedNotification(this);
                    }
                    break;
                case InputState.InputDown:
                    {
                        if (Input.GetMouseButton(0))
                        {
                            MoveCamera();
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            CurrentState = InputState.InputUp;
                        }
                    }
                    break;

                case InputState.InputUp:
                    {
                        CurrentState = InputState.Idle;
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
            CurrentState = InputState.Idle;
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
                vResult = Id.Equals(vOrbitter.Id);
            }
            return vResult;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }



  
        public bool Equals(CameraOrbitter vOther)
        {
            bool vResult = false;
            if (vOther != null)
            {
                vResult = Id.Equals(vOther.Id);
            }
            return vResult;
        }

        public override SubControlType SubControlType
        {
            get { return sType; }
        }


    }
}
