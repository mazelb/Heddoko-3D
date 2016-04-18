/** 
* @file ImportRecordingGridSelectionComponent.cs
* @brief Contains the ImportRecordingGridSelectionComponent class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using Assets.Scripts.UI.Tagging;
using UnityEngine;
using UnityEngine.UI;

using UIWidgets;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    /// <summary>
    /// resizable items specific to importing views
    /// </summary>
    [Serializable]
    public class ImportRecordingGridSelectionComponent : ListViewItem, IResizableItem
    {
        public MarkedForDeletion MarkedForDeletionItem;
        public Text MovementTitle;
        public Text CreatedAtDescription;
        public Text DurationDescription;
        private TaggingManager mTaggingManager;
        public TaggingContainer TagContainer;
        public ListView FloatingList;


        /// <summary>
        /// The gameobject that can be resized
        /// </summary>
        public GameObject[] ObjectsToResize
        {
            get { return new GameObject[] { MovementTitle.gameObject, CreatedAtDescription.gameObject, DurationDescription.gameObject, TagContainer.gameObject }; }
        }

        public TaggingManager TaggingManager
        {
            get { return mTaggingManager; }
            set
            {
                TagContainer.TaggingManager = value;
                mTaggingManager = value;
            }
        }

        /// <summary>
        /// Sets data from the given 
        /// </summary> 
        /// <param name="vDescriptorItem"></param>
        public void SetData(RecordingItemDescriptor vDescriptorItem)
        {
            MovementTitle.text = vDescriptorItem.MovementTitle;
            CreatedAtDescription.text = vDescriptorItem.CreatedAtDescription;
            DurationDescription.text = vDescriptorItem.RecordingDurationToString;
            if (MarkedForDeletionItem.Item == null)
            {
                MarkedForDeletionItem.Item = vDescriptorItem;
            }
            MarkedForDeletionItem.Item = vDescriptorItem;
            MarkedForDeletionItem.IsMarkedForDeletion = vDescriptorItem.IsMarkedForDeletion;
            TagContainer.FloatListView = FloatingList;
            //update the tagset
            if (vDescriptorItem.TagSet.Count > 0)
            {
                foreach (var vItem in vDescriptorItem.TagSet)
                {
                    TagContainer.AddTag(vItem);
                }
            }
            else
            {
                TagContainer.RemoveAllTags();
            }

        }

        /// <summary>
        /// Onpointer click ignores right click events
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                return;
            }
            base.OnPointerClick(eventData);
        }

        public void AddTag(string vTagText)
        {
            //todo
        }


    }
}
