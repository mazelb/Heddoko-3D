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
        public Button DownloadButton;
        public Image InProgressIcon;
        public SynchronizableRecordingListController Controller;
        private RecordingListItem mCurrentItem;

        void Awake()
        {
            DownloadButton.onClick.AddListener(() =>
            {
                if (mCurrentItem != null)
                {
                    Controller.ProcessRecording(ref mCurrentItem);
                }
            });
            PlayButton.onClick.AddListener(() =>
            {
                Controller.PlayRecording( mCurrentItem);
            });

        }
        public void SetData(RecordingListItem vItem)
        {
            if (vItem.Location.LocationType == RecordingListItem.LocationType.DownloadingAndUnavailable)
            {
                PlayButton.gameObject.SetActive(false);
                InProgressIcon.gameObject.SetActive(true);
                DownloadButton.gameObject.SetActive(false);
            }
            else if (vItem.Location.LocationType == RecordingListItem.LocationType.CachedLocal)
            {
                PlayButton.gameObject.SetActive(true);
                InProgressIcon.gameObject.SetActive(false);
                DownloadButton.gameObject.SetActive(false);
            }
            else if (vItem.Location.LocationType == RecordingListItem.LocationType.RemoteEndPoint)
            {
                PlayButton.gameObject.SetActive(false);
                InProgressIcon.gameObject.SetActive(false);
                DownloadButton.gameObject.SetActive(true);
            }
            mCurrentItem = vItem;
        }
    }
}