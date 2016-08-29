// /**
// * @file SingleUploadableListComponent.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.UI.RecordingLoading.Model;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    public class SingleUploadableListComponent : ListViewItem, IResizableItem
    {
        public UnityEngine.UI.Text NameRow;
        public Image IsNewImage;
        private GameObject[] mObjectsToResize;
        void Awake()
        {
            base.Awake();
            mObjectsToResize = new[] { NameRow.gameObject, IsNewImage.gameObject };
        }

        public GameObject[] ObjectsToResize
        {
            get { return mObjectsToResize; }
        }


        /// <summary>
        /// Set the data to the view.
        /// </summary>
        /// <param name="vItem"></param>
        public void SetData(UploadableListItem vItem)
        {
            NameRow.text = vItem.FileName;
            IsNewImage.gameObject.SetActive(vItem.IsNew);
        }

    }
}