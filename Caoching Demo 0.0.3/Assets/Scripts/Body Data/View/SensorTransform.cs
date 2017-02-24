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
using System.Text;

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
                Vector3 vGyroVec  = new Vector3(vFrame.Rot_x, vFrame.Rot_y, vFrame.Rot_z);     
                
                //champs magnetique provenant du capteur           
                //****
                Vector3 vMagVec   = new Vector3(vFrame.Mag_x, vFrame.Mag_y, vFrame.Mag_z);
                //**************


                Rotation.CurId = SensorPos;
                Rotation.UpdateAccel(vAccelVec);
                Rotation.UpdateGyro(vGyroVec);  
                
                //Mise a jour du vecteur champs magnetique avec les valeurs courantes                          
                //****
                Rotation.UpdateMag(vMagVec);                
                //**************

                Rotation.UpdateRotatation(vFrameRot);

                //Normalisation du champs magnetique 
                //****
                vMagVec               = Vector3.Normalize(vMagVec); 
                //traduction du champ magnetique local mesure par le capteur vers le systeme de reference exterieur global  
                Vector3 GMagVector    = Rotation.AbsoluteRotation * vMagVec;
                //traduction de lacceleration gravitationnelle locale mesure par le capteur vers le systeme de reference exterieur global  
                Vector3 GAccelVec     = Rotation.AbsoluteRotation * vAccelVec;
                //declaration du vecteur portant la composante du champ magnetique perpendiculaire au champs gravitationnelle (tous deux mesurer dans le systeme exterieur global)
                Vector3 CompMagPerp2G = new Vector3(0.0f,0.0f,0.0f);                
                if (Rotation.CurId == 0)
                {
                    SensorContainer.MeanMagField = GMagVector;              // champ magnetique correspondant a la moyenne des mesures effectuees par les 9 capteurs pour un frame donne
                    ConditionnalAccelerationAverage(GAccelVec, vGyroVec);   // appel de methode de moyennage conditionnel 
                                                                            // utilise pour obtenir le champ gravitationnel lorsque aucune autre source d'acceleration 
                                                                            // est presente ce qui implique pas de mouvement de rotation ni d'acceleration lineaire 
                }
                else if (Rotation.CurId == 8)                               // apres avoir parcourue les 9 capteurs on obtient la valeur moyenne du champ magnetique pour le frame
                {                                                           // actuel. Aussi on obtient la composante perpendiculaire du champ magnetique en le projettant
                    SensorContainer.MeanMagField *= Rotation.CurId;         // dans le plan defini par une normale de meme orientation que le champ gravitationnel moyen
                    SensorContainer.MeanMagField += GMagVector;
                    SensorContainer.MeanMagField /= (Rotation.CurId + 1.0f);
                    ConditionnalAccelerationAverage(GAccelVec, vGyroVec);
                    SensorContainer.NbSensAcc     = 1;
                    CompMagPerp2G = VProjectionPerp2W(SensorContainer.MeanMagField, SensorContainer.MeanGravField);   //appel de la methode de projection du champ magnetique
                }
                else 
                {
                    SensorContainer.MeanMagField *= Rotation.CurId;          // accumulation des valeurs de champs magnetiques et acceleration pour les capteurs 1 a 7
                    SensorContainer.MeanMagField += GMagVector;
                    SensorContainer.MeanMagField /= (Rotation.CurId + 1.0f);
                    ConditionnalAccelerationAverage(GAccelVec, vGyroVec);
                }
                //StringBuilder vBuilder = new StringBuilder();
                //float dp = Vector3.Dot(CompMagPerp2G, SensorContainer.MeanGravField);
                //vBuilder.Append("ns/" + Rotation.CurId);
                //vBuilder.AppendFormat("                  /ms/ {0:0.00} / {1:0.00} / {2:0.00}", vMagVec.x,    vMagVec.y,    vMagVec.z);
                //vBuilder.AppendFormat("                  /mg/ {0:0.00} / {1:0.00} / {2:0.00}", GMagVector.x, GMagVector.y, GMagVector.z);
                //vBuilder.AppendFormat("                 /mgp/ {0:0.00} / {1:0.00} / {2:0.00}", CompMagPerp2G.x,
                //                                                                               CompMagPerp2G.y,
                //                                                                               CompMagPerp2G.z);
                //vBuilder.AppendLine();
                //vBuilder.AppendFormat("                 /sg/ {0:0.00} / {1:0.00} / {2:0.00}", vAccelVec.x, vAccelVec.y, vAccelVec.z);
                //vBuilder.AppendFormat("                 /gg/ {0:0.00} / {1:0.00} / {2:0.00}", GAccelVec.x, GAccelVec.y, GAccelVec.z);
                //vBuilder.AppendFormat("                 /mg/ {0:0.00} / {1:0.00} / {2:0.00} /dotpro/ {3:0.00} ", SensorContainer.MeanGravField.x,
                //                                                                                                 SensorContainer.MeanGravField.y,
                //                                                                                                 SensorContainer.MeanGravField.z,dp);
                //vBuilder.AppendLine();
                //ConsoleTextView.UpdateStaticConsole(vBuilder.ToString());               
                //Debug.Log(vBuilder.ToString());                
                //******************
            }
        }
        /// <summary>
        /// methode de moyennage conditionnelle pour l'acceleration
        /// </summary>
        /// <param name="CurAcc"></param>
        /// <param name="CurGyro"></param>
        public void ConditionnalAccelerationAverage(Vector3 CurAcc, Vector3 CurGyro)
        {            
            float GyroNorm        = CurGyro.magnitude/32767.0f;
            float AcceNorm        = CurAcc.magnitude;
            float DiffAccG        = Mathf.Abs(Mathf.Abs(AcceNorm)/2047.0f - 1.0f);
            Vector3 vUnitAccelVec = Vector3.Normalize(CurAcc);
            if (GyroNorm < 20.0f && DiffAccG < 0.02f)
            {
                SensorContainer.MeanGravField *= SensorContainer.NbSensAcc;
                SensorContainer.MeanGravField += vUnitAccelVec            ;
                SensorContainer.NbSensAcc++                               ;
                SensorContainer.MeanGravField /= (float) SensorContainer.NbSensAcc;
            }     
        }
        /// <summary>
        /// methode de projection permettant d'extraire les composantes d'un vecteur (V) contenues dans le plan definit par un autre vecteur (W) la normale de ce plan.  
        /// </summary>
        /// <param name="V"></param>
        /// <param name="W"></param>
        /// <returns></returns>
        public Vector3 VProjectionPerp2W(Vector3 V, Vector3 W)
        {
            Vector3 nv = V.normalized;
            Vector3 nw = W.normalized;
            float   nvdotnw = Vector3.Dot(nv, nw);
            Vector3 PjpVonW = nv - nvdotnw * nw  ;
            return  PjpVonW;          
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