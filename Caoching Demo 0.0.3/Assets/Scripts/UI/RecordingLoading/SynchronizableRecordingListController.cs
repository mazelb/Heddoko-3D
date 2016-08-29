// /**
// * @file SynchronizableRecordingListController.cs
// * @brief Contains the SynchronizableRecordingListController
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System; 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.Tests;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.RecordingLoading.View;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using HeddokoSDK.Models;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading
{

    /// <summary>
    /// A controller that fetches recordings from multiple data sources, and allows for the manipulation of this data. 
    /// </summary>
    public class SynchronizableRecordingListController : MonoBehaviour
    {

        public RecordingListSyncView RecordingListSyncView;
        public RecordingPlayerView RecordingPlayerView;
        private UserProfileModel mProfile;
        private List<RecordingListItem> mRecordingItems = new List<RecordingListItem>();
        private RecordingListFetcher mListFetcher;
        private HeddokoDownloadFetcher mRecordingFetcher;
        public int ItemNumbersPerPage = 25;
        private int mSkipMultiplier = 0;

        void Awake()
        {
            mListFetcher = new RecordingListFetcher(UserSessionManager.Instance);
            mListFetcher.RecordingListUpdatedHandler += LoadDataThroughUnityThread;
            mRecordingFetcher = new HeddokoDownloadFetcher(UserSessionManager.Instance);
            mRecordingFetcher.ErrorDownloadingExceptionHandler += ExceptionHandler;
        }
        
        /// <summary>
        /// Update a list by ensuring it is handled by a unity thread
        /// </summary>
        /// <param name="vList"></param>
        private void LoadDataThroughUnityThread(List<RecordingListItem> vList)
        {
            OutterThreadToUnityThreadIntermediary.EnqueueOverwrittableActionInUnity("LoadDownloadedRecordingsList", () => RecordingListSyncView.LoadData(vList));
        }
        /// <summary>
        /// Updates the recording list of items that belong to a user
        /// </summary>
        public void UpdateList()
        {
            mListFetcher.UpdateFetchedList();
            if (mListFetcher.RecordingItems.Count > 0)
            {
                mRecordingItems = mListFetcher.RecordingItems;
                RecordingListSyncView.LoadData(mRecordingItems);
            }

        }


        public void ReleaseResources()
        {
            RecordingListSyncView.Clear();
        }

        void OnApplicationQuit()
        {
             
        }


        /// <summary>
        /// Play a recording from the  given RecordingListItem
        /// </summary>
        /// <param name="vItem">the tiem to try to play</param>
        public void ProcessRecording(ref RecordingListItem vItem)
        {

            //Check if the item exists in the cache already. else proceed to download it. 
            if (vItem.Location.LocationType == RecordingListItem.LocationType.CachedLocal)
            {
                //todo: load from this location
            }
            else
            {
                //start downloading file. since this responsiblity will be delegated to a seperate thread, 
                //wait until completed.

                string vCachePath = ApplicationSettings.CacheFolderPath;
                DirectoryInfo vInfo = new DirectoryInfo(vCachePath);
                var vFilesInfo = vInfo.GetFiles();
                RecordingListItem vRecItem = vItem;
                var vFoundItem = vFilesInfo.FirstOrDefault(x => x.Name.Equals(vRecItem.Name));
                DataFetchingStructure vStructure = new DataFetchingStructure();
                //a cached item has been found
                if (vFoundItem != null)
                {
                    //change the location type
                    vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
                    vItem.Location.RelativePath = vFoundItem.FullName;
                    RecordingListSyncView.LoadData(mRecordingItems);
                }
                else
                {
                    //start downloading
                    vStructure.DownloadLocation = vCachePath + Path.DirectorySeparatorChar + vItem.Name;
                    //=    vItem.RelativePath;
                    vStructure.Item = vItem;
                    vItem.Location.LocationType = RecordingListItem.LocationType.DownloadingAndUnavailable;
                    RecordingListSyncView.LoadData(mRecordingItems);
                    mRecordingFetcher.DownloadCompletedHandler += DownloadCompletedCallback;
                    ThreadPool.QueueUserWorkItem(mRecordingFetcher.FetchData, vStructure);

                }
            }
        }
 
        public void PlayRecording(RecordingListItem vItem)
        {
            Debug.Log("hide item");
            SingleRecordingSelection.Instance.LoadFile(vItem.Location.RelativePath, RecordingPlayerView.PbControlPanel.NewRecordingSelected);

        }


        /// <summary>
        /// When the download is complete, this callback function puts the item in a playable state.
        /// </summary>
        /// <param name="vHedAsset"></param>
        /// <param name="vItem"></param>
        private void DownloadCompletedCallback(BaseModel vHedAsset, ref RecordingListItem vItem)
        {
            RecordingListItem vNonRefItem = vItem;
            Action vAction = () =>UpdateList(vHedAsset, ref vNonRefItem);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);
        }

        private void UpdateList(BaseModel vHedAsset, ref RecordingListItem vItem)
        {

            Debug.Log("Download completed");
            vItem.Location.RelativePath = ApplicationSettings.CacheFolderPath + Path.DirectorySeparatorChar + vItem.Name;
            vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
            //reload the data
            RecordingListSyncView.LoadData(mRecordingItems);
        }
        /// <summary>
        ///setup uploading. 
        /// </summary>
        public void PrepareToUpload()
        {

        }

        private void ExceptionHandler(Exception vE)
        {
            Debug.Log(vE.Message);
        }
    }
}