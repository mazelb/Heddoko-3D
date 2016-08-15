// /**
// * @file AnalysisToggleBoxContainer.cs
// * @brief Contains the AnalysisContainerToggle class
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
 
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Views
{
    /// <summary>
    /// Responsible for controlling the analysis container view toggle.
    /// </summary>
    public class AnalysisContainerToggle : MonoBehaviour
    {
        [SerializeField] private SlideBlock mSliderBlock;
        private string mCurrentKey = "closed";

        /// <summary>
        /// On button click, verify the state that the slider block is in, and closes it according to the current state of the analysis slider block container.
        /// </summary>
        /// <param name="vAnaylsisComponentKey"></param>
        public void OnSwitchToggle(string vAnaylsisComponentKey)
        {
            if (mCurrentKey.Equals(vAnaylsisComponentKey))
            {
                mSliderBlock.Toggle();
                mCurrentKey = "closed";
            }
            else
            {
                mCurrentKey = vAnaylsisComponentKey;
            }
        }
    }
}