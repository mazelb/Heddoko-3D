/** 
* @file BodySelectedInfo.cs
* @brief Contains the BodySelectedInfo class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Communication;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI.MainScene.Model
{
    /// <summary>
    /// Represents which recording a user has chosen
    /// </summary>
    public class BodySelectedInfo : MonoBehaviour
    {
        private static BodySelectedInfo mInstance;

        public delegate void BodyRecordingChanged();

        public event BodyRecordingChanged BodyRecordingChangedEvent;

        [SerializeField]
        private SuitSocketClientSettings mSocketClientSettings; 

        public string mSelectedRecordingPath;
        public string mSelectedBody;
        public int TotalRecordingsAvailable;
        private Dictionary<string, BodyFramesRecordingBase> mBodyRecordingMap = new Dictionary<string, BodyFramesRecordingBase>(1);

        /// <summary>
        /// Singleton instance of the class
        /// </summary>
        public static BodySelectedInfo Instance
        {
            get
            {
                if (mInstance == null)
                {

                    GameObject vGo = GameObject.FindGameObjectWithTag("Controllers");
                    if (vGo != null)
                    {
                        mInstance = vGo.GetComponent<BodySelectedInfo>();
                    }
                    if (mInstance == null)
                    {
                        mInstance = FindObjectOfType<BodySelectedInfo>();
                    }
                    if (mInstance == null)
                    {
                        GameObject vNewGo = new GameObject("BodySelectedInfo");
                        mInstance = vNewGo.AddComponent<BodySelectedInfo>();
                    }
                    DontDestroyOnLoad(mInstance.gameObject);
                }

                return mInstance;

            }
        }

        /// <summary>
        /// Updates the number of recordings 
        /// </summary>
        public void UpdateNumberOfRecordings()
        {
            TotalRecordingsAvailable = BodyRecordingsMgr.Instance.ScanRecordings("/Resources/Recordings");
        }

        /// <summary>
        /// Returns the current selected Recording based on the selected recording from the main menu
        /// </summary>
        public BodyFramesRecordingBase CurrentSelectedRecording
        {
            get
            {
                if (!string.IsNullOrEmpty(mSelectedRecordingPath)  && mBodyRecordingMap.ContainsKey(mSelectedRecordingPath))
                {
                    return mBodyRecordingMap[mSelectedRecordingPath];
                }
                else if(!string.IsNullOrEmpty(mSelectedRecordingPath))
                {
                    UpdateCurrentBodyFrameRecording();
                    return mBodyRecordingMap[mSelectedRecordingPath];
                }
                return null;
            }
        }

       
        /// <summary>
        /// On awake, initialize the instance
        /// </summary>
        void Awake()
        {
            Instance.Init();
        }

        /// <summary>
        /// On application quit, removed listeners 
        /// </summary>
        void OnApplicationQuit()
        {
            BodyRecordingChangedEvent = null;
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        void Init()
        {
            
        }
         
        /// <summary>
        /// Updates the current recording
        /// </summary>
        public void UpdateSelectedRecording(int vRecordingIndex)
        {
            if (TotalRecordingsAvailable == 0)
            {
                TotalRecordingsAvailable = BodyRecordingsMgr.Instance.FilePaths.Length;
            }
            if (vRecordingIndex >= 0 && vRecordingIndex < TotalRecordingsAvailable)
            {
                mSelectedRecordingPath = BodyRecordingsMgr.Instance.FilePaths[vRecordingIndex];
                if (BodyRecordingChangedEvent != null)
                {
                    BodyRecordingChangedEvent();
                } 
            }

        }

        /// <summary>
        ///helper method update the current bodyframes recording
        /// </summary>
        public void UpdateCurrentBodyFrameRecording()
        {
            if (!mBodyRecordingMap.ContainsKey(mSelectedRecordingPath))
            {
                BodyRecordingsMgr.Instance.ReadRecordingFile(mSelectedRecordingPath);
                //the latest item to be placed in the list is now the current body frame recording
                BodyFramesRecordingBase vCurrBFR = BodyRecordingsMgr.Instance.Recordings[BodyRecordingsMgr.Instance.Recordings.Count - 1];
                mBodyRecordingMap.Add(mSelectedRecordingPath,vCurrBFR); 
            }
        }

        /// <summary>
        /// Uodates the selected recording based on the subpath passed in
        /// </summary>
        /// <param name="vSubPath"></param>
        public void UpdateSelectedRecording(string vSubPath)
        {
            //get the index of the passed in activityTypeSubPath
            string[] vPaths =  BodyRecordingsMgr.Instance.FilePaths;
            int vIndex=-1;
            for (int i = 0; i < vPaths.Length; i++)
            {
                if (vPaths[i].Contains(vSubPath))
                {
                    vIndex = i;
                    break;
                }
            }
            UpdateSelectedRecording(vIndex);

        }
    }
}
