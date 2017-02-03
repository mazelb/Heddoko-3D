// /**
// * @file SensorRotation.cs
// * @brief Contains the SensorRotation class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Security.Policy;
using Assets.Scripts.Utils.DebugContext;
using heddoko;
using HeddokoLib.body_pipeline;
using UnityEngine;
using Assets.Scripts.Utils;
#if !SEGMENTS_DEBUG
namespace Assets.Scripts.Body_Data.View
{

    public class SensorRotation : MonoBehaviour
    {
        public Vector3 AxisOfRotation = new Vector3(0, 180, 0);        
        public Quaternion AbsoluteRotation = Quaternion.identity;
        //public Quaternion RelativeRotation = Quaternion.identity;
        public Quaternion InitialRotation  = Quaternion.identity;
        public Quaternion UpAxisRotation;
        public Quaternion GravityOffset    = Quaternion.identity;

        public Vector3 UpVector      = Vector3.up;              
        public Vector3 RightVector   = Vector3.right;      
        public Vector3 ForwardVector = Vector3.forward;      

        public Vector3 LegUpVector = new Vector3(0, 0, 1);           
        public Vector3 LegRightVector = new Vector3(1, 0, 0);        
        public Vector3 LegForwardVector = new Vector3(0, 1, 0);      

        public Vector3 RightArmUpVector = new Vector3(0, 0, 1);      
        public Vector3 RightArmRightVector = new Vector3(0, 1, 0);   
        public Vector3 RightArmForwardVector = new Vector3(1, 0, 0); 

        public Vector3 LeftArmUpVector = new Vector3(0, 0, 1);       
        public Vector3 LeftArmRightVector = new Vector3(0, -1, 0);   
        public Vector3 LeftArmForwardVector = new Vector3(-1, 0, 0); 

        public Vector3 CurAccelVector        = Vector3.zero;            
        public Vector3 CurGyroVector         = Vector3.zero;

        private bool mCalState               = false;
        private bool mMagState               = false;
        public int CurId = -1;

        [SerializeField]
        public Vector3 OrientationCorrection;               
        public bool UseCorrection = false;
        public bool IsReset       = false;

        void Awake()
        {
            IsReset = false;
            SetAxisOfRotation();   
        }

        void SetAxisOfRotation()
        {
            UpAxisRotation = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }

        public void UpdateAccel(Vector3 vNewAccel)
        {
            CurAccelVector = Vector3.Lerp(CurAccelVector, vNewAccel, 0.15f);  
        }

        public void UpdateGyro(Vector3 vNewGyro)
        {
            CurGyroVector = vNewGyro;
        }

        public void UpdateRotatation(Quaternion vNewRot)               
        {
            AbsoluteRotation.x = -vNewRot.y;
            AbsoluteRotation.y =  vNewRot.z;
            AbsoluteRotation.z = -vNewRot.x;
            AbsoluteRotation.w =  vNewRot.w;
            if (!IsReset)
            {
                Reset();
            }
        }

        /// <summary>
        /// Computes an offset from a given input quaternion. 
        /// </summary>
        /// <param name="vInputQuaternion">the input quaternion to extract an offset from</param>
        /// <returns>The offset rotation</returns>
        private static Quaternion ComputeTorsoOffsetFromGlobalDown(Quaternion vInputQuaternion)          
        {                                                                                               
            //get the global down vector3 with respect to the input quaternion
            Vector3 vRelativeDown = vInputQuaternion * Vector3.down;                                    
            float vAngle = Vector3.Angle(Vector3.down, vRelativeDown);                                   
            //get the cross between the two, giving us an orthonormal axis from which we can create a quaternion from
            Vector3 vOrthoNormalCross = Vector3.Cross(Vector3.down, vRelativeDown).normalized;           
            //create a quaternion with the cross product as the axis
            var vOffset = Quaternion.AngleAxis(vAngle, vOrthoNormalCross);
            return vOffset;
        }
        void Q2HPR(Quaternion vQ, ref Vector3 ypr)
        {
            ypr[0] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[0] * vQ[1] + 2 * vQ[3] * vQ[2]), (2 * vQ[3] * vQ[3] + 2 * vQ[0] * vQ[0] - 1));
            ypr[1] = Mathf.Rad2Deg * Mathf.Asin(-(2 * vQ[0] * vQ[2] - 2 * vQ[3] * vQ[1]));
            ypr[2] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[1] * vQ[2] + 2 * vQ[3] * vQ[0]), (2 * vQ[3] * vQ[3] + 2 * vQ[2] * vQ[2] - 1));
            if (ypr[0] < 0)
                ypr[0] += 360;
        }

        public void Reset()
        {
            IsReset = true;
            Quaternion vExpectedRotation = Quaternion.identity;
            Vector3 vExpectedGravity     = Vector3.down;
            Vector3 vCurGravity = vExpectedGravity;
            if (CurId == 5 || CurId == 7 || CurId == 6 || CurId == 8)
            {
                vExpectedRotation = Quaternion.LookRotation(LegForwardVector, LegUpVector);
            }
            else if (CurId == 1 || CurId == 2)
            {
                vExpectedRotation = Quaternion.LookRotation(RightArmForwardVector, RightArmUpVector);
            }
            else if (CurId == 3 || CurId == 4)
            {
                vExpectedRotation = Quaternion.LookRotation(LeftArmForwardVector, LeftArmUpVector);
            }
            else if (CurId == 0)
            {
                vExpectedRotation = Quaternion.LookRotation(-LegForwardVector, LegUpVector);
            }
            vExpectedGravity = vExpectedRotation * Vector3.down;
            vCurGravity      = Quaternion.Inverse(vExpectedRotation) * CurAccelVector;
            GravityOffset    = Quaternion.FromToRotation(vExpectedGravity, vCurGravity);
            InitialRotation  = AbsoluteRotation * GravityOffset;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
                Reset();

            if (IsReset)
            {
                Quaternion vCurRotation = Quaternion.identity;
                Quaternion vExpectedRotation = Quaternion.identity;
                Quaternion vGravityOffset = Quaternion.identity;

                if (CurId == 5 || CurId == 7 || CurId == 6 || CurId == 8)
                {
                    vExpectedRotation = Quaternion.LookRotation(LegForwardVector, LegUpVector);
                }
                else if (CurId == 1 || CurId == 2)
                {
                    vExpectedRotation = Quaternion.LookRotation(RightArmForwardVector, RightArmUpVector);
                }
                else if (CurId == 3 || CurId == 4)
                {
                    vExpectedRotation = Quaternion.LookRotation(LeftArmForwardVector, LeftArmUpVector);
                }
                else if (CurId == 0)
                {
                    vExpectedRotation = Quaternion.LookRotation(-LegForwardVector, LegUpVector);
                }
                vCurRotation = Quaternion.Inverse(vExpectedRotation) * Quaternion.Inverse(InitialRotation) * (AbsoluteRotation * GravityOffset);
                Quaternion vNewRotation = Quaternion.Slerp(transform.rotation, vCurRotation, 0.3f);
                transform.rotation = vNewRotation;
            }
        }
    }
}
#elif SEGMENTS_DEBUG
namespace Assets.Scripts.Body_Data.View
{
    public class SensorRotation : MonoBehaviour
    {         
        private bool mCalState     = false;
        private bool mMagState     = false;
        public bool  UseCorrection = false;
        public bool  IsReset       = false;

        public Vector3    AxisOfRotation      = new Vector3(0, 180, 0);
        public Quaternion UpAxisRotation;
        public Quaternion AbsoluteRotation    = Quaternion.identity; // quaternion des capteurs
        public Quaternion AbsoluteRotationInU = Quaternion.identity; // quaternion traduit dans le systeme de Unity
        public Quaternion InitialRotation     = Quaternion.identity; // quaternion associe aux valeurs initiales renvoyees par les capteurs 
                                                                     // et corrigees pour tenir compte du decalage verticale (gravity offset) 
        public Quaternion InitialRotationInU  = Quaternion.identity; // traduction de InitialRotation dans le systeme de Unity       
        public Quaternion GravityOffsetInU    = Quaternion.identity; // quaternion de correction par rapport a la verticale          
        public Vector3 CurAccelVector         = Vector3.zero;        // valeur actuelle de l'acceleration lineaire provenant des accelerometres
        public Vector3 CurGyroVector          = Vector3.zero;        // valeur actuelle de la vitesse angulaire porvenant des gyroscopes
        public int CurId                      = -1;
        [SerializeField]        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Reset();
            }
            if (IsReset)
            {
                Quaternion vCurRotationInU      = Quaternion.identity;    
                Quaternion vExpectedRotation    = Quaternion.identity;
                Quaternion vGravityOffset       = Quaternion.identity;
                //Correction des orientations actuelles en tenant compte des valeurs initiales des quaternions et de la correction verticale 
                vCurRotationInU                 = Quaternion.Inverse(InitialRotationInU) * (AbsoluteRotationInU * GravityOffsetInU);
                //Filtrage passe-bas
                Quaternion vNewRotationInU      = Quaternion.Slerp(transform.rotation, vCurRotationInU, 0.3f);
                //transformation dans le systeme de Unity
                transform.rotation              = vNewRotationInU;
            }
        }
        public void UpdateRotatation(Quaternion vNewRot)                
        {     
            //quaternion provenant du capteur (sans aucune modification)       
            AbsoluteRotation.x = vNewRot.x;
            AbsoluteRotation.y = vNewRot.y;
            AbsoluteRotation.z = vNewRot.z;
            AbsoluteRotation.w = vNewRot.w;
            //quaternion provenant du capteur et traduit dans le systeme de reference de Unity (InU: in Unity)
            AbsoluteRotationInU = SensorSystemMapToUnitySystem(AbsoluteRotation, CurId);           
            if (!IsReset)
            {
                Reset();
            }
        }
        public void Reset()
        {
            IsReset                         = true;
            Vector3    CurAccelVectorNormed = CurAccelVector.normalized; //Normalisation de l'acceleration lineaire actuelle
            Vector3     vExpectedGravityInU = Vector3.down;              //Orientation attendue de la gravite pour l'avatar (0,-1,0)           
            //traduction du vecteur acceleration mesure par les capteurs dans Unity  
            Vector3 SensorAcceleroVectorInU = SensorSystemMapToUnitySystem(CurAccelVectorNormed, CurId);
            //Calcule (dans Unity) du decalage entre la mesure de l'acceleration actuelle InU (supposee  = a la gravite) et celle Attendue   
            GravityOffsetInU = Quaternion.FromToRotation(vExpectedGravityInU, SensorAcceleroVectorInU);
            //Calcule de la correction dependant de l'orientation initiale et le GravityOffset
            InitialRotationInU              = AbsoluteRotationInU * GravityOffsetInU;
        }
        /// <summary>
        /// Fonction qui reorganise les composantes d'un vecteur mesure dans le repere d'un capteur
        /// afin de l'exprimer dans Unity selon le systeme d'axes local associe au segment du corps de l'avatar
        /// Cette reorganisation dependant de la position du capteur sur le corps
        /// </summary>
        /// <param name="SensorVector"></param>
        /// <param name="CurId"></param>
        /// <returns></returns>
        public Vector3 SensorSystemMapToUnitySystem(Vector3 SensorVector,int CurId)
        {
            Vector3 TranslatedVectorS2U = new Vector3(0,0,0); 
            if (CurId == 5 || CurId == 7 || CurId == 6 || CurId == 8) //jambes
            {
                TranslatedVectorS2U[0] = SensorVector[1];
                TranslatedVectorS2U[1] = SensorVector[0];
                TranslatedVectorS2U[2] = SensorVector[2];
            }
            else if (CurId == 1 || CurId == 2)                        //bras droit
            {
                TranslatedVectorS2U[0] = -SensorVector[0];
                TranslatedVectorS2U[1] = -SensorVector[1];
                TranslatedVectorS2U[2] = -SensorVector[2];
            }
            else if (CurId == 3 || CurId == 4)                        //bras gauche
            {
                TranslatedVectorS2U[0] =  SensorVector[0];
                TranslatedVectorS2U[1] =  SensorVector[1];
                TranslatedVectorS2U[2] = -SensorVector[2];
            }
            else if (CurId == 0)                                      //torse
            {
                TranslatedVectorS2U[0] = -SensorVector[1];
                TranslatedVectorS2U[1] = -SensorVector[0];
                TranslatedVectorS2U[2] =  SensorVector[2];                
            }
            return TranslatedVectorS2U;
        }
        /// <summary>
        /// Surcharge de la fonction SensorSystemMapToUnitySystem avec un quaternion pour 
        /// argument. Reexprime le quaternion dans le systeme gauche de Unity et permettant de 
        /// conserver les orientations de l'axe de rotation mais en tenant compte du nouveau des axes locaux du segment de l'avatar
        /// </summary>
        /// <param name="SensorQuat"></param>
        /// <param name="CurId"></param>
        /// <returns></returns>
        public Quaternion SensorSystemMapToUnitySystem(Quaternion SensorQuat, int CurId)
        {
            //reecriture de l'axe de rotation du quaternion (partie vectorielle
            Vector3 RotAxvector             = new Vector3(SensorQuat.x, SensorQuat.y, SensorQuat.z);
            Vector3 RotAxvectorS2U          = SensorSystemMapToUnitySystem(RotAxvector, CurId);  
            Quaternion TranslatedQuatS2U;
            //changement de signe de l'angle a cause du systeme gauche
            TranslatedQuatS2U.x = -RotAxvectorS2U[0];
            TranslatedQuatS2U.y = -RotAxvectorS2U[1];
            TranslatedQuatS2U.z = -RotAxvectorS2U[2];
            TranslatedQuatS2U.w = SensorQuat.w;
            return TranslatedQuatS2U;
        }
        private static Quaternion ComputeTorsoOffsetFromGlobalDown(Quaternion vInputQuaternion)
        {
            Vector3 vRelativeDown = vInputQuaternion * Vector3.down;
            float vAngle = Vector3.Angle(Vector3.down, vRelativeDown);
            Vector3 vOrthoNormalCross = Vector3.Cross(Vector3.down, vRelativeDown).normalized;
            var vOffset = Quaternion.AngleAxis(vAngle, vOrthoNormalCross);
            return vOffset;
        }
        void SetAxisOfRotation()
        {
            UpAxisRotation  = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }
        public void UpdateAccel(Vector3 vNewAccel)
        {
            CurAccelVector = Vector3.Lerp(CurAccelVector, vNewAccel, 0.15f);
        }
        public void UpdateGyro(Vector3 vNewGyro)
        {
            CurGyroVector = vNewGyro;
        }
        void Q2HPR(Quaternion vQ, ref Vector3 ypr)
        {
            ypr[0] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[0] * vQ[1] + 2 * vQ[3] * vQ[2]), (2 * vQ[3] * vQ[3] + 2 * vQ[0] * vQ[0] - 1));
            ypr[1] = Mathf.Rad2Deg * Mathf.Asin(-(2 * vQ[0] * vQ[2] - 2 * vQ[3] * vQ[1]));
            ypr[2] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[1] * vQ[2] + 2 * vQ[3] * vQ[0]), (2 * vQ[3] * vQ[3] + 2 * vQ[2] * vQ[2] - 1));
            if (ypr[0] < 0)
                ypr[0] += 360;
        }
        void Awake()
        {
            IsReset = false;
            SetAxisOfRotation();
        }
    }
}
#endif