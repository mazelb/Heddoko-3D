// /**
// * @file RecordingIndexValue.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @ heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// Display an index value
    /// </summary>
    public class RecordingIndexValue : MonoBehaviour
    {
        public Text IndexValueLabel;

        public void SetIndexValue(int vIndexVal)
        {
            if (vIndexVal < 0)
            {
                vIndexVal = 0;
            }
            IndexValueLabel.text = "" + vIndexVal;
        }
    }
}