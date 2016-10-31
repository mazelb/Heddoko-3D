// /**
// * @file AssetUploader.cs
// * @brief Contains the AssetUploader class
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
    public class AssetUploader
    {
        public event UploadComplete UploadCompleteEvent;
        public event ErrorInUploading UploadErrorEvent;
        private UserProfileModel mProfile;

        public AssetUploader(UserProfileModel vProfile)
        {
            mProfile = vProfile;
        }

        /// <summary>
        /// Blocking operation: will upload a single recording and invoke an event on completion
        /// </summary>
        /// <param name="vItem"></param>
        public void UploadSingleItem(object vItem)
        {
            try
            {
                UploadableListItem vUploadableItem = (UploadableListItem)vItem;
                Asset vAsset = mProfile.Client.Upload(new AssetRequest()
                { 
                    Serial = vUploadableItem.BrainpackSerialNumber,
                    Type = vUploadableItem.AssetType
                }, vUploadableItem.RelativePath);
                if (vAsset.IsOk)
                {
                    if (UploadCompleteEvent != null)
                    {
                        UploadCompleteEvent(vUploadableItem);
                    }
                }
                else
                {
                    ErrorCollection vCollection =vAsset.Errors;
                    ErrorUploadEventArgs vObj = new ErrorUploadEventArgs()
                    {
                        Object = (UploadableListItem)vItem,
                        ExceptionArgs = null,
                        ErrorCollection =  vCollection
                    };
                    InvokeErrorEvent(vObj);
                }
            }
            catch (Exception vE)
            {
                if (UploadErrorEvent != null)
                {
                    string vMessage = vE.Message;
                    ErrorUploadEventArgs vObj = new ErrorUploadEventArgs()
                    {
                        Object = (UploadableListItem)vItem,
                        ExceptionArgs = vE
                    };
                    UploadErrorEvent(vObj);
                }
            }
        }

        /// <summary>
        /// Invokes error events
        /// </summary>
        /// <param name="vArgs"></param>
        void InvokeErrorEvent(ErrorUploadEventArgs vArgs )
        {
            if (UploadErrorEvent != null)
            {
                UploadErrorEvent(vArgs);
            }
        }
    }

    /// <summary>
    /// A structure for upload errors, holding the object and exeception
    /// </summary>
    public class ErrorUploadEventArgs
    {
        public UploadableListItem Object;
        public ErrorCollection ErrorCollection;
        public Exception ExceptionArgs;
    }
}