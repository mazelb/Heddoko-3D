// /**
// * @file RecordingLoader.cs
// * @brief Contains the RecordingLoader class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Localization;
using Assets.Scripts.Tests;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using UnityEngine.Events;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Class that loads a recording from a source path
    /// </summary>
    public class RecordingLoader
    {
        private Action<BodyFramesRecordingBase> mRecordingLoadedCallback;
        public FinishLoading FinishLoadingEvent;
        public StartLoading StartLoadingEvent;
        public void SetCallbackAction(Action<BodyFramesRecordingBase> vRecordingLoadedCallback)
        {
            mRecordingLoadedCallback = vRecordingLoadedCallback;
        }

        /// <summary>
        /// Loads a file from a relative path, then invokes the loaded recording callback
        /// </summary>
        /// <param name="vRecordingRelativePath"></param>
        /// <param name="vRecordingLoadedCallback">the callback after the recording has been loaded</param>
        public void LoadFile(string vRecordingRelativePath, Action<BodyFramesRecordingBase> vRecordingLoadedCallback)
        {
            mRecordingLoadedCallback = vRecordingLoadedCallback;
            LoadFile(vRecordingRelativePath);
        }

        /// <summary>
        /// Callback, on file selection. 
        /// </summary>
        /// <param name="vRecordingRelativePath"></param>
        private void LoadFile(string vRecordingRelativePath)
        {
            FileInfo vInfo = new FileInfo(vRecordingRelativePath);
            if (vInfo.Name.Equals("logindex.dat", StringComparison.CurrentCultureIgnoreCase))
            {
                string vTopLabel = LocalizationBinderContainer.GetString(KeyMessage.TopLabelLogindexOpeningErrMsg);
                string vContent = LocalizationBinderContainer.GetString(KeyMessage.LogindexOpeningErrMsg);
                UnityAction vOnYes = () => { };
                ModalPanel.SingleChoice(vTopLabel, vContent, vOnYes);
                return;
            }
            if (StartLoadingEvent != null)
            {
                StartLoadingEvent();
            }
            //Verify the file first, if it's a logindex.dat file, then throw an error

            ApplicationSettings.PreferedRecordingsFolder = vInfo.DirectoryName;
            BodyRecordingsMgr.Instance.ScanRecordings(UniFileBrowser.use.filePath);
            BodyRecordingsMgr.Instance.ReadRecordingFile(vRecordingRelativePath, BodyFramesRecordingCallback);
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

       
    }
}