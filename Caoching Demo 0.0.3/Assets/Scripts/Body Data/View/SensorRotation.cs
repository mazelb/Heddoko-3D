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
using System.Text;
namespace Assets.Scripts.Body_Data.View
{
    public class SensorRotation : MonoBehaviour
    {
        //private bool mCalState               = false;
        //private bool mMagState               = false;
        public  bool UseCorrection           = false;
        public  bool IsReset                 = false;

        public Vector3 AxisOfRotation        = new Vector3(0, 180, 0);
        public Quaternion UpAxisRotation;
        public Quaternion AbsoluteRotation   = Quaternion.identity; // quaternion des capteurs
        public Quaternion AbsoluteRotationInU= Quaternion.identity; // quaternion traduit dans le systeme de Unity
        public Quaternion InitialRotation    = Quaternion.identity; // quaternion associe aux valeurs initiales renvoyees par les capteurs 
                                                                    // et corrigees pour tenir compte du decalage verticale (gravity offset) 
        public Quaternion InitialRotationInU = Quaternion.identity; // traduction de InitialRotation dans le systeme de Unity       
        public Quaternion GravityOffsetInU   = Quaternion.identity; // quaternion de correction par rapport a la verticale           
        public Quaternion MagOffsetInU       = Quaternion.identity; // quaternion de correction dans le plan horizontal
        //public Quaternion MagOffsetInG     = Quaternion.identity; // quaternion de correction dans le plan horizontal
        public Vector3 CurAccelVectorInS     = Vector3.zero;        // valeur actuelle de l'acceleration lineaire provenant des accelerometres
        public Vector3 CurGyroVectorInS      = Vector3.zero;        // valeur actuelle de la vitesse angulaire porvenant des gyroscopes
        public Vector3 CurMagInS             = Vector3.zero;        // valeur actuelle champ magnetique
        public int     CurId                 = -1          ;

        [SerializeField]
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Reset();
            }
            if (IsReset)
            {
                Quaternion vCurRotationInU   = Quaternion.identity;
                Quaternion vExpectedRotation = Quaternion.identity;
                Quaternion vGravityOffset    = Quaternion.identity;
                //Quaternion vMagOffset        = Quaternion.identity;   //quaternion de Correction de l'orientation du champ magnetique obtenu sur la base d'une comparaison entre
                                                                      //l'avant moyen sur 9 capteurs, et le champ magnetique exterieur moyen sur 9 capteurs 

                //Correction des orientations actuelles en tenant compte des valeurs initiales des quaternions,
                //de la correction verticale (correction gravitationnelle), et la correction dans le plan orizontal (champ magnetique)                
                vCurRotationInU = Quaternion.Inverse(InitialRotationInU * MagOffsetInU * GravityOffsetInU) * (AbsoluteRotationInU  * MagOffsetInU * GravityOffsetInU); 
                //Filtrage passe-bas
                Quaternion vNewRotationInU   = Quaternion.Slerp(transform.rotation, vCurRotationInU, 0.7f);
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
            ////Correction verticale (Gravitationnelle)
            Vector3 CurAccelVectorInSNormed = CurAccelVectorInS.normalized;     //Normalisation de l'acceleration lineaire actuelle
            Vector3 vExpectedGravityInU     = Vector3.down;                     //Orientation attendue de la gravite pour l'avatar (0,-1,0)
            //traduction du vecteur acceleration mesure par les capteurs dans Unity  
            Vector3 SensorAcceleroVectorInU = SensorSystemMapToUnitySystem(CurAccelVectorInSNormed, CurId);
            //Calcul (dans Unity) du decalage entre la mesure de l'acceleration actuelle InU (supposee  = a la gravite) et celle Attendue   
            GravityOffsetInU = Quaternion.FromToRotation(vExpectedGravityInU, SensorAcceleroVectorInU);
            //Calcule de la correction dependant de l'orientation initiale et le GravityOffset
            InitialRotationInU = AbsoluteRotationInU * GravityOffsetInU ;              
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
            UpAxisRotation  = Quaternion.Euler(AxisOfRotation);
            InitialRotation = UpAxisRotation;
        }

        public void UpdateAccel(Vector3 vNewAccel)
        {
            if(CurAccelVectorInS.Equals(Vector3.zero))
            {
                CurAccelVectorInS = vNewAccel;
            }
            else
            {
                //Use lowpass filter to extract the gravity vector from cumulative acceleration data
                CurAccelVectorInS = Vector3.Lerp(CurAccelVectorInS, vNewAccel, 0.15f);
            }
        }

        public void UpdateGyro(Vector3 vNewGyro)
        {
            CurGyroVectorInS = vNewGyro;
        }
       
       /// <summary>
       /// methode de mise a jour du champ magnetique
       /// </summary>
       /// <param name="vNewMag"></param>
        public void UpdateMag(Vector3 vNewMag)
        {
            CurMagInS = vNewMag;            
        }
       
        void Awake()
        {
            IsReset = false;
            SetAxisOfRotation();
        }
    }
}
//Vector3 CurMagInSNormed = CurMagInS.normalized;                   ///Champ magnetique tel que mesure localement par un capteur (InS) et norme
//Vector3 CurMagInGNormed = AbsoluteRotation*CurMagInS.normalized;  ///Champ magnetique tel que decrit dans un referentiel exterieur, les composantes vecteurs
///sont independantes de l'orientation du capteur   
/////Vecteur unitaire courant de la composante perpendiculaire a la gravite du champ magnetique exterieur moyen (sur 9 capteurs) 
//Vector3 CurMagHInGNormed = SensorContainer.VProjectionPerp2W(CurMagInGNormed, RefAccelgInGNormed).normalized;
//MagOffsetInG             = Quaternion.FromToRotation(CurMagHInGNormed, RefMagHInGNormed);
//            MagOffsetInU             = SensorSystemMapToUnitySystem(MagOffsetInG, CurId);

/////Vecteur avant courant pour un capteur  dans la base exterieure
//Vector3 PersonFowardInS = new Vector3(0.0f, 0.0f, 1.0f);
//            if (CurId > 0 && CurId< 5){PersonFowardInS = new Vector3(0.0f, 0.0f,-1.0f);}
//            Vector3 CurForwInGNormed = (AbsoluteRotation * PersonFowardInS).normalized;
//Vector3 CurForwHInGNormed = SensorContainer.VProjectionPerp2W(CurForwInGNormed, RefAccelgInGNormed).normalized;
//Vector3 MeanForwardHInGNormed = SensorContainer.VProjectionPerp2W(SensorContainer.MeanFowardInG, RefAccelgInGNormed).normalized;

//Quaternion ForwOffsetInG = Quaternion.FromToRotation(CurForwHInGNormed, MeanForwardHInGNormed);
//Quaternion ForwOffsetInU = SensorSystemMapToUnitySystem(ForwOffsetInG, CurId);

//Quaternion HorizontalOffsetInU = Quaternion.Slerp(MagOffsetInU, ForwOffsetInU, 0.5f);
//MagOffsetInU = HorizontalOffsetInU;

////Correction horizontale (champ magnetique et direction avant)
///Vecteur unitaire de reference : la composante perpendiculaire a la gravite du champ magnetique exterieur moyen (sur 9 capteurs) 
/////Vecteur unitaire de reference : champ gravitationnel moyen exterieur (sur 9 capteurs et sur la valeur du temps precedant)
//Vector3 RefMagHInGNormed = SensorContainer.MeanMagH.normalized;
//Vector3 RefAccelgInGNormed = SensorContainer.MeanGravFieldInG.normalized;

/////Champ magnetique courant pour un capteur et Champs magnetique exprimer dans le repere exterieur           
//Vector3 CurMagInSNormed = CurMagInS.normalized;                     ///Champ magnetique tel que mesure localement par un capteur (InS) et norme
//Vector3 CurMagInGNormed = AbsoluteRotation * CurMagInS.normalized;  ///Champ magnetique tel que decrit dans un referentiel exterieur, les composantes vecteurs
//                                                                    ///sont independantes de l'orientation du capteur
//                                                                    ///Vecteur avant courant pour un capteur  dans la base exterieure
//Vector3 CurMagHInGNormed = SensorContainer.VProjectionPerp2W(CurMagInGNormed, RefAccelgInGNormed).normalized;

//Vector3 PersonFowardInS = new Vector3(0.0f, 0.0f, 1.0f);
//            if (CurId > 0 && CurId< 5) { PersonFowardInS = new Vector3(0.0f, 0.0f, -1.0f); }

//            Vector3 CurForwInGNormed = (AbsoluteRotation * PersonFowardInS).normalized;
//Vector3 CurForwHInGNormed = SensorContainer.VProjectionPerp2W(CurForwInGNormed, RefAccelgInGNormed).normalized;
//Quaternion MagForwQuatInS = Quaternion.FromToRotation(CurMagHInGNormed, CurForwHInGNormed);

//Vector3 MeanForwardHInGNormed = SensorContainer.VProjectionPerp2W(SensorContainer.MeanFowardInG, RefAccelgInGNormed).normalized;
//Quaternion RefMagForwQuatInG = Quaternion.FromToRotation(RefMagHInGNormed, MeanForwardHInGNormed);
//MagOffsetInU = Quaternion.Inverse(MagForwQuatInS) * RefMagForwQuatInG;