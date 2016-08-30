using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using HeddokoSDK;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void RecordingListUpdated(List<RecordingListItem> vList);

    /// <summary>
    /// Fetches a list of recordings from the a HeddokoClient
    /// </summary>
    public class RecordingListFetcher
    {
        private List<RecordingListItem> mRecordingItems = new List<RecordingListItem>();
        private UserProfileModel mProfile;
        public int ItemNumbersPerPage = 50;
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
        public List<RecordingListItem> RecordingItems
        {
            get { return mRecordingItems; }
            set { mRecordingItems = value; }
        }

        void WorkingFunction()
        {
            while (mIsWorking)
            {
                try
                {
                    UpdateFetchedList();
                }
                catch (Exception vE)
                {
                     
                }
                Thread.Sleep(mTimer);
            }
        }
        /// <summary>
        /// Updates the fetched list
        /// </summary>
        public void UpdateFetchedList()
        {
            ListCollection<Asset> vRecords = mManager.UserProfile.Client.AssetsCollection(new AssetListRequest()
            {
                UserID = mManager.UserProfile.User.ID,//optional
                Take = ItemNumbersPerPage,
                Skip = mSkipMultiplier * ItemNumbersPerPage
            });
            if (vRecords != null)
            {
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
                            mRecordingItems.Add(vItem);
                        }
                    }
                }
                if (RecordingListUpdatedHandler != null && mIsWorking)
                {
                    RecordingListUpdatedHandler(mRecordingItems);
                }
            }
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
            catch (Exception )
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
            mWorkerThread.Start();
            
        }
    }
}