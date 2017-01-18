// /**
// * @file AssetUploader.cs
// * @brief Contains the AssetUploader class
// * @author Mohammed Haider( mohammed@heddoko.com) 
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */


using System;
using System.Collections.Generic;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.UI.RecordingLoading.Model;
using HeddokoSDK.Models;
using HeddokoSDK.Models.Requests;

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
            string vItemName = "";

            try
            {
                UploadableListItem vUploadableItem = (UploadableListItem)vItem;
                int vKitId = 0;
               
                var vRequest = new RecordRequest
                {
                    //Label = "SN0001", //optional should be set Kit Label or Kit ID or Brainpack Label - that settings should be useful when you parse files from sd cards mostly only for Data analyst uploading
                    Label = vUploadableItem.BrainpackSerialNumber,
                    KitID = mProfile.GetKitIdFromBrainpackLabel(vUploadableItem.BrainpackSerialNumber),
                    Files = new List<AssetFile>
                    { 
                        new AssetFile { FileName =vUploadableItem.RelativePath ,Type =AssetType.RawFrameData }
                    }
                };
                Record vRecord= mProfile.Client.UploadRecord(vRequest);
                vItemName = vUploadableItem.RelativePath;
                if (vRecord.IsOk)
                {
                    if (UploadCompleteEvent != null)
                    {
                        UploadCompleteEvent(vUploadableItem);
                    }
                }
                else
                {
                    ErrorCollection vCollection = vRecord.Errors;
                    ErrorUploadEventArgs vObj = new ErrorUploadEventArgs()
                    {
                        Object = (UploadableListItem)vItem,
                        ExceptionArgs = null,
                        ErrorCollection = vCollection
                    };
                    InvokeErrorEvent(vObj);
                }
            }
            catch (Exception vE)
            {
                if (UploadErrorEvent != null)
                {
                    string vMessage = "This item failed to upload" + vItemName + " \t Reason:" + vE.Message;
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
        void InvokeErrorEvent(ErrorUploadEventArgs vArgs)
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