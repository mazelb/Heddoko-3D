/** 
* @file SingleRecordingSelection.cs
* @brief Contains the SingleRecordingSelection  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.IO;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tests
{
    public delegate void StartLoading();

    public delegate void FinishLoading();
    /// <summary>
    /// singleton that loads a single recording
    /// </summary>
    public class SingleRecordingSelection : MonoBehaviour
    {
        public Rect mRect;
        private static SingleRecordingSelection sInstance;
        private Action<BodyFramesRecordingBase> mRecordingLoadedCallback;
        public StartLoading StartLoadingEvent;
        public FinishLoading FinishLoadingEvent;
        public static Action<BodyFrame[]> ReadProtoFileAction;
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
        public void OpenFileBrowseDialog(Action<BodyFramesRecordingBase> vCallback = null)
        {
            SetTransform();
            DisablerPanel.SetActive(true);
            if (vCallback != null)
            {
                mRecordingLoadedCallback = vCallback;
            }
            var vPaths = new[] { "dat", "hsm" };
            //initialize the browser settings
#if DEBUG  
            vPaths = new[] { "csv", "dat", "hsm" };
#endif
            UniFileBrowser.use.SetFileExtensions(vPaths);
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
            FileInfo vInfo = new FileInfo(vRecordingSelected);
            if (vInfo.Name.Equals("logindex.dat", StringComparison.CurrentCultureIgnoreCase))
            {
                string vTopLabel = "CANNOT OPEN A LOGINDEX FILE";
                string vContent =
                    "Unfortunately, this file is not a recording file. Typical recordings are longer and start with SXXXX, where the X represents a number. Would you like to try again?";
                UnityAction vOnYes = () => OpenFileBrowseDialog();
                UnityAction vOnNo = () => { };
                ModalPanel.Instance().Choice(vTopLabel, vContent, vOnYes, vOnNo);
                return;
            }
            if (StartLoadingEvent != null)
            {
                StartLoadingEvent();
            }
            //Verify the file first, if it's a logindex.dat file, then throw an error
            
            ApplicationSettings.PreferedRecordingsFolder = vInfo.DirectoryName;
            BodyRecordingsMgr.Instance.ScanRecordings(UniFileBrowser.use.filePath);
            BodyRecordingsMgr.Instance.ReadRecordingFile(vRecordingSelected, BodyFramesRecordingCallback);
        }

        /// <summary>
        /// once loading is completed, this callback is reached. Note: invokes the member callback.
        /// </summary>
        /// <param name="vRecording"></param>
        private void BodyFramesRecordingCallback(BodyFramesRecordingBase vRecording)
        {
            if (mRecordingLoadedCallback != null)
            {
                if (!OutterThreadToUnityThreadIntermediary.InUnityThread())
                {
                    OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
                    {
                        if (FinishLoadingEvent != null)
                        {
                            FinishLoadingEvent();
                        }
                        mRecordingLoadedCallback.Invoke(vRecording);
                    });
                }
                else
                {
                    if (FinishLoadingEvent != null)
                    {
                        FinishLoadingEvent();
                    }
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