/** 
* @file CameraOrbitterCentralObserver.cs
* @brief Contains the CameraOrbitterCentralObserver class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic; 

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls
{
    /// <summary>
    /// A central observer that notifies CameraOrbitterObjects of other camera orbitter state changes.
    /// </summary>
public class CameraOrbitterCentralObserver
    {
        private static List<CameraOrbitter> sCameraOrbitterSet = new List<CameraOrbitter>();

        /// <summary>
        /// Add a camera orbitter to the set
        /// </summary>
        /// <param name="vCameraOrbitter"></param>
        public static void Add(CameraOrbitter vCameraOrbitter )
        {
            if (!sCameraOrbitterSet.Contains(vCameraOrbitter))
            {
                sCameraOrbitterSet.Add(vCameraOrbitter);
            }
        }

        /// <summary>
        /// When an orbitter enters a valid mouse clicked state, disable all other orbitters
        /// </summary>
        /// <param name="vCameraOrbit">the camera orbitter to be left enabled</param>
        public static void MouseClickedNotification(CameraOrbitter vCameraOrbit)
        {
            for (int i = 0; i < sCameraOrbitterSet.Count; i++)
            {
                if (sCameraOrbitterSet[i].Equals(vCameraOrbit))
                {
                    continue;
                }
                sCameraOrbitterSet[i].DisableOrbitAction();
            }

        }
        /// <summary>
        /// When an orbitter exits a mouse released state, enabled all other orbitters
        /// </summary>
        /// <param name="vCameraOrbit">the camera orbitter that initiated the call</param>
        public static void MouseReleasedNotification(CameraOrbitter vCameraOrbit)
        {
            for (int i = 0; i < sCameraOrbitterSet.Count; i++)
            {
                if (sCameraOrbitterSet[i].Equals(vCameraOrbit))
                {
                    continue;
                }
                sCameraOrbitterSet[i].EnableOrbitAction();
            }
        }

        /// <summary>
        /// Disables all orbitters
        /// </summary>
        public static void DisableAllOrbitters()
        {
            for (int i = 0; i < sCameraOrbitterSet.Count; i++)
            {
                 
                sCameraOrbitterSet[i].DisableOrbitAction();
            }
        }

        /// <summary>
        /// Enables all Listening CameraOrbitters
        /// </summary>
        public static void EnableAllOrbitters()
        {
            for (int i = 0; i < sCameraOrbitterSet.Count; i++)
            {

                sCameraOrbitterSet[i].DisableOrbitAction();
            }
        }
    }
}
