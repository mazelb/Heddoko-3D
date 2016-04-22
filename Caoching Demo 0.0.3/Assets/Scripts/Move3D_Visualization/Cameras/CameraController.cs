
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using Assets.Scripts.UI.Metrics;
using Assets.Scripts.UI._2DSkeleton;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// This class controls a camera's behaviour in scene
    /// </summary>
    [RequireComponent(typeof(MoveCameraToPositon))]
    public class CameraController : MonoBehaviour
    {

        public LayerMask ModelMask;
        private RaycastHit mHitInfo;
        public Camera CurrentCam;
        //  private CameraOrbit mCameraOrbit;
        private bool mCommenceMove;
        public float ReturnSpeed = 10f;
        public bool ReturnToPositionEnabled = true;
        [SerializeField]
        private MoveCameraToPositon mAutoCamMover; 

        private Vector3 mOriginalPos;
        private Quaternion mOriginalRotation;
        [SerializeField]
        private bool mIn2DMode = false;
        public Button LeftButton;
        public Button RightButton;
        public LegMoveSwitcher LegSwitcher;
        public CurrentViewBox[] CurrentViewBox;

        private Vector3 mPositionOffset;
        [SerializeField]
        private float mFoV;
        public Vector3 Offset
        {
            set
            {
                mAutoCamMover.Offset = value;

            }
        }
        void Awake()
        {
            mAutoCamMover = GetComponent<MoveCameraToPositon>();
            mAutoCamMover.Cam = CurrentCam;
            mOriginalPos = transform.position;
            mOriginalRotation = transform.rotation;
            if (LeftButton != null && RightButton != null)
            {
                LeftButton.onClick.AddListener(SwitchToLeftSide);
                RightButton.onClick.AddListener(SwitchToRightSide);
            }
        }

        void Update()
        {

            if (mIn2DMode)
            {
                mAutoCamMover.enabled = false;
            }

            if (CurrentViewBox != null)
            {
                if (mIn2DMode)
                {
                    for (int i = 0; i< CurrentViewBox.Length; i++)
                    {
                        CurrentViewBox[i].UpdateText(true, LegSwitcher.CurrentSpriteIndex);
                    }
                   // CurrentViewBox.UpdateText(true, LegSwitcher.CurrentSpriteIndex);
                }
                else
                {
                    for (int i = 0; i < CurrentViewBox.Length; i++)
                    {
                        CurrentViewBox[i].UpdateText(false, mAutoCamMover.CurrentPosition);
                        
                    }
             
                }

            }
            InputHandler();
            //CurrentCam.orthographicSize = mFoV;
        }

        /// <summary>
        /// handles input and changes the camera angle accordingly
        /// </summary>
        private void InputHandler()
        {
            if (!mIn2DMode)
            {
                //move camera
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    mAutoCamMover.MovetoNextPos();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    mAutoCamMover.MoveToPos(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    mAutoCamMover.MoveToPos(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    mAutoCamMover.MoveToPos(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    mAutoCamMover.MoveToPos(3);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    mAutoCamMover.MoveToPos(4);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    mAutoCamMover.MovetoNextPos();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    mAutoCamMover.MoveToPrevPos();
                }
            }

        }

        public float offset;
        /// <summary>
        /// sets the cameras view port and the position offset of the look at target
        /// </summary>
        /// <param name="mNewFov">new view port size</param>
        /// <param name="vOffSet">offset </param>
        public void SetCamFov(float mNewFov, Vector3 vOffSet)
        {
            CurrentCam.orthographicSize = mNewFov;
            offset = vOffSet.y;
            mFoV = mNewFov;
            Offset = vOffSet;
        }

        /// <summary>
        /// Disable camera orbit and move to camera scripts
        /// </summary>
        public void PrepFor2DView()
        {

            mAutoCamMover.enabled = false;
            mIn2DMode = true;
            Reset();
        }

        /// <summary>
        /// Switches to the left view of the body, depending on whether in 2d mode or 3d
        /// </summary>
        private void SwitchToLeftSide()
        {
            if (mIn2DMode)
            {
                if (LegSwitcher != null)
                {
                    LegSwitcher.TurnOnSprite(0);
                }
            }
            else
            {
                mAutoCamMover.MoveToPrevPos();
                mAutoCamMover.enabled = true;
            }

        }

        /// <summary>
        /// Switches to the right  view of the body, depending on whether in 2d mode or 3d
        /// </summary>
        private void SwitchToRightSide()
        {
            if (mIn2DMode)
            {
                if (LegSwitcher != null)
                {
                    LegSwitcher.TurnOnSprite(1);

                }
            }
            else
            {
                mAutoCamMover.MovetoNextPos();
                mAutoCamMover.enabled = true;
            }
        }

        /// <summary>
        /// Enables camera orbit and move to camera scripts
        /// </summary>
        public void PrepFor3DView()
        {
            mAutoCamMover.enabled = true;
            mIn2DMode = false;
        }
        /// <summary>
        /// Resets the tranformation
        /// </summary>
        public void Reset()
        {
            transform.position = mOriginalPos;
            transform.rotation = mOriginalRotation;
        }


    }
}
