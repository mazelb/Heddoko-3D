
/**  
* @file UIElementFollowTarget.cs 
* @brief Contains the UIElementFollowTarget class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using UnityEngine;

namespace Assets.Scripts.UI.ArcAngle
{
    /// <summary>
    /// Given a canvas that is set in world space, this script will follow a target  
    /// </summary>
  public class UIWorldCanvasFollowTarget : MonoBehaviour
    {
        public Transform Target;
        public Vector3 Offset;
        public Camera FollowCam;
        private void Update()
        {
            transform.position = Target.position + Offset;
            /*  mCurrTransform = transform;
            mCurrTransform.SetParent(Target);
            mCurrTransform.localPosition = SetPosition;*/
        }
         
    }
}
