using System;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls
{
    public class CameraZoom : CameraSubControl, IEquatable<CameraZoom>
    {

        private Action mZoomAction;
        public float MaxZoomIn = 0.2f;
        public float MaxZoomOut = 3f;
        public float ZoomSpeed = 1f;
        public float mTouchZoomSpeed = 0.005f;
        private bool mEnableActionCalled = false;

        private void AndroidZoomAction()
        {

            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                // If the camera is orthographic...


                float vNewSize = Camera.orthographicSize + deltaMagnitudeDiff * mTouchZoomSpeed;
                // ... change the orthographic size based on the change in distance between the touches.


                // Make sure the orthographic size never drops below zero.

                vNewSize = Mathf.Clamp(vNewSize, MaxZoomIn, MaxZoomOut);
                Camera.orthographicSize = vNewSize;


            }
        }

        private void DefaultZoomAction()
        {

        }

        private void ZoomStateMachine()
        {
            Ray = Camera.ScreenPointToRay(Input.mousePosition);
            bool vMouseOver = Physics.Raycast(Ray, 11000, TargetsLayer);
            if (vMouseOver)
            {
                float vMiddleMouseVal = Input.mouseScrollDelta.y / 10;
                float vZoom = vMiddleMouseVal * -ZoomSpeed;
                float vNewOrthoSize = Mathf.Clamp(Camera.orthographicSize + vZoom, MaxZoomIn, MaxZoomOut);
                Camera.orthographicSize = vNewOrthoSize;
                Camera.transform.LookAt(Target);
            }

        }


        public override SubControlType SubControlType
        {
            get { return SubControlType.CameraZoomSubControl; }
        }

        public override void Disable()
        {

        }

        public override void Enable()
        {

        }

        public bool Equals(CameraZoom vOther)
        {
            bool vResult = false;
            if (vOther != null)
            {
                vResult = Id.Equals(vOther.Id);
            }
            return vResult;
        }

        void Update()
        {

    //    ZoomStateMachine();
           AndroidZoomAction();
        }


    }
}