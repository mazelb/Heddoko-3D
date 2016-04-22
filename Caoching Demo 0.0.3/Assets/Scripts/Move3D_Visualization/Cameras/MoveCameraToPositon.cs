
/**  
* @file MoveCameraToPositon.cs 
* @brief Contains the MoveCameraToPositon class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using System.Collections; 
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Moves the camera towards a targetted position
    /// </summary>
    public class MoveCameraToPositon : MonoBehaviour
    {
        //always set the default positon to CamPositions[0]
        public Transform[] CamPositions;
        public Vector3 Offset { get; set; }
        
        private int mCurrentPos = 0;

        public int CurrentPosition
        {
            get
            {
                return mCurrentPos;
            }
        }

        public float Duration;
        public Transform Target;
        public Camera Cam;


        void OnDisable()
        {  
            StopAllCoroutines();
        }

        void OnEnable()
        {

            // Cam.orthographic = true;
            StartCoroutine(MoveCam());
        }
        private IEnumerator MoveCam()
        {
            //get current pos
            Vector3 vPos = transform.position;
            if (CamPositions.Length > 0)
            {
               
               for (float i = 0; i < 1.0f; i += Time.deltaTime / Duration)
                {

                    vPos = Vector3.Slerp(vPos, CamPositions[mCurrentPos].position, i);
                   transform.position = vPos;
                    transform.LookAt(Target.position + Offset);
                    yield return null;
                     
                } 
                
               
            }
        }

        /// <summary>
        /// Moves to the next transform position
        /// </summary>
        public void MovetoNextPos()
        {
            
            if (this.enabled)
            {
                enabled = false;
            }
            if ( ++mCurrentPos >= CamPositions.Length)
            {
           
                mCurrentPos = 0;
            }  
            this.enabled = true;
        }

        /// <summary>
        /// Moves to the next transform position
        /// </summary>
        public void MoveToPrevPos()
        {

            if (this.enabled)
            {
                enabled = false;
            }
            if (--mCurrentPos <0)
            {

                mCurrentPos = CamPositions.Length-1;
            }
            this.enabled = true;
        }
        /// <summary>
        /// Move to a specific transform position
        /// </summary>
        /// <param name="vIndex"></param>
        public void MoveToPos(int vIndex)
        {
            if (this.enabled)
            {
                enabled = false;
            }

            //prevent out of bound exceptions
            if (vIndex >= CamPositions.Length)
            {
                vIndex = CamPositions.Length -1;
            }

            if (vIndex < 0)
            {
                vIndex = 0;
            }
        
            mCurrentPos = vIndex;
            this.enabled = true;

        }
    }
}
