// /**
// * @file SensorTransform.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */


using heddoko;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// copies the behaviour of an associated subsegment transform.
    /// </summary>
    public class SensorTransform : MonoBehaviour, IPointerClickHandler
    {

        public Transform OrientationToCopy;
        public bool UseBodyOrientation = false;
        private bool mIsHidden = true;
        public AxisViewContainer Axis;
        public GameObject[] ListenerLayerobjects;
        public Vector3[] Positions;
        private int mSpatialPos = 0;
        public int SensorPos;
        public UnityAction<int> SensorClicked;
        public GameObject[] HighlightedGameObjects;
        private bool mIsHighlighted;
        private bool mCalState = false;
        private bool mMagState = false;
        private ParticlePoolManager mRedPool;
        private ParticlePoolManager mWhitePool;
        public AirplaneSensorColor Wings;
        public AirplaneSensorColor Fuselage;
        [SerializeField]
        private SensorRotation mRotation;

        [SerializeField]
        private bool mUseParticles = false;

        /// <summary>
        /// The rotational component of the sensor transform. 
        /// </summary>
        private SensorRotation Rotation
        {
            get
            {
                if (mRotation == null)
                {
                    mRotation = gameObject.GetComponent<SensorRotation>();
                    if (mRotation == null)
                    {
                        mRotation = gameObject.AddComponent<SensorRotation>();
                    }
                }
                return mRotation;
            }
        }

        public bool IsHighlighted
        {
            get { return mIsHighlighted; }
        }


        /// <summary>
        /// Hide the sensor
        /// </summary>
        public void Hide()
        {
            mIsHidden = true;
            HighlightObject(false);
            gameObject.SetActive(false);
        }

        public void HighlightObject(bool vFlag)
        {
            for (int i = 0; i < HighlightedGameObjects.Length; i++)
            {
                HighlightedGameObjects[i].SetActive(vFlag);
            }
            mIsHighlighted = vFlag;
        }

        public void Show()
        {
            mIsHidden = false;
            HighlightObject(false);
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (!mIsHidden && UseBodyOrientation)
            {
                transform.localRotation = OrientationToCopy.localRotation;
            }
        }

        public void UpdateRotation(ImuDataFrame vFrame)
        {
            if (!UseBodyOrientation)
            {
                Quaternion vFrameRot = Quaternion.identity;
                vFrameRot.x = vFrame.quat_x_yaw;
                vFrameRot.y = vFrame.quat_y_pitch;
                vFrameRot.z = vFrame.quat_z_roll;
                vFrameRot.w = vFrame.quat_w;

                Vector3 vAccelVec = new Vector3(vFrame.Accel_x, vFrame.Accel_y, vFrame.Accel_z);
                vAccelVec = Vector3.Normalize(vAccelVec);

                Vector3 vGyroVec = new Vector3(vFrame.Rot_x, vFrame.Rot_y, vFrame.Rot_z);

                Rotation.CurId = SensorPos;
                Rotation.UpdateAccel(vAccelVec);
                Rotation.UpdateGyro(vGyroVec);
                Rotation.UpdateRotatation(vFrameRot);
            }
        }

        /// <summary>
        /// Changes the spatial position of the sensor
        /// </summary>
        /// <param name="vDirection"></param>
        public void ChangePosition(int vDirection)
        {

            mSpatialPos += vDirection;
            if (mSpatialPos < 0)
            {
                mSpatialPos = Positions.Length - 1;
            }

            if (mSpatialPos >= Positions.Length)
            {
                mSpatialPos = 0;
            }
            transform.localPosition = Positions[mSpatialPos];
        }

        /// <summary>
        /// Set the layer mask that the object is on.
        /// </summary>
        /// <param name="vCurrLayerMask"></param>

        public void SetLayer(LayerMask vCurrLayerMask)
        {
            gameObject.layer = vCurrLayerMask;
            Axis.SetLayer(vCurrLayerMask);
            if (ListenerLayerobjects != null)
            {
                for (int i = 0; i < ListenerLayerobjects.Length; i++)
                {
                    ListenerLayerobjects[i].layer = vCurrLayerMask;
                }
            }
        }


        public void OnPointerClick(PointerEventData vEventData)
        {
            if (SensorClicked != null)
            {
                SensorClicked(SensorPos);
            }
        }

        public void UpdateState(bool vCalStatus, bool vMagneticTransience)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            if (mUseParticles)
            {
                if (vCalStatus != mCalState)
                {
                    TriggerCalStateChange(vCalStatus);
                    mCalState = vCalStatus;
                }

                if (vMagneticTransience != mMagState)
                {
                    TriggerMagTransStateChange(vMagneticTransience);
                    mMagState = vMagneticTransience;
                }
            }
        }

        public void TriggerMagTransStateChange(bool vMagneticTransience)
        {
            mRedPool.RequestResource(transform.position);
            AirplaneSensorColor.SensorState vState = vMagneticTransience
               ? AirplaneSensorColor.SensorState.Good
               : AirplaneSensorColor.SensorState.Bad;
            Fuselage.SetState(vState);
        }

        public void TriggerCalStateChange(bool vCalStatus)
        {
            mWhitePool.RequestResource(transform.position);
            AirplaneSensorColor.SensorState vState = vCalStatus
                ? AirplaneSensorColor.SensorState.Good
                : AirplaneSensorColor.SensorState.Bad;
            Wings.SetState(vState);
        }

        public void SetPools(ParticlePoolManager vRedPool, ParticlePoolManager vWhitePool)
        {
            mWhitePool = vWhitePool;
            mRedPool = vRedPool;
        }
    }
}