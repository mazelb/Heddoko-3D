using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.Utils;
using HeddokoSDK.Models;
using HeddokoSDK.Models.Requests;

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void RecordingListUpdated(List<RecordingListItem> vList);

    /// <summary>
    /// Fetches a list of recordings from the a HeddokoClient
    /// </summary>
    public class RecordingListFetcher
    {
        private UserProfileModel mProfile;
        public int ItemNumbersPerPage = 500;

        public RecordingListUpdated RecordingListUpdatedHandler;
        private IUserProfileManager mManager;
        private Thread mWorkerThread;
        private bool mIsWorking;
        private int mTimer = 10000;
        private int mRemainderRecordingsLeft;
        private object mIsWorkingLock = new object();
        /// <summary>
        /// Skips the number of items to download
        /// </summary>
        private int mSkipMultiplier { get; set; }
        /// <summary>
        /// Flag to collect default records
        /// </summary>
        private bool mAddDefaultRecord { get; set; }
        /// <summary>
        /// Create a new instance with a user profile manager object
        /// </summary>
        /// <param name="vManager"></param>
        public RecordingListFetcher(IUserProfileManager vManager)
        {
            mManager = vManager;
        }

        /// <summary>
        /// Flag to determine if the working function is supposed to be working or not. 
        /// </summary>
        private bool IsWorking
        {
            get
            {
                lock (mIsWorkingLock)
                {
                    return mIsWorking;
                }
            }
            set
            {
                lock (mIsWorkingLock)
                {
                    mIsWorking = value;
                }
            }
        }

        void WorkingFunction()
        {
            while (IsWorking)
            {
                try
                {
                    RequestFetchList();
                }
                catch (Exception vE)
                {

                }
                Thread.Sleep(mTimer);
            }
        }


        /// <summary>
        /// Helper function with thread pools
        /// </summary>
        /// <param name="vItem"></param>
        public void RequestFetchList(object vItem)
        {
            RequestFetchList();
        }
        /// <summary>
        /// Sends a request to fetch list
        /// </summary>
        public void RequestFetchList()
        {
            int vCount = 0;
            if (mManager.UserProfile.User.RoleType == UserRoleType.Analyst)
            {
                var vList = mManager.UserProfile.UserList;

                for (int i = 0; i < vList.TotalCount && mIsWorking; i++)
                {
                    var vUser = vList.Collection[i];
                    int vTempCount = UpdateList(vUser);
                    if (vTempCount != 0)
                    {
                        vCount = vTempCount;
                    }
                }
            }

            else
            {
                vCount = UpdateList(mManager.UserProfile.User);
            }
            if (vCount > 0)
            {
                mSkipMultiplier++;
            }
        }


         
        /// <summary>
        /// Fetches a list and triggers an update
        /// </summary>
        /// <param name="vUser">the users whos recording list needs to be requested</param>
        /// <returns>the total number of recordings that the fetched list retrieved</returns>
        private int UpdateList(User vUser)
        {
            int vTotalCount = 0;

            List<RecordingListItem> vRecordingItems = new List<RecordingListItem>();
            ListCollection<Record> vRecords = null;
            //check if the list is new. Request default recordings and add it to the list.  
            if (mAddDefaultRecord)
            {
                vRecordingItems = RequestDefaultRecordings();
                mAddDefaultRecord = false;
            }
            else
            {
                vRecordingItems = new List<RecordingListItem>();
            }
            if (mManager.UserProfile.User.RoleType == UserRoleType.LicenseUniversal)
            {
                var request = new RecordListRequest
                {
                    Take = ItemNumbersPerPage,
                    Skip = mSkipMultiplier * ItemNumbersPerPage
                };

                vRecords = mManager.UserProfile.Client.GetAllRecords(request);
            }
            else
            {
                //the old version of the sdk had a different request type of AssetCollection. This function collects older types of records
                AddOldRecordAssetsTypes(ref vRecordingItems, vUser);
                try
                {
                    vRecords = mManager.UserProfile.Client.RecordsCollection(new UserRecordListRequest()
                    {
                        UserID = vUser.ID,
                        Take = ItemNumbersPerPage,
                        Skip = mSkipMultiplier * ItemNumbersPerPage
                    });
                }
                catch (Exception ve)
                {
                    OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
                    {
                        UnityEngine.Debug.Log("exception in list fetching "+ve);
                    });
                }
            }
            if (vRecords != null)
            {
                if (vRecords.Collection.Count > 0)
                {
                    var vRecordCollection = vRecords.Collection;
                    for (int vI = 0; vI < vRecords.TotalCount; vI++)
                    {
                        var vRecord = vRecordCollection[vI];
                        for (int vJ = 0; vJ < vRecord.Assets.Count; vJ++)
                        {
                            var vRecordedAsset = vRecord.Assets[vJ];
                            var vIsRecord = vRecordedAsset.Type == AssetType.Record || vRecordedAsset.Type == AssetType.RawFrameData;
                            if (!string.IsNullOrEmpty(vRecordedAsset.Name) &&   vIsRecord)
                            {
                                RecordingListItem vItem = new RecordingListItem();
                                vItem.Name = vRecordedAsset.Name;
                                vItem.AssetType = AssetType.Record;
                                RecordingListItem.RecordingItemLocation vLoc =
                                    new RecordingListItem.RecordingItemLocation(vRecordedAsset.Url,
                                        RecordingListItem.LocationType.RemoteEndPoint);
                                vItem.Location = vLoc;
                                vItem.User = vUser;
                                vRecordingItems.Add(vItem);
                            }
                        }
                    }
                }
            }
            
            if (RecordingListUpdatedHandler != null && mIsWorking)
            {
                RecordingListUpdatedHandler(vRecordingItems);
            }
            vTotalCount = vRecordingItems.Count;

            return vTotalCount;
        }

        private void AddOldRecordAssetsTypes(ref List<RecordingListItem> vItems, User vUser)
        {
            ListCollection<Asset> vRecords = mManager.UserProfile.Client.AssetsCollection(new AssetListRequest()
            {
                UserID = vUser.ID,
                Take = ItemNumbersPerPage,
                Skip = mSkipMultiplier * ItemNumbersPerPage
            });
            if (vRecords != null)
            {
                if (vRecords.Collection.Count > 0)
                {
                    foreach (var vRecordedAsset in vRecords.Collection)
                    {
                        if (!string.IsNullOrEmpty(vRecordedAsset.Name) && vRecordedAsset.Type == AssetType.Record)
                        {
                            RecordingListItem vItem = new RecordingListItem();
                            vItem.Name = vRecordedAsset.Name;
                            vItem.AssetType = AssetType.Record;
                            RecordingListItem.RecordingItemLocation vLoc =
                                new RecordingListItem.RecordingItemLocation(vRecordedAsset.Url,
                                    RecordingListItem.LocationType.RemoteEndPoint);
                            vItem.Location = vLoc;
                            vItem.User = vUser;
                            vItems.Add(vItem);
                        }
                    }
                }
            }

        }
        /// <summary>
        /// requests default recordings
        /// </summary> 
        /// <returns>a list of default recordings</returns>>
        internal List<RecordingListItem> RequestDefaultRecordings()
        {
            List<RecordingListItem> vRecordingItems = null;
            var vRecords = mManager.UserProfile.Client.GetDefaultRecords(new ListRequest { Take = 100, Skip = 0 });
            if (vRecords != null)
            {
                vRecordingItems = new List<RecordingListItem>();
                if (vRecords.Collection.Count > 0)
                {
                    var vRecordCollection = vRecords.Collection;
                    for (int vI = 0; vI < vRecords.TotalCount; vI++)
                    {
                        var vRecord = vRecordCollection[vI];
                        for (int vJ = 0; vJ < vRecord.Assets.Count; vJ++)
                        {
                            var vRecordedAsset = vRecord.Assets[vJ];
                            if (!string.IsNullOrEmpty(vRecordedAsset.Name) && vRecordedAsset.Type == (AssetType.RawFrameData | AssetType.DefaultRecords))
                            {
                                RecordingListItem vItem = new RecordingListItem();
                                vItem.Name = vRecordedAsset.Name;
                                vItem.AssetType = AssetType.Record;
                                RecordingListItem.RecordingItemLocation vLoc =
                                    new RecordingListItem.RecordingItemLocation(vRecordedAsset.Url,
                                        RecordingListItem.LocationType.RemoteEndPoint);
                                vItem.Location = vLoc;
                                vRecordingItems.Add(vItem);
                            }
                        }
                    }
                }
            }
            return vRecordingItems;
        }
        /// <summary>
        /// stops the work
        /// </summary>
        public void Stop()
        {
            IsWorking = false;
            try
            {
                mWorkerThread.Abort();
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// Start background worker
        /// </summary>
        public void Start()
        {
            IsWorking = true;
            mWorkerThread = new Thread(WorkingFunction);
            mWorkerThread.IsBackground = true;
            mAddDefaultRecord = true;
            mWorkerThread.Start();
        }

        private void FetchInitialList()
        {

        }
        /// <summary>
        /// Clears the multiplier, which allows the collection of records from the beginning.
        /// </summary>
        public void Clear()
        {
            mAddDefaultRecord = true;
            mSkipMultiplier = 0;
        }
    }
}
