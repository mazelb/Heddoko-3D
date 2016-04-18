/** 
* @file Vector3DAxis .cs
* @brief Contains the Vector3DAxi 
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using UnityEngine;

namespace Assets.Scripts.UI.ArcAngle
{
    /// <summary>
    /// A representation of an axis in 3d in the shape of an arrow
    /// </summary>
    public class Vector3DAxis: MonoBehaviour
    {
        public MeshRenderer ConeObj;
        public MeshRenderer RodObj;

        public void SetLayer(LayerMask vMask)
        { 
            gameObject.layer = vMask;
            ConeObj.gameObject.layer = vMask;
            RodObj.gameObject.layer = vMask;
        }
    }
}