// /**
// * @file RenderedBodyDebugComponents.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 12 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class RenderedBodyDebugComponents : MonoBehaviour
    {
        public AxisViewContainer[] AxisViewContainer;
        private bool mIsEnabled = false;
        public bool DebugModeEnabled { get; set; }

        void Start()
        {
#if DEBUG
            mIsEnabled = true;
            DebugModeEnabled = true;
            for (int i = 0; i < AxisViewContainer.Length; i++)
            {
                AxisViewContainer[i].Show();
            }
#endif
        }

        /// <summary>
        /// Set the rendering layer
        /// </summary>
        public void SetLayer(LayerMask vLayer)
        {
            for (int i = 0; i < AxisViewContainer.Length; i++)
            {
                AxisViewContainer[i].SetLayer(vLayer);
            }
        }

        /// <summary>
        /// Flips the visibility of the axis containers
        /// </summary>
        public void FlipVisibility()
        {
            mIsEnabled = !mIsEnabled;
            if (mIsEnabled)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Show()
        {
            for (int i = 0; i < AxisViewContainer.Length; i++)
            {
                AxisViewContainer[i].Show();
            }
            mIsEnabled = true;
        }

        public void Hide()
        {
            for (int i = 0; i < AxisViewContainer.Length; i++)
            {
                AxisViewContainer[i].Hide();
            }
            mIsEnabled = false;
        }
    }
}