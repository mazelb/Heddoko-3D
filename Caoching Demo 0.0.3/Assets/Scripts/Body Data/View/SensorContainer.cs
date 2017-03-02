// /**
// * @file SensorContainer.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using heddoko;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System.Text;
using Assets.Scripts.Utils.DebugContext;
using Assets.Scripts.Utils;





namespace Assets.Scripts.Body_Data.View
{
    public class SensorContainer : MonoBehaviour
    {
        [SerializeField]
        private List<SensorTransform> mSensorTransformList = new List<SensorTransform>();

        private ParticlePoolManager mRedPool;
        private ParticlePoolManager mWhitePool;
        public ParticleSystemDisabler WhitePref;
        public ParticleSystemDisabler RedPref;
        public Action<int> SensorSelected;
        public Action<int> SensorDeselected;

        /// <summary>
        /// declaration des vecteurs de champs magnetique et d'acceleration moyen visible de tous de l'interieur de SensorTransform 
        /// </summary>

        public static Vector3 MeanMagInG          = new Vector3(0, 0, 0);///moyenne sur 9 capteurs du champ magnetique dans le referentiel exterieur (Reference) 
        public static Vector3 MeanGravFieldInG    = new Vector3(0, 0, 0);///moyenne sur 9 capteurs du champ gravitationnelle dans le referentiel exterieur (Reference) 
        public static Vector3 MeanMagH            = new Vector3(0, 0, 0);///composante perpendiculaire moyenne sur 9 capteurs du champ magnetique dans le referentiel exterieur (Reference) 
        public static Vector3 MeanFowardInG       = new Vector3(0, 0, 0);///moyenne sur 9 capteurs de la direction avant (dans le ref exterieur)
        public static int NbSensAcc               = 1;   // pour un frame donne nombre de capteurs ayant contribue a la moyenne 
       

        void Awake()
        {
            mRedPool = new ParticlePoolManager(18, RedPref);
            mWhitePool = new ParticlePoolManager(18, WhitePref);
            List<SensorTransform> vSortedList = mSensorTransformList.OrderBy(o => o.SensorPos).ToList();
            mSensorTransformList = vSortedList;
        }

        void OnEnable()
        {

            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SensorClicked += SetHighlightToObject;
                mSensorTransformList[i].SetPools(mRedPool, mWhitePool);
            }
        }

        public void ChangeState(int vSensorIndex, bool vCalStatus, bool vMagneticTransience)
        {
            mSensorTransformList[vSensorIndex].UpdateState(vCalStatus, vMagneticTransience);
        }

        private void SetHighlightToObject(int vArg0)
        {
            //If it's highlighted, show all objects
            if (mSensorTransformList[vArg0].IsHighlighted)
            {
                //remove the highlight from this object
                mSensorTransformList[vArg0].HighlightObject(false);
                for (int i = 0; i < mSensorTransformList.Count; i++)
                {
                    mSensorTransformList[i].Show();
                }
                if (SensorDeselected != null)
                {
                    SensorDeselected(vArg0);
                }
            }
            else
            {
                for (int i = 0; i < mSensorTransformList.Count; i++)
                {
                    mSensorTransformList[i].Hide();
                }
                mSensorTransformList[vArg0].Show();
                mSensorTransformList[vArg0].HighlightObject(true);
                if (SensorSelected != null)
                {
                    SensorSelected(vArg0);
                }
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SensorClicked -= SetHighlightToObject;
            }
        }

        public void Hide()
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].Hide();
            }
        }

        public void Show()
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].Show();
            }
        }

        /// <summary>
        /// Set the layer of the gameobject
        /// </summary>
        /// <param name="vCurrLayerMask"></param>
        public void SetLayer(LayerMask vCurrLayerMask)
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].SetLayer(vCurrLayerMask);
            }
            mRedPool.SetLayer(vCurrLayerMask);
            mWhitePool.SetLayer(vCurrLayerMask);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SetSensorsSpatialPosition(-1);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                SetSensorsSpatialPosition(1);
            }
        }

        /// <summary>
        /// Sets the spatial position of the sensors according to a passed in direction
        /// </summary>
        /// <param name="vDirection"></param>
        public void SetSensorsSpatialPosition(int vDirection)
        {
            for (int i = 0; i < mSensorTransformList.Count; i++)
            {
                mSensorTransformList[i].ChangePosition(vDirection);
            }
        }

        public void UpdateSensorOrientation(Packet vPacket)
        {
            var vImuFrames = vPacket.fullDataFrame.imuDataFrame;
            VecFrameAverage(vImuFrames);    ///calcul des champs moyenne de reference 
            for (int vI = 0; vI < vImuFrames.Count; vI++)
            {
                int vIdx = (int)vImuFrames[vI].imuId;
                mSensorTransformList[vIdx].UpdateRotation(vImuFrames[vI]);
            }
            //uint calStable = (dataFrame.sensorMask >> 19) & 0x01;
            //uint magTransient = (dataFrame.sensorMask >> 20) & 0x01;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurAcc"></param>
        /// <param name="CurGyro"></param>
        public void ConditionnalAccelerationAverage(Vector3 CurAcc, Vector3 CurGyro)
        {
            float GyroNorm = CurGyro.magnitude / 32767.0f;
            float AcceNorm = CurAcc.magnitude;
            float DiffAccG = Mathf.Abs(Mathf.Abs(AcceNorm) / 2047.0f - 1.0f);
            Vector3 vUnitAccelVec = Vector3.Normalize(CurAcc);
            if (GyroNorm < 20.0f && DiffAccG < 0.02f)
            {
                SensorContainer.MeanGravFieldInG *= SensorContainer.NbSensAcc;
                SensorContainer.MeanGravFieldInG += vUnitAccelVec;
                SensorContainer.NbSensAcc++;
                SensorContainer.MeanGravFieldInG /= (float)SensorContainer.NbSensAcc;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="V"></param>
        /// <param name="W"></param>
        /// <returns></returns>
        public static Vector3 VProjectionPerp2W(Vector3 V, Vector3 W)
        {
            Vector3 nv = V.normalized;
            Vector3 nw = W.normalized;
            float nvdotnw = Vector3.Dot(nv, nw);
            Vector3 PjpVonW = nv - nvdotnw * nw;
            return PjpVonW;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vImuFrames"></param>
        public void VecFrameAverage(List<ImuDataFrame> vImuFrames)
        {
            for (int vI = 0; vI < vImuFrames.Count; vI++)
            {
                ImuDataFrame vFrame  = vImuFrames[vI];
                Quaternion vFrameRot = Quaternion.identity;
                vFrameRot.x = vFrame.quat_x_yaw;
                vFrameRot.y = vFrame.quat_y_pitch;
                vFrameRot.z = vFrame.quat_z_roll;
                vFrameRot.w = vFrame.quat_w;
                Vector3 vAccelVec        = new Vector3(vFrame.Accel_x, vFrame.Accel_y, vFrame.Accel_z);
                Vector3 vGyroVec         = new Vector3(vFrame.Rot_x, vFrame.Rot_y, vFrame.Rot_z);
                Vector3 vMagVec          = new Vector3(vFrame.Mag_x, vFrame.Mag_y, vFrame.Mag_z);
                vMagVec                  = Vector3.Normalize(vMagVec);
                Vector3 MagVecInG        = vFrameRot * vMagVec;
                Vector3 AccelVecInG      = vFrameRot * vAccelVec;
                Vector3 PersonFowardInS  ;
                if (vI > 0 && vI <5)
                {
                    PersonFowardInS       = new Vector3(0.0f,0.0f,-1.0f);
                    MeanFowardInG  += vFrameRot * PersonFowardInS;
                }
                else  
                {
                    PersonFowardInS       = new Vector3(0.0f,0.0f, 1.0f);
                    MeanFowardInG  += vFrameRot * PersonFowardInS;
                }

                if (vI == 0)
                {
                    MeanMagInG = MagVecInG;
                    ConditionnalAccelerationAverage(AccelVecInG, vGyroVec);
                }
                else if (vI == 8)
                {
                    MeanMagInG    += MagVecInG  ;
                    MeanMagInG    /= (vI + 1.0f);
                    MeanFowardInG /= (vI + 1.0f);
                    ConditionnalAccelerationAverage(AccelVecInG, vGyroVec);
                    NbSensAcc            = 1;
                    MeanMagH       = VProjectionPerp2W(MeanMagInG, MeanGravFieldInG);
                }
                else
                {
                    MeanMagInG += MagVecInG;
                    ConditionnalAccelerationAverage(AccelVecInG, vGyroVec);
                }
            }
        }
    }
}