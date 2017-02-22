
/** 
* @file ArcAngleFill.cs
* @brief ArcAngleFill class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Cameras;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Calculate the angle between two vectors and sets the Arc gameobject to be oriented in the direction between the two vectors
    /// </summary>
    public class ArcAngleFill : MonoBehaviour
    {

        public Vector3 PositionOffset;

        public Image ImageToFill;
        public GameObject ArcCanvas;
        public Transform ZeroElbowVector;
        public Transform RightUpperArm;
        private float mFill;
        private float mAngle;
        public float MinimalScale;
        public float MaxScale;
        public PlayerStreamManager PlayerStreamManager;
        public Camera WorldSpaceUiCamera;
        public Vector3 NormalToThePlane;
        public Text DisplayAngleText; 
        public float TextMagnitude = 0.5f;
        public float ShowTime=1f;
        
        /// <summary>
        /// The angle of the arc
        /// </summary>
        public float Angle
        {
            get
            {
                return mAngle;
            }
        }

        private void Update()
        {
            Body vBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vBody != null)
            {
                UpdateTransform();
            }
        }


        /// <summary>
        /// Updates the orientation of the canvas to be perpendicular to the camera
        /// </summary> 
        public void UpdateTransform()
        {
            Vector3 vUpVector =  NormalToThePlane;

            //Get the projection of the perfect Vector
            Vector3 vPerfectVectProjection = Vector3.ProjectOnPlane(ZeroElbowVector.right, vUpVector);

            //Get the projection of the elbow
            Vector3 vElbowVector = Vector3.ProjectOnPlane(RightUpperArm.right, vUpVector);
            Quaternion Rotation = Quaternion.LookRotation(vUpVector, vPerfectVectProjection);
            transform.rotation = Rotation;

            transform.position = RightUpperArm.position + PositionOffset;
             
            Vector3 vCross = Vector3.Cross(vPerfectVectProjection, vElbowVector);
            float vSign = Mathf.Sign(Vector3.Dot(vUpVector, vCross));
            mAngle = Vector3.Angle(vPerfectVectProjection, vElbowVector);
            Vector3 vHalfwayVector3 = vPerfectVectProjection + vElbowVector;
            vHalfwayVector3.Normalize();
            DisplayAngleText.transform.position = transform.position + vHalfwayVector3* TextMagnitude;

            mFill = mAngle / 360f;

            //set the image fill from the angles between two vectors
            ImageToFill.fillAmount = mFill;
          
            if (vSign < 0)
            {
                ImageToFill.fillClockwise = true; 
            }

            else
            {
                ImageToFill.fillClockwise = false;
                mAngle *= -1;
            }

            DisplayAngleText.text = (int)mAngle + "°";



        }

        /// <summary>
        /// Start the object display process
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(ScaleTransform(1, 1f, false));
        }

        /// <summary>
        /// Start the hiding process
        /// </summary>
        public void Hide()
        {
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ScaleTransform(0, 0.4f, true));
            }
        }


        /// <summary>
        /// Scales the transform with respect to Time.delta Time. and optional parameter to disable the gameobject
        /// </summary>
        /// <param name="vScale">the scale to interpolate towards</param>
        /// <param name="vTimeScale">Scale the time  </param>
        /// <param name="vDisableGO">Flag to disable once complete</param>
        /// <returns></returns>
        private IEnumerator ScaleTransform(float vScale,float vTimeScale, bool vDisableGO)
        {
            float vStartTime = 0;
            Vector3 vStartScale = transform.localScale;
            Vector3 vEndScale = Vector3.one*vScale;
            float vPercentage = 0;
            float vEndTime =  ShowTime*vTimeScale;
            while (true)
            {
                vStartTime += Time.deltaTime;
                vPercentage = vStartTime/vEndTime;

                //use smoothstep interpolation
                vPercentage = vPercentage*vPercentage*vPercentage* (vPercentage*(6f*vPercentage - 15f) + 10f);
              
                if (vPercentage >= 1)
                {
                    transform.localScale = vEndScale;
                    break;
                }

                Vector3 vNewScale = Vector3.Lerp(vStartScale, vEndScale, vPercentage);
                transform.localScale = vNewScale;
                yield return null;
            }

            if (vDisableGO)
            {
                gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// From the given point parameters, set the zero vector's orientation, the sprite used by the arc image fill and the plane normal 
        /// </summary>
        /// <param name="vPointParameters"></param>
        public void SetParametersFromPoint(CameraMovementPointSetting vPointParameters)
        {
            PositionOffset = vPointParameters.TransformOffset;
            NormalToThePlane = vPointParameters.PlaneNormal;
            Sprite vNewArcSprite = vPointParameters.ArcSprite;
            if (vNewArcSprite != null)
            {
                ImageToFill.sprite = vPointParameters.ArcSprite;
                UpdateTransform();
            }
            else
            {
                Hide();
            }

        }
    }
}
