/** 
* @file BrainpackDataList.cs
* @brief LeftArmAnalBrainpackDataListysis class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

 
using UIWidgets;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Communication.View.Table
{
    /// <summary>
    /// List of brainpack data
    /// </summary>
    public class BrainpackDataList : ListViewCustom<FrameSelectionComponent, BodyFrame>
    {
        private bool mInitialized = false;

        public int MaxDisplayCount
        {
            get
            {
                CalculateMaxVisibleItems();
                return maxVisibleItems;
            }
        }   
        public void Initialize()
        {
            if (mInitialized)
            {
                return;
            }
            mInitialized = true;
            base.Start();
        }

       

        /// <summary>
        /// Loads data for the current list
        /// </summary>
        /// <param name="vItemDescriptors"></param>
        public void LoadData(List<BodyFrame> vItemDescriptors)
        {
            DataSource.BeginUpdate();
            DataSource.Clear();
            for (int i = 0; i < vItemDescriptors.Count; i++)
            {
                DataSource.Add(vItemDescriptors[i]);
            }
            DataSource.EndUpdate();
             
        }

        public void ScrollToIndex(int v)
        {
             base.ScrollTo(v);
        }


        /// <summary>
        /// Sets the component with spefied vItem
        /// </summary>
        /// <param name="vComponenent"></param>
        /// <param name="vItem"></param>
        protected override void SetData(FrameSelectionComponent vComponenent, BodyFrame vItem)
        {
            vComponenent.SetData(vItem); 
        }

        protected override void HighlightColoring(FrameSelectionComponent vComponent )
        {
            base.HighlightColoring(vComponent);
            vComponent.SensorData.color = HighlightedColor;
            vComponent.TimeStamp.color = HighlightedColor;
        }

        protected override void SelectColoring(FrameSelectionComponent vComponent)
        {
            base.SelectColoring(vComponent);
            vComponent.SensorData.color = HighlightedColor;
            vComponent.TimeStamp.color = HighlightedColor;
        }

        protected override void DefaultColoring(FrameSelectionComponent vComponent)
        {
            base.DefaultColoring(vComponent);
            vComponent.SensorData.color = HighlightedColor;
            vComponent.TimeStamp.color = HighlightedColor;
        }



        /// <summary>
        /// Returns the number of current selected items
        /// </summary>
        /// <returns>The total number of counted items</returns>
        public int GetSelectedCount
        {
            get
            {
                int vTotalCount = SelectedItems.Count;
                return vTotalCount;
            }
        }

 



    }
}