// /**
// * @file UploadableListSyncView.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using Assets.Scripts.UI.RecordingLoading.Model;
using UIWidgets;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    public class UploadableListSyncView : ListViewCustom<SingleUploadableListComponent, UploadableListItem>
    {
         
        /// <summary>
        /// Load data into the list
        /// </summary>
        /// <param name="vSingleRecItem"></param>
        public void LoadData(List<UploadableListItem> vUploadableItemsList)
        {
            DataSource.BeginUpdate();
            DataSource.Clear();
            for (int i = 0; i < vUploadableItemsList.Count; i++)
            {
                dataSource.Add(vUploadableItemsList[i]);
            }
            DataSource.EndUpdate();
        }
         
        protected override void SetData(SingleUploadableListComponent vComponenent, UploadableListItem vItem)
        {
            vComponenent.SetData(vItem); 
        }
    }
}