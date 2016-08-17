// /**
// * @file SynchronizableRecordingListController.cs
// * @brief Contains the SynchronizableRecordingListController
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
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
        private List<RecordingListItem> vRecordingItems = new List<RecordingListItem>();
        public UserProfileModel Profile
        {
            get
            {
                if (mProfile == null)
                {
                    mProfile = UserSessionManager.UserProfile;
                }
                return mProfile;
            }
        }
        public int ItemNumbersPerPage = 25;
        private int mSkipMultiplier = 0;
        public void GetList()
        {
            //ping server: get list
            ListCollection<Asset> vRecords = Profile.Client.AssetsCollection(new AssetListRequest()
            {
                UserID = Profile.User.ID,//optional
                Take = ItemNumbersPerPage,
                Skip = mSkipMultiplier * ItemNumbersPerPage
            });
            //Skip number of items in order to paginate properly.
            mSkipMultiplier++;

            if (vRecords.Collection.Count > 0)
            {
                foreach (var vRecordedAsset in vRecords.Collection)
                {
                    if (!string.IsNullOrEmpty(vRecordedAsset.Name) && vRecordedAsset.Type == AssetType.Record)
                    {
                        RecordingListItem vItem = new RecordingListItem();
                        vItem.Name = vRecordedAsset.Name;
                     //   vItem.Location.RelativePath = vRecordedAsset.Url;
                        RecordingListItem.RecordingItemLocation vLoc = new RecordingListItem.RecordingItemLocation(vRecordedAsset.Url, RecordingListItem.LocationType.RemoteEndPoint);
                        vItem.Location = vLoc;
                        vRecordingItems.Add(vItem);
                    }
                }

                //it is still possible that the count == 0, because of null name check
                if (vRecords.Collection.Count > 0)
                {
                    RecordingListSyncView.LoadData(vRecordingItems);
                }
            }

        }


        public void ReleaseResources()
        {
            RecordingListSyncView.Clear();
        }

        void OnApplicationQuit()
        {

        }

        public void UploadRecording(string vRelativePath)
        {
            //verify the user passed in has a kit assigned to him
            Kit vKit = mProfile.User.Kit;
            if (vKit != null)
            {
                Asset asset = Profile.Client.Upload(new AssetRequest
                {
                    KitID = vKit.ID, //optional
                    Type = AssetType.Record //required
                },
                    @vRelativePath);
            }
            else
            {
                //Throw or handle exception when a user doesn't have a kit assigned to him and try to upload a recording.
                throw new NotImplementedException();

            }
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
                    RecordingListSyncView.LoadData(vRecordingItems);
                }
                else
                {
                    //start downloading
                    vStructure.DownloadLocation = vCachePath + Path.DirectorySeparatorChar + vItem.Name;
                    //=    vItem.RelativePath;
                    vStructure.Item = vItem;
                    vItem.Location.LocationType = RecordingListItem.LocationType.DownloadingAndUnavailable;
                    RecordingListSyncView.LoadData(vRecordingItems);
                    ThreadPool.QueueUserWorkItem(FetchData, vStructure);
                }
            }
        }

        public void PlayRecording(RecordingListItem vItem)
        {
            Debug.Log("hide item");
            SingleRecordingSelection.Instance.LoadFile(vItem.Location.RelativePath, RecordingPlayerView.PbControlPanel.NewRecordingSelected);
            
        }
        public void FetchData(object vCallbackStruct)
        {
            BaseModel vHedAsset = null;
            try
            {
                DataFetchingStructure vStructure = (DataFetchingStructure)vCallbackStruct;
                vHedAsset = Profile.Client.DownloadFile(vStructure.Item.Location.RelativePath, vStructure.DownloadLocation);
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => DownloadCompletedCallback(vHedAsset, ref vStructure.Item));
                Profile.Client.DownloadFile(vStructure.Item.Location.RelativePath, vStructure.DownloadLocation);
            }
            catch (Exception vE)
            {
                ErrrorDownloadHandler(vE);
            }
        }

        /// <summary>
        /// Handle's fetch data errors
        /// </summary>
        /// <param name="vE"></param>
        private void ErrrorDownloadHandler(Exception vE)
        {
            //check if response stream is null message
            if (vE.Message.Equals("Response stream is null"))
            {
                Debug.Log("handle this message");
            }
            Debug.Log(vE.Message + " " + vE.StackTrace);
        }

        /// <summary>
        /// When the download is complete, this callback function puts the item in a playable state.
        /// </summary>
        /// <param name="vHedAsset"></param>
        /// <param name="vItem"></param>
        private void DownloadCompletedCallback(BaseModel vHedAsset, ref RecordingListItem vItem)
        {
            Debug.Log("Download completed");
            vItem.Location.RelativePath = ApplicationSettings.CacheFolderPath + Path.DirectorySeparatorChar + vItem.Name;
            vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
            //reload the data
            RecordingListSyncView.LoadData(vRecordingItems);
        }
        /// <summary>
        ///setup uploading. 
        /// </summary>
        public void PrepareToUpload()
        {

        }

        private struct DataFetchingStructure
        {
            public string DownloadLocation;
            public RecordingListItem Item;
        }
    }
}