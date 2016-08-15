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
        /// Relative path of the item
        /// </summary>
        public string RelativePath { get; internal set; }

        /// <summary>
        /// Public getter, internal setter property: the location of the item
        /// </summary>
        public   RecordingItemLocation  Location{ get; internal set; }

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
            /// <summary>
            /// Public getter, internal setter property: the relative path of the recording item
            /// </summary>
            public string RelativePath { get; internal set;}

            /// <summary>
            /// Public getter, internal setter property: the location type of the recording item
            /// </summary>
            public LocationType LocationType
            {
                get; internal set;
            }

        }

        public enum LocationType
        {
            CachedLocal,
            OnBrainpack,
            RemoteEndPoint,
        }
    }



}