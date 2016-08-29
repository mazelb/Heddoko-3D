// /**
// * @file RecordingListSyncView.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.RecordingLoading.Model;
using UIWidgets;
using UnityEngine.Events;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    /// <summary>
    /// View: describes recording items that are cached/available for download
    /// </summary>
    public class RecordingListSyncView : ListViewCustom<SingleRecordingListItemComponent, RecordingListItem>
    {

        void Awake()
        {
            base.Start();
        }
        public UnityAction OnClickAction;
        /// <summary>
        /// Load data into the list
        /// </summary>
        /// <param name="vSingleRecItem"></param>
        public void LoadData(List<RecordingListItem> vSingleRecItemList)
        {
            DataSource.BeginUpdate();
            DataSource.Clear();
            for (int i = 0; i < vSingleRecItemList.Count; i++)
            {
                Add(vSingleRecItemList[i]);
            }
            DataSource.EndUpdate();
        }

        /// <summary>
        /// returns a list item at index vIndex 
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public RecordingListItem GetRecordingItem(int vIndex)
        {
            return DataSource[vIndex];
        }
        /// <summary>
        /// Set component data according to the passed in item
        /// </summary>
        /// <param name="vComponent"></param>
        /// <param name="vItem"></param>
        protected override void SetData(SingleRecordingListItemComponent vComponent, RecordingListItem vItem)
        {
            vComponent.onClick.RemoveListener(OnClickAction);
            vComponent.onClick.AddListener(OnClickAction);
            vComponent.SetData(vItem);
        }


        protected override void HighlightColoring(SingleRecordingListItemComponent component)
        {
            base.HighlightColoring(component);

        }


        /// <summary>
        /// returns the currently selected item.
        /// </summary>
        /// <returns></returns>
        public RecordingListItem GetSelectedItem()
        {
            if (SelectedIndex >= 0)
            {
                return dataSource[SelectedIndex];
            }
            return null;
        }

        public void SetItems(List<RecordingListItem> vRecordingItems)
        {
            //base.SetNewItems(vRecordingItems);
        }
    }
}