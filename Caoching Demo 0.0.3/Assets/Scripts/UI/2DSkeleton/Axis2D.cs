/** 
* @file Axis2D.cs
* @brief Contains the Axis2D class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.UI._2DSkeleton
{
    /// <summary>
    /// Draws a an axis with specified paramaters
    /// </summary>
    public class Axis2D : MonoBehaviour
    {
        public Transform TransformOriginPoint;
        public float HorizontalWidth;
        public float VerticalWidth;
        public Color AxisXColor;
        public Color AxisYColor;
        public LineRenderer AxisX;
        public LineRenderer AxisY;

        void Awake()
        {
            

        }

        void Update()
        {
            UpdatePosition();
        }

        /// <summary>
        /// Initialize the axis and set the passed in transform as the parent
        /// </summary>
        /// <param name="vTransformOriginPoint"></param>
       public void Initialize(Transform vTransformOriginPoint)
        {
            TransformOriginPoint = vTransformOriginPoint;
            
            transform.SetParent(TransformOriginPoint);
            transform.localPosition = Vector3.zero;
            UpdatePosition();
        }

        /// <summary>
        /// Updates the position of the axis
        /// </summary>
        void UpdatePosition()
        { 
            AxisX.SetPosition(0, TransformOriginPoint.position + Vector3.left * HorizontalWidth / 2f);  
            AxisX.SetPosition(1, TransformOriginPoint.position +  Vector3.right * HorizontalWidth / 2f); 

            AxisY.SetPosition(0, TransformOriginPoint.position +Vector3.down * VerticalWidth / 2f); 
            AxisY.SetPosition(1, TransformOriginPoint.position +Vector3.up * VerticalWidth / 2f);
        }
    }
}
