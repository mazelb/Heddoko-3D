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

namespace Assets.Scripts.Body_Data.View
{
    public class SensorRotation : MonoBehaviour
    {
        private bool mCalState = false;
        private bool mMagState = false;
        public bool UseCorrection = false;
        public bool IsReset = false;

        public Vector3 AxisOfRotation = new Vector3(0, 180, 0);
        public Quaternion UpAxisRotation;
        public Quaternion AbsoluteRotation = Quaternion.identity; // quaternion des capteurs
        public Quaternion AbsoluteRotationInU = Quaternion.identity; // quaternion traduit dans le systeme de Unity
        public Quaternion InitialRotation = Quaternion.identity; // quaternion associe aux valeurs initiales renvoyees par les capteurs 
                                                                 // et corrigees pour tenir compte du decalage verticale (gravity offset) 
        public Quaternion InitialRotationInU = Quaternion.identity; // traduction de InitialRotation dans le systeme de Unity       
        public Quaternion GravityOffsetInU = Quaternion.identity; // quaternion de correction par rapport a la verticale          
        public Vector3 CurAccelVector = Vector3.zero;        // valeur actuelle de l'acceleration lineaire provenant des accelerometres
        public Vector3 CurGyroVector = Vector3.zero;        // valeur actuelle de la vitesse angulaire porvenant des gyroscopes
        public int CurId = -1;

        [SerializeField]
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Reset();
            }
            if (IsReset)
            {
                Quaternion vCurRotationInU = Quaternion.identity;
                Quaternion vExpectedRotation = Quaternion.identity;
                Quaternion vGravityOffset = Quaternion.identity;
                //Correction des orientations actuelles en tenant compte des valeurs initiales des quaternions et de la correction verticale 
                vCurRotationInU = Quaternion.Inverse(InitialRotationInU) * (AbsoluteRotationInU * GravityOffsetInU);
                //Filtrage passe-bas
                Quaternion vNewRotationInU = Quaternion.Slerp(transform.rotation, vCurRotationInU, 0.3f);
                //transformation dans le systeme de Unity
                transform.rotation = vNewRotationInU;
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
            IsReset = true;

            Vector3 CurAccelVectorNormed = CurAccelVector.normalized; //Normalisation de l'acceleration lineaire actuelle
            Vector3 vExpectedGravityInU = Vector3.down;              //Orientation attendue de la gravite pour l'avatar (0,-1,0)           
            //traduction du vecteur acceleration mesure par les capteurs dans Unity  
            Vector3 SensorAcceleroVectorInU = SensorSystemMapToUnitySystem(CurAccelVectorNormed, CurId);
            //Calcule (dans Unity) du decalage entre la mesure de l'acceleration actuelle InU (supposee  = a la gravite) et celle Attendue   
            GravityOffsetInU = Quaternion.FromToRotation(vExpectedGravityInU, SensorAcceleroVectorInU);
            //Calcule de la correction dependant de l'orientation initiale et le GravityOffset
            InitialRotationInU = AbsoluteRotationInU * GravityOffsetInU;
        }
        /// <summary>
        /// Fonction qui reorganise les composantes d'un vecteur mesure dans le repere d'un capteur
        /// afin de l'exprimer dans Unity selon le systeme d'axes local associe au segment du corps de l'avatar
        /// Cette reorganisation dependant de la position du capteur sur le corps
        /// </summary>
        /// <param name="SensorVector"></param>
        /// <param name="CurId"></param>
        /// <returns></returns>
        public Vector3 SensorSystemMapToUnitySystem(Vector3 SensorVector, int CurId)
        {
            Vector3 TranslatedVectorS2U = new Vector3(0, 0, 0);

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
                TranslatedVectorS2U[0] = SensorVector[0];
                TranslatedVectorS2U[1] = SensorVector[1];
                TranslatedVectorS2U[2] = -SensorVector[2];
            }
            else if (CurId == 0)                                      //torse
            {
                TranslatedVectorS2U[0] = -SensorVector[1];
                TranslatedVectorS2U[1] = -SensorVector[0];
                TranslatedVectorS2U[2] = SensorVector[2];
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
            Vector3 RotAxvector = new Vector3(SensorQuat.x, SensorQuat.y, SensorQuat.z);
            Vector3 RotAxvectorS2U = SensorSystemMapToUnitySystem(RotAxvector, CurId);
            Quaternion TranslatedQuatS2U;
            //changement de signe de l'angle a cause du systeme gauche
            TranslatedQuatS2U.x = -RotAxvectorS2U[0];
            TranslatedQuatS2U.y = -RotAxvectorS2U[1];
            TranslatedQuatS2U.z = -RotAxvectorS2U[2];
            TranslatedQuatS2U.w = SensorQuat.w;
            return TranslatedQuatS2U;
        }

        void SetAxisOfRotation()
        {
            UpAxisRotation = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }

        public void UpdateAccel(Vector3 vNewAccel)
        {
            if(CurAccelVector.Equals(Vector3.zero))
            {
                CurAccelVector = vNewAccel;
            }
            else
            {
                //Use lowpass filter to extract the gravity vector from cumulative acceleration data
                CurAccelVector = Vector3.Lerp(CurAccelVector, vNewAccel, 0.15f);
            }
        }

        public void UpdateGyro(Vector3 vNewGyro)
        {
            CurGyroVector = vNewGyro;
        }

        void Awake()
        {
            IsReset = false;
            SetAxisOfRotation();
        }
    }
}