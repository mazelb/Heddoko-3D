using System;
using System.Collections.Generic;
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
        public int ItemNumbersPerPage = 25;
        private int mSkipMultiplier = 0;
        public RecordingListUpdated RecordingListUpdatedHandler;
        private IUserProfileManager mManager;
        public RecordingListFetcher(IUserProfileManager vManager)
        {
            mManager = vManager;
        }
        public List<RecordingListItem> RecordingItems
        {
            get { return mRecordingItems; }
            set { mRecordingItems = value; }
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
            if (RecordingListUpdatedHandler != null)
            {
                RecordingListUpdatedHandler(mRecordingItems);
            }
        }

        public void UploadRecording(string vRelativePath, SynchronizableRecordingListController synchronizableRecordingListController)
        {
            //verify the user passed in has a kit assigned to him
            Kit vKit = mProfile.User.Kit;
            if (vKit != null)
            {
                Asset asset = mProfile.Client.Upload(new AssetRequest
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
    }
}