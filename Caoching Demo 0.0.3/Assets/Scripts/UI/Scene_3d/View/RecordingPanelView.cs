/** 
* @file RecordingPanelView.cs
* @brief Contains the RecordingPanelView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/ 
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    /// <summary>
    /// A recording Panel view in split screen scene
    /// </summary>
    public class RecordingPanelView : MonoBehaviour
    {
        public Button UpArrow;
        public Button DownArrow;
        public Button LeftSelectionRecording;
        public Button RightSelectionRecording;

        public GameObject AvailableRecordingButtonPrefab;
        private bool mImportCompleted;

        private int mCurrentIndex;
        public Scrollbar Scrollbar;

        private PlayerStreamManager mPlayerStreamManager;

        public PlayerStreamManager PlayerStreamManager
        {
            get
            {
                if (mPlayerStreamManager == null)
                {
                    mPlayerStreamManager = FindObjectOfType<PlayerStreamManager>();
                }
                return mPlayerStreamManager;
            }
        }

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            LeftSelectionRecording.onClick.AddListener(LeftButtonEngaged);
            RightSelectionRecording.onClick.AddListener(RightButtonEngaged);
            UpArrow.onClick.AddListener(UpButtonEngaged);
            DownArrow.onClick.AddListener(DownButtonEngaged);
        }

        public void Show()
        {
            if (!mImportCompleted)
            {
                Transform vContentPanel = SceneObjectFinder<Transform>.FindObjectByName(transform, "ScrollView") as Transform;
                string[] vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                if (vRecordingsFiles == null)
                {
                    BodyRecordingsMgr.Instance.ScanRecordings(ApplicationSettings.PreferedRecordingsFolder);
                    vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                }
                if (vContentPanel != null)
                {
                    vContentPanel = vContentPanel.GetChild(0);
                    for (int i = 0; i < vRecordingsFiles.Length; i++)
                    {

                        GameObject vNewAvRecButton = Instantiate(AvailableRecordingButtonPrefab);
                        Button vAvRecButton = vNewAvRecButton.GetComponentInChildren<Button>();
                        if (i == 0)
                        {
                        }
                        string vCleanedName = vRecordingsFiles[i].Replace(ApplicationSettings.PreferedRecordingsFolder + "\\", null);
                        vNewAvRecButton.GetComponentInChildren<Text>().text = vCleanedName;

                        //copy the variable i and pass it into the listener
                        int vTemp = i;
                        vAvRecButton.onClick.AddListener(() => ChooseAndPlayRecording(vTemp));
                        vNewAvRecButton.transform.SetParent(vContentPanel, false);
                    }
                }
                mImportCompleted = true;
            }
            gameObject.SetActive(true);
        }


        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
           PlayerStreamManager.ChangeState(PlayerStreamManager.BodyPlaybackState.Waiting);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Chooses and plays the recording
        /// </summary>
        /// <param name="vRecordingIndex"></param>
        private void ChooseAndPlayRecording(int vRecordingIndex)
        {
            mCurrentIndex = vRecordingIndex;
            PlayerStreamManager.RequestRecordingForPlayback(vRecordingIndex); 
        }



        /// <summary>
        /// The right button has been pressed
        /// </summary>
        private void RightButtonEngaged()
        {
            mCurrentIndex++;
            if (mCurrentIndex > BodyRecordingsMgr.Instance.FilePaths.Length)
            {
                mCurrentIndex = 0;
            }
            ChooseAndPlayRecording(mCurrentIndex);
        }

        /// <summary>
        /// the left button has been pressed
        /// </summary>
        private void LeftButtonEngaged()
        {
            mCurrentIndex--;
            if (mCurrentIndex < 0)
            {
                mCurrentIndex = BodyRecordingsMgr.Instance.FilePaths.Length;
            }
            ChooseAndPlayRecording(mCurrentIndex);
        }

        /// <summary>
        /// the up arrow button has been engaged
        /// </summary>
        private void UpButtonEngaged()
        {
            float vNewVal = Scrollbar.value + 0.12f;
            Scrollbar.value = vNewVal;
        }

        /// <summary>
        /// the down arrow button has been engaged
        /// </summary>
        private void DownButtonEngaged()
        {
            float vNewVal = Scrollbar.value - 0.12f;
            Scrollbar.value = vNewVal;
        }
    }
}
