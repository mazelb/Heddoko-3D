// /**
// * @file AxisView.cs
// * @brief Contains the AxisView class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// A view of a 3d axis
    /// </summary>
  public   class AxisView:MonoBehaviour
    {
        public GameObject Cone;
     

        public void SetLayer(LayerMask vCurrLayerMask)
        {
            gameObject.layer = vCurrLayerMask;
            Cone.layer = vCurrLayerMask;
        }
    }
}
