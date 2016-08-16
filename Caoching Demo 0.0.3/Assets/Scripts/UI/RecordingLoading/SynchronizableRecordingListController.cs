// /**
// * @file SynchronizableRecordingListController.cs
// * @brief Contains the SynchronizableRecordingListController
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.UI.RecordingLoading.View;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// A controller that fetches recordings from multiple data sources, and allows for the manipulation of this data. 
    /// </summary>
    public class SynchronizableRecordingListController
    {

        public RecordingListSyncView RecordingListSyncView;
        private HeddokoSDK.Models.User mUser;
        private HeddokoSDK.HeddokoClient mClient;

        public void GetList()
        {
            throw new NotImplementedException();
        }

        public void ReleaseResources()
        {
            throw new NotImplementedException();
        }

        void OnApplicationQuit()
        {

            throw new NotImplementedException();
        }

        public void UploadRecording(string vRelativePath)
        {
            //verify the user passed in has a kit assigned to him
            Kit vKit = mUser.Kit;
            if (vKit != null)
            {
                Asset asset = mClient.Upload(new AssetRequest
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


        /// <summary>
        /// Start the syncing process. 
        /// </summary>
        public void Sync()
        {
            
        }

    }
}