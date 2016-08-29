// /**
// * @file CloudLocalStorageViewManager.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.RecordingLoading.View;
using Assets.Scripts.UI.Settings;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading
{
    
    public delegate void RecordingLoadingComplete(BodyFramesRecordingBase vObj);
    public class CloudLocalStorageViewManager: MonoBehaviour
    {
        public BodyControlPanel BodyControlPanel;
        public LocalRecordingView LocalRecordingView;
        public RecordingListViewController RecordingListViewController;
        
        public Tabs Tabs;
        public event RecordingLoadingComplete RecordingLoadingCompleteEvent;
        public Camera RenderingCamera;
        private RecordingLoader mRecordingLoader;
        void Start()
        {
            Tabs.OnTabSelect.AddListener(HideBodyControlPanel);
            mRecordingLoader = new RecordingLoader();
            LocalRecordingView.RecFileSelectedEvent += RecordingSelectedStringHandler;
            RecordingListViewController.RecordingToBePlayedEvent += RecordingSelectedRecItemHandler;
        }

        /// <summary>
        /// Recording selected with its recordinglist item
        /// </summary>
        /// <param name="vItem"></param>
        private void RecordingSelectedRecItemHandler(RecordingListItem vItem)
        {
            Hide();
            mRecordingLoader.LoadFile(vItem.Location.RelativePath, InvokeRecordingPlayback);
        }

        /// <summary>
        /// Recording selected with a relative path in string form
        /// </summary>
        /// <param name="vRelativepath"></param>
        private void RecordingSelectedStringHandler(string vRelativepath)
        {
            Hide();
            mRecordingLoader.LoadFile(vRelativepath,InvokeRecordingPlayback);
        }

        /// <summary>
        /// Invoke recording playback function
        /// </summary>
        /// <param name="vObj"></param>
        private void InvokeRecordingPlayback(BodyFramesRecordingBase vObj)
        {
           
            if (RecordingLoadingCompleteEvent != null)
            {
                RecordingLoadingCompleteEvent(vObj);
            }
        }

        /// <summary>
        /// hides the body control panel
        /// </summary>
        /// <param name="vArg0"></param>
        private void HideBodyControlPanel(int vArg0)
        {
            if (BodyControlPanel.PanelIsVisible())
            {
                BodyControlPanel.HidePanel();
            }
        }

        /// <summary>
        /// Hide the view
        /// </summary>
        public void Hide()
        {
            StopCoroutine(DisplayLocalRecordingSelectionViewAfterDelay());
            LocalRecordingView.Hide();
            RenderingCamera.gameObject.SetActive(false);
           
        }

        /// <summary>
        /// shows the view
        /// </summary>
        public void Show()
        {
            RenderingCamera.gameObject.SetActive(true);
            if (BodyControlPanel.PanelIsVisible())
            {
                BodyControlPanel.HidePanel();
            }
            StartCoroutine(DisplayLocalRecordingSelectionViewAfterDelay());
        }

        /// <summary>
        /// Enables the view based on the flag that was passed in
        /// </summary>
        /// <param name="vFlag"></param>
        public void Enable(bool vFlag)
        {
            if (vFlag)
            {
                Show();
            }
            else
            {
                Hide();
            }
         }

        IEnumerator DisplayLocalRecordingSelectionViewAfterDelay()
        {
            yield return new WaitForSeconds(0.15f);
                LocalRecordingView.Show();
        }
    }
}