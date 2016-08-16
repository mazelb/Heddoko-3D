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
        private UserProfileModel mProfile;

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
            List<RecordingListItem> vItemDescriptors = new List<RecordingListItem>();
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
                        vItem.RelativePath = vRecordedAsset.Url;
                        RecordingListItem.RecordingItemLocation vLoc = new RecordingListItem.RecordingItemLocation(vItem.Name, RecordingListItem.LocationType.RemoteEndPoint);
                        vItem.Location = vLoc;
                        vItemDescriptors.Add(vItem);
                    }
                }

                //it is still possible that the count == 0, because of null name check
                if (vRecords.Collection.Count > 0)
                {
                    RecordingListSyncView.LoadData(vItemDescriptors);
                }
            }

            //get files in cache directory.
            // BodyRecordingsMgr.Instance.ScanRecordings(ApplicationSettings.CacheFolderPath);
            // string[] vFilePaths = BodyRecordingsMgr.Instance.FilePaths;
            //FileInfo[] vFilesInfos= new FileInfo[vFilePaths.Length];
            // for (int i = 0; i < vFilePaths.Length; i ++)
            // {
            //     vFilesInfos[i] =  new FileInfo(vFilePaths[i]);
            // }

            // //cross compare records vs vFilepaths. Only Display recordings that belong to the passed in user.
            // var vFilteredLocalPaths = vFilesInfos.Where(x => vRecords.Collection.Any(y => y.Name.Equals(x.Name))).ToList();
            // foreach (var vFilteredLocalPath in vFilteredLocalPaths)
            // {
            //     RecordingListItem vItem = new RecordingListItem();
            //     vItem.Name = vFilteredLocalPath.Name;
            //     RecordingListItem.RecordingItemLocation vLoc = new RecordingListItem.RecordingItemLocation(vFilteredLocalPath.FullName, RecordingListItem.LocationType.CachedLocal);
            //     vItem.Location = vLoc;
            //     vItemDescriptors.Add(vItem);
            // }
            // //filter out cached recordings from new recordings
            // var vFilteredRemoteRecordings = vRecords.Collection.Where(x => vItemDescriptors.Any(y => y.Name.Equals(x.Name))).ToList();
            // foreach (var vFilteredRemoteRecording in vFilteredRemoteRecordings)
            // {
            //     vItemDescriptors.Add(vi);
            // }

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
        /// Play a recording from its id. 
        /// </summary>
        /// <param name="vId"></param>
        public void PlayRecording(int vId)
        {
            //Implement a state machine for playing. 
            throw new NotImplementedException();
        }


        public void PlayRecording(ref RecordingListItem vItem)
        {

            //Check if the item exists in the cache already. else proceed to download it. 
            if (vItem.Location.LocationType == RecordingListItem.LocationType.CachedLocal)
            {
                //todo: load from this location
            }
            else
            {
                string vCachePath = ApplicationSettings.CacheFolderPath;
                DirectoryInfo vInfo = new DirectoryInfo(vCachePath);
                var vFilesInfo = vInfo.GetFiles();
                RecordingListItem vRecItem = vItem;
                var vFoundItem = vFilesInfo.FirstOrDefault(x => x.Name.Equals(vRecItem.Name));
                if (vFoundItem != null)
                {
                    //change the location type
                    vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
                    vItem.RelativePath = vFoundItem.FullName;
                    //todo: load and play from this location
                }
                else
                {
                    DataFetchingStructure vStructure = new DataFetchingStructure();
                    vStructure.DownloadPath = vItem.RelativePath;
                    vStructure.Item = vItem;
                    //set the relative path to the cached path on completion
                    vItem.RelativePath = vCachePath + Path.DirectorySeparatorChar + vItem.Name; 
                    //change the location type
                    vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
                   
                   //todo: load and play from this location
                   var vItem2 = ThreadPool.QueueUserWorkItem(FetchData, vStructure);
                }


            }
        }


        public void FetchData(object vCallbackStruct)
        {
            try
            {
                DataFetchingStructure vStructure = (DataFetchingStructure) vCallbackStruct;
                BaseModel vHedAsset = Profile.Client.DownloadFile(vStructure.Item.RelativePath, vStructure.DownloadPath);
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => DownloadCompleted(vHedAsset));
            }
            catch (Exception vE)
            {
                Debug.Log("Donwload not completed");

            }
        
        }

        private void DownloadCompleted(HeddokoSDK.Models.BaseModel vHedAsset)
        {
            Debug.Log("Donwload completed");
        }
        /// <summary>
        /// Start the syncing process. 
        /// </summary>
        public void Sync()
        {

        }

        private struct DataFetchingStructure
        {
            public string DownloadPath;
            public RecordingListItem Item;
        }
    }
}