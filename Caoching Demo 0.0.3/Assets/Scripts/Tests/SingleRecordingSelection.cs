/** 
* @file SingleRecordingSelection.cs
* @brief Contains the SingleRecordingSelection  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// singleton that loads a single recording
    /// </summary>
    public class SingleRecordingSelection : MonoBehaviour
    {
        public Rect mRect;
        private static SingleRecordingSelection sInstance;
        private Action<BodyFramesRecording> mRecordingLoadedCallback;

        //Panel that will cover other ui elements, thereby dissallowing their controls
        public GameObject DisablerPanel;

        public RectTransform SizeControlRectTransform;

        public static SingleRecordingSelection Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = GameObject.FindObjectOfType<SingleRecordingSelection>();
                    DontDestroyOnLoad(sInstance.gameObject);
                }
                return sInstance;
            }
        }

        private void Start()
        {
            UniFileBrowser.use.SendWindowCloseMessage(HideDisablerPanel);

        }

        /// <summary>
        /// opens a File browser dialog to select a recording with an optional callback after file is completed loading
        /// </summary>
        public void OpenFileBrowseDialog(Action<BodyFramesRecording> vCallback = null)
        {
            SetTransform();
            DisablerPanel.SetActive(true);
            // SetTransform();
            mRecordingLoadedCallback = vCallback;
            //initialize the browser settings
            UniFileBrowser.use.SetFileExtensions(new[] { "csv", "dat" });
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.OpenFileWindow(SelectRecordingFile);
        }

        /// <summary>
        /// Callback, on file selection
        /// </summary>
        /// <param name="vRecordingSelected"></param>
        private void SelectRecordingFile(string vRecordingSelected)
        {
            BodyRecordingsMgr.Instance.ScanRecordings(UniFileBrowser.use.filePath);
            BodyRecordingsMgr.Instance.ReadRecordingFile(vRecordingSelected, BodyFramesRecordingCallback);
        }

        /// <summary>
        /// once loading is completed, this callback is reached. Note: invokes the member callback.
        /// </summary>
        /// <param name="vRecording"></param>
        private void BodyFramesRecordingCallback(BodyFramesRecording vRecording)
        {

            if (mRecordingLoadedCallback != null)
            {

                if (!OutterThreadToUnityThreadIntermediary.InUnityThread())
                {
                    OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => mRecordingLoadedCallback.Invoke(vRecording));
                }
                else
                {
                    mRecordingLoadedCallback.Invoke(vRecording);
                }
            }

        }

        /// <summary>
        /// Disables the disabler panel
        /// </summary>
        private void HideDisablerPanel()
        {
            DisablerPanel.SetActive(false);
        }
        /// <summary>
        /// Sets the transform of the unifilebrowser. It uses the old UI system as its front end
        /// </summary>
        private void SetTransform()
        {
            Rect vSizeRect = mRect = RectTransformToScreenSpace(SizeControlRectTransform);
            Vector2 vPos = new Vector2(vSizeRect.x, vSizeRect.y);
            Vector2 vSize = new Vector2(vSizeRect.width, vSizeRect.height);
            UniFileBrowser.use.SetFileWindowSize(vSize);
            UniFileBrowser.use.SetFileWindowPosition(vPos);
        }

        /// <summary>
        /// get the recttransform's rect and rect of its rect in screen space
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 vSize = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (vSize * 0.5f), vSize);
        }

    }
}