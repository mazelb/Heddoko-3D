// /**
// * @file UploadableListItem.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading.Model
{
    public class UploadableListItem
    {
        public string RelativePath;
        public bool IsNew;
        private string mBrainpackSerialNumber = "";
        public AssetType AssetType;

        /// <summary>
        /// File name of the item
        /// </summary>
        public string FileName { get; set; }

        public string BrainpackSerialNumber
        {
            get
            {
                if (string.IsNullOrEmpty(mBrainpackSerialNumber) && FileName != null)
                {
                    int vIndex = FileName.IndexOf("_",StringComparison.InvariantCulture);
                    var vSubstring = FileName.Substring(0, vIndex );
                    mBrainpackSerialNumber = vSubstring;
                }
                return mBrainpackSerialNumber;
            }
            set { mBrainpackSerialNumber = value; }
        }
    }
}