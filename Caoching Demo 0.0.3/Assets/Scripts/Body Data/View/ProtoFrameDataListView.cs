// /**
// * @file ProtoFrameDataViewDescription.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.View
{


    /// <summary>
    /// ProtoFrameDataViewDescription item description.
    /// </summary>
    [System.Serializable]
    public class ProtoFrameDataViewDescription
    {
        /// <summary>
        /// The icon.
        /// </summary>
        [SerializeField]
        public int Index;

        [SerializeField]
        public string RawQuat;

        [SerializeField]
        public string MappedQuat;

        [SerializeField]
        public string RawEuler;

        [SerializeField]
        public string MappedEuler;
        [SerializeField]
        public string Magnetometer;
        [SerializeField]
        public string Acceleration;
    }

 
 
    public class ProtoFrameDataListView : ListViewCustom<ProtoFrameDataListComponent, ProtoFrameDataViewDescription>
    {
        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected override void Awake()
        {
            Start();
        }

        [System.NonSerialized]
        bool mIsStartedListViewIcons = false;

     
        /// <summary>
        /// Start this instance.
        /// </summary>
        public override void Start()
        {
            if (mIsStartedListViewIcons)
            {
                return;
            }
            mIsStartedListViewIcons = true;

            base.Start();
        
            //DataSource.Comparison = ItemsComparison;
        }

        /// <summary>
        /// Sets component data with specified item.
        /// </summary>
        /// <param name="vComponent">Component.</param>
        /// <param name="vItem">Item.</param>
        protected override void SetData(ProtoFrameDataListComponent vComponent, ProtoFrameDataViewDescription vItem)
        {
            vComponent.SetData(vItem);
        }

        /// <summary>
        /// Set highlights colors of specified component.
        /// </summary>
        /// <param name="vComponent">Component.</param>
        protected override void HighlightColoring(ProtoFrameDataListComponent vComponent)
        {
            base.HighlightColoring(vComponent);
            vComponent.SetColor(HighlightedColor); 
        }

        /// <summary>
        /// Set select colors of specified component.
        /// </summary>
        /// <param name="vComponent">Component.</param>
        protected override void SelectColoring(ProtoFrameDataListComponent vComponent)
        {
            base.SelectColoring(vComponent);
            vComponent.SetColor(SelectedColor); 
        }

        /// <summary>
        /// Set default colors of specified component.
        /// </summary>
        /// <param name="vComponent">Component.</param>
        protected override void DefaultColoring(ProtoFrameDataListComponent vComponent)
        {
            base.DefaultColoring(vComponent);
            vComponent.SetColor(DefaultColor); 
        }
    }
}