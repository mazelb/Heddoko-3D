// /**
// * @file RecordingListSyncView.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.Tagging;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    /// <summary>
    /// View: describes recording items that are cached/available for download
    /// </summary>
    public class RecordingListSyncView : ListViewCustom<SingleRecordingListItemComponent, RecordingListItem>
    {

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
                dataSource.Add(vSingleRecItemList[i]);
            }
            DataSource.EndUpdate();
        }
         
        protected override void SetData(SingleRecordingListItemComponent vComponenent, RecordingListItem vItem)
        {
            vComponenent.SetData(vItem); 
        }
    }
}