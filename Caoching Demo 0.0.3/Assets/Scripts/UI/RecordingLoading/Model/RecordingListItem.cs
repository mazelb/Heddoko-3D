// /**
// * @file RecordingListItem.cs
// * @brief Contains the RecordingListItem class 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;

namespace Assets.Scripts.UI.RecordingLoading.Model
{
    /// <summary>
    ///  a model that represents an item who's component is marked as either cached, or downloadable
    /// </summary>
    public class RecordingListItem
    {
        /// <summary>
        /// marked as nullable.
        /// </summary>
        private DateTime? mUploadDate;
        /// <summary>
        /// Getter/Setter property
        /// </summary>
        public string Name { get; set; }
 

        /// <summary>
        /// Public getter, internal setter property: the location of the item
        /// </summary>
        public RecordingItemLocation Location;

        /// <summary>
        /// Public getter, internal setter property: last time the item was played back
        /// </summary>
        public DateTime LastPlayedDate { get; internal set; }

        /// <summary>
        /// Public getter, internal setter property: creation date of the item
        /// </summary>
        public DateTime CreationDate { get; internal set; }


        /// <summary>
        /// Public getter, internal setter property: Uploaded date of the item
        /// </summary>
        public DateTime? UploadDate
        {
            get
            {
                return mUploadDate;
            }
            internal set
            {
                mUploadDate = value;
            }
        }

        /// <summary>
        /// Denotes the relative location of the RecordingItem
        /// </summary>
        public struct RecordingItemLocation
        {
            private string mRelativePath;
            private LocationType mType;
            /// <summary>
            /// Public getter, internal setter property: the relative path of the recording item
            /// </summary>
            public string RelativePath {
                get { return mRelativePath; }
                 set { mRelativePath = value; }
            }

            /// <summary>
            /// Public getter, internal setter property: the location type of the recording item
            /// </summary>
            public LocationType LocationType
            {
                get {  return mType; }
                set { mType = value; }
            }
            public RecordingItemLocation(string vRelativePath, LocationType vLocationType)
            {
                mRelativePath = vRelativePath;
                mType = vLocationType;
            }
        }

        public enum LocationType
        {
            CachedLocal,
            OnBrainpack,
            DownloadingAndUnavailable,
            RemoteEndPoint,
        }
    }



}