using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void RecordingListUpdated(List<RecordingListItem> vList);

    /// <summary>
    /// Fetches a list of recordings from the a HeddokoClient
    /// </summary>
    public class RecordingListFetcher
    {
        //  private List<RecordingListItem> mRecordingItems = new List<RecordingListItem>();
        private UserProfileModel mProfile;
        public int ItemNumbersPerPage = 500;
        private int mSkipMultiplier = 0;
        public RecordingListUpdated RecordingListUpdatedHandler;
        private IUserProfileManager mManager;
        private Thread mWorkerThread;
        private bool mIsWorking;
        private int mTimer = 10000;

        public RecordingListFetcher(IUserProfileManager vManager)
        {
            mManager = vManager;
        }

        void WorkingFunction()
        {
            while (mIsWorking)
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
            ListCollection<Asset> vRecords = mManager.UserProfile.Client.AssetsCollection(new AssetListRequest()
            {
                UserID = vUser.ID,
                Take = ItemNumbersPerPage,
                Skip = mSkipMultiplier * ItemNumbersPerPage
            });
            if (vRecords != null)
            {
                List<RecordingListItem> vRecordingItems = new List<RecordingListItem>();
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
                            vRecordingItems.Add(vItem);
                        }
                    }
                }
                if (RecordingListUpdatedHandler != null && mIsWorking)
                {
                    RecordingListUpdatedHandler(vRecordingItems);
                }
                vTotalCount = vRecordingItems.Count;
            }
            return vTotalCount;
        }

        /// <summary>
        /// stops the work
        /// </summary>
        public void Stop()
        {
            mIsWorking = false;
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
            mIsWorking = true;
            mWorkerThread = new Thread(WorkingFunction);
            mWorkerThread.IsBackground = true;
            mWorkerThread.Start();
        }

        private void FetchInitialList()
        {
            
        }

        public void Clear()
        {
            mSkipMultiplier = 0;
        }
    }
}