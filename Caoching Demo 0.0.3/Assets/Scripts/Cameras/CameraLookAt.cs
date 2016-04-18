using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Simple camera look at script
    /// </summary>
    public class CameraLookAt : MonoBehaviour
    {

        public Transform Target;
        public Vector3 Offset;
        public Vector3 TargetPos;
        /// <summary>
        /// Uses a vector3 position instead of a transform target
        /// </summary>
        public bool UsePosition;

        void Update()
        {
            if (Target)
            {
                 if (!UsePosition)
                 {
                     transform.LookAt(Target.position + Offset);
                 }
                 else
                 {
                     transform.LookAt(TargetPos);
                 } 
            }
             
        }
    }
}
