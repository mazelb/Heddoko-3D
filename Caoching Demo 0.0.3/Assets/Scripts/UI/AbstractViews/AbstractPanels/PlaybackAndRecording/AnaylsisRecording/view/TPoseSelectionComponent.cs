// /**
// * @file TPoseSelectionComponent.cs
// * @brief Contains the TPoseSelectionComponent class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Body_Data.CalibrationData.TposeSelection;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording.view
{
    /// <summary>
    ///  A view component to the TPoseList
    /// </summary>
    public class TPoseSelectionComponent : ListViewItem, IResizableItem
    {
        public Button DeleteButton;
        public InputField LeftIndex;
        public InputField TPoseIndex;
        public InputField RightIndex;

        public GameObject[] ObjectsToResize
        {
            get
            {
                return new[]
                {DeleteButton.gameObject, LeftIndex.gameObject, TPoseIndex.gameObject, RightIndex.gameObject};
            }
        }


        public void SetData(TPoseSliderMask vItem)
        {
            DeleteButton.onClick.RemoveAllListeners();
            LeftIndex.onValueChanged.RemoveAllListeners();
            TPoseIndex.onValueChanged.RemoveAllListeners();
            RightIndex.onValueChanged.RemoveAllListeners();
           // LeftIndex.text = vItem.PoseIndexLeft.ToString();
          //  TPoseIndex.text = vItem.PoseIndex.ToString();
           // RightIndex.text = vItem.PoseIndexRight.ToString();
        }
    }
}