
/**  
* @file VectorDraw.cs 
* @brief Contains the VectorDraw class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ArcAngle
{
    /// <summary>
    /// Takes in  two Images representing a vector in 2d space. The second vector orients itself according to the ArcAngleFill angle 
    /// </summary>
   public class VectorDraw : MonoBehaviour
    {
        public ArcAngleFill ArcAngleFill;
        public Image VectorImage1;
        public Image VectorImage2;
        public float RotationFactor= -1f;

        void Update()
        {
            Quaternion vNewRotation = Quaternion.Euler(0, 0, ArcAngleFill.Angle * RotationFactor); 
            VectorImage2.transform.localRotation = vNewRotation;
        }

    }
}
