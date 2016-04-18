/** 
* @file RecordingItemDescriptor.cs
* @brief Contains the RecordingItemDescriptor class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.UI.Tagging;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors
{
    /// <summary>
    /// Describes an item that is held by the import view selectable grid list
    /// </summary>
    [Serializable]
    public class RecordingItemDescriptor
    {
        public const int MaxNumberOfTags = 5;
        [System.NonSerialized] public string RecordingUuid;
        
        [SerializeField]
        public string MovementTitle;
        [SerializeField]
        public string CreatedAtDescription;

        [SerializeField]
        public HashSet<string> TagSet = new HashSet<string>();
        [SerializeField]
        private DateTime mCreatedAtTime;

        [SerializeField]
        public int RecordingDuration;

        public string RecordingDurationToString
        {
            get
            {
                string vTime = RecordingProgressSubControl.FormatSecondsToTimeString(RecordingDuration);
                return vTime;
            }
        }

        public bool IsMarkedForDeletion;
        public DateTime CreatedAtTime
        {
            get
            {
                return mCreatedAtTime;
            }
            set
            {
                mCreatedAtTime = value;
                //check if the created time is today or yesterday
                bool vIsYesterday = DateTime.Today - mCreatedAtTime.Date == TimeSpan.FromDays(1);
                bool vIsToday = DateTime.Today - mCreatedAtTime.Date == TimeSpan.FromDays(0);
                string vPrefix = mCreatedAtTime.ToLongDateString();
                if (vIsToday)
                {
                    vPrefix = "Today";
                }
                else if (vIsYesterday)
                {
                    vPrefix = "Yesterday";
                }
                CreatedAtDescription = vPrefix + ", " +
                                       mCreatedAtTime.ToString("hh:mm tt",
                                           System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
        }

        public string FilePath { get; set; }


        /// <summary>
        /// adds a tag to the set
        /// </summary>
        /// <param name="vText"></param>
        public void AddTag(string vText)
        {
            //verify if capacity hasn't been reached... add it
            if (TagSet.Count < MaxNumberOfTags)
            {
                if (!TagSet.Contains(vText))
                {
                    TagSet.Add(vText);
                }
            }
        }
    }
}