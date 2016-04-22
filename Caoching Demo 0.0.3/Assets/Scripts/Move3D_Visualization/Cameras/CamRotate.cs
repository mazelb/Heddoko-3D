/**  
* @file CamRotate.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    public class CamRotate : MonoBehaviour
    {
        public Transform LookAtTransform;
        public float Speed;

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
               transform.RotateAround(LookAtTransform.position, Vector3.up, Speed);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.RotateAround(LookAtTransform.position, Vector3.up, -Speed);
            }
            transform.LookAt(LookAtTransform.position);
        }
    }
}
