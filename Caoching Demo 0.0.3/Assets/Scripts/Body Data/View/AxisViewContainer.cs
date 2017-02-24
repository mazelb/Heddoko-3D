// /**
// * @file AxisViewContainer.cs
// * @brief Contains the AxisViewContainer class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class AxisViewContainer: MonoBehaviour
    {
        public AxisView X;
        public AxisView Y;
        public AxisView Z;
        public void Show()
        {
           gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetLayer(int vLayerMask)
        {
            X.SetLayer(vLayerMask);
            Y.SetLayer(vLayerMask);
            Z.SetLayer(vLayerMask);
        }
    }
}