using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.UI.RecordingLoading.Model;
using HeddokoSDK.Models;
 
namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void UploadComplete(UploadableListItem vItem);
    public class RecordingsUploader
    {
        public UploadComplete UploadCompleteHandler;
        private UserProfileModel mProfile;

        public RecordingsUploader(UserProfileModel vProfile)
        {
            mProfile = vProfile;
        }

      

        /// <summary>
        /// Blocking operation: will upload a single recording and invoke an event on completion
        /// </summary>
        /// <param name="vItem"></param>
        public void UploadSingleRecording(object vItem)
        {
            UploadableListItem vUploadableItem = (UploadableListItem) vItem;
            Asset vAsset = mProfile.Client.Upload(new AssetRequest()
            {
                KitID = mProfile.User.Kit.ID,
                Type = AssetType.Record
            }, vUploadableItem.RelativePath);
            if (UploadCompleteHandler != null)
            {
                UploadCompleteHandler(vUploadableItem);
            }
            /* //verify the user passed in has a kit assigned to him
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

             }*/
        }
    }
}