// /**
// * @file RecordingUploader.cs
// * @brief Contains the RecordingUploader class
// * @author Mohammed Haider( mohammed@heddoko.com) 
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */


using System; 
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.UI.RecordingLoading.Model;
using HeddokoSDK.Models;
 
namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void UploadComplete(UploadableListItem vItem);

    public delegate void ErrorInUploading(ErrorUploadEventArgs vArgs);
    /// <summary>
    /// A Single recording uploader 
    /// </summary>
    public class RecordingUploader
    {
        public event UploadComplete UploadCompleteEvent;
        public event ErrorInUploading UploadErrorEvent;
        private UserProfileModel mProfile;

        public RecordingUploader(UserProfileModel vProfile)
        {
            mProfile = vProfile;
        }

        /// <summary>
        /// Blocking operation: will upload a single recording and invoke an event on completion
        /// </summary>
        /// <param name="vItem"></param>
        public void UploadSingleRecording(object vItem)
        {
            try
            {
                UploadableListItem vUploadableItem = (UploadableListItem) vItem;
                Asset vAsset = mProfile.Client.Upload(new AssetRequest()
                {
                    KitID = mProfile.User.Kit.ID,
                    Type = AssetType.Record
                }, vUploadableItem.RelativePath);
                if (UploadCompleteEvent != null)
                {
                    UploadCompleteEvent(vUploadableItem);
                }
            }
            catch (Exception vE)
            {
                if (UploadErrorEvent != null)
                {
                    ErrorUploadEventArgs vObj = new ErrorUploadEventArgs()
                    {
                        Object = vItem,
                        ExceptionArgs = vE
                    };
                    UploadErrorEvent(vObj);
                }
            }
         }
    }

    /// <summary>
    /// A structure for upload errors, holding the object and exeception
    /// </summary>
    public class ErrorUploadEventArgs
    {
        public object Object;
        public Exception ExceptionArgs;
    }
}