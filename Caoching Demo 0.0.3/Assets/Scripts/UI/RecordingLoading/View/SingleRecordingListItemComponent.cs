// /**
// * @file SingleRecordingListItemComponent.cs
// * @brief Contains the SingleRecordingListItemComponent
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.UI.RecordingLoading.Model;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    public class SingleRecordingListItemComponent : ListViewItem, IResizableItem
    {
        public Text NameRow;
        public Text UserRow;
        public SingleRecListItemButtons ButtonContainer;
        private GameObject[] mObjectsToResize;
        void Awake()
        {
            base.Awake();
            mObjectsToResize = new[] { NameRow.gameObject, ButtonContainer.gameObject };
        }

        /// <summary>
        /// required by IResizableItem: the objects to resize
        /// </summary>
        public GameObject[] ObjectsToResize
        {
            get { return mObjectsToResize; }
        }

        /// <summary>
        /// Sets the data from the given item
        /// </summary>
        /// <param name="vItem"></param>
        public void SetData(RecordingListItem vItem)
        {
            NameRow.text = vItem.Name;
            ButtonContainer.SetData(vItem);
            if (vItem.User != null)
            {
                UserRow.text = vItem.User.Name;
            }
            else
            {
                UserRow.text = "";
            }
        }


    }
}