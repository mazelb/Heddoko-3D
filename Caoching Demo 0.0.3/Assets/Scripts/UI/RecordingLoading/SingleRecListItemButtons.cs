// /**
// * @file SingleRecListItemButtons.cs
// * @brief Contains the  SingleRecListItemButtons class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.UI.RecordingLoading.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// A list of buttons for SingleRecordingListItemComponent 
    /// </summary>
    public class SingleRecListItemButtons : MonoBehaviour
    {
        public Button PlayButton;
        public SynchronizableRecordingListController Controller;
        private RecordingListItem mCurrentItem;

        void Awake()
        {
            PlayButton.onClick.AddListener(() =>
            {
                if (mCurrentItem != null)
                {
                    Controller.PlayRecording(ref mCurrentItem);
                }
            });
        }
        public void SetData(RecordingListItem vItem)
        {
            mCurrentItem = vItem;
        }
    }
}