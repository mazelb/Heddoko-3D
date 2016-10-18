// /**
// * @file TPoseList.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Body_Data.CalibrationData.TposeSelection;
using UIWidgets;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording.view
{
    public class TPoseList : ListViewCustom<TPoseSelectionComponent, TPoseSliderMask>
    {
        public UnityAction OnClickAction;
        public UnityAction<string> OnLeftItemChanged;

        public UnityAction<string> OnTposeItemChanged;

        public UnityAction<string> OnRightItemChanged;


        void Awake()
        {
            base.Start();
        }
        /// <summary>
        /// Load data into the list
        /// </summary>
        /// <param name="vSingleRecItem"></param>
        public void LoadData(List<TPoseSliderMask> vSingleRecItemList)
        {
            DataSource.BeginUpdate();
            // DataSource.Clear();
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
        public TPoseSliderMask GetRecordingItem(int vIndex)
        {
            return DataSource[vIndex];
        }
        /// <summary>
        /// Set component data according to the passed in item
        /// </summary>
        /// <param name="vComponent"></param>
        /// <param name="vItem"></param>
        protected override void SetData(TPoseSelectionComponent vComponent, TPoseSliderMask vItem)
        {

            vItem.PoseSelection.RemoveAllListeners();
            vComponent.SetData(vItem);
            vComponent.DeleteButton.onClick.RemoveAllListeners();
            vComponent.DeleteButton.onClick.AddListener(() =>
            {
                int vIndex = dataSource.IndexOf(vItem);
                dataSource.Remove(vItem);
            });
            vComponent.LeftIndex.onValueChanged.RemoveAllListeners();
            vComponent.TPoseIndex.onValueChanged.RemoveAllListeners();
            vComponent.RightIndex.onValueChanged.RemoveAllListeners();
            vItem.PoseSelection.AddLeftIndexListener(() =>
            {
                vComponent.LeftIndex.text = vItem.PoseSelection.PoseIndexLeft + "";
            });
            vItem.PoseSelection.AddRightIndexListener(() =>
            {
                vComponent.RightIndex.text = vItem.PoseSelection.PoseIndexRight + "";
            });
            vItem.PoseSelection.AddMainIndexListener(() =>
            {
                vComponent.TPoseIndex.text = vItem.PoseSelection.PoseIndex + "";
            });
            vComponent.LeftIndex.onValueChanged.AddListener((x) =>
            {
                int vResult = -1;
                int.TryParse(x, out vResult);
                if (vResult > -1)
                {
                    vItem.PoseSelection.PoseIndexLeft = vResult;
                }
                vItem.VerifyLimits();
                vItem.MoveRightEdge(vItem.PoseSelection.PoseIndexRight, 1);
                vItem.MoveLeftEdge(vItem.PoseSelection.PoseIndexLeft, 1);
            });

            vComponent.RightIndex.onValueChanged.AddListener((x) =>
            {
                int vResult = -1;
                int.TryParse(x, out vResult);
                if (vResult > -1)
                {
                    vItem.PoseSelection.PoseIndexRight = vResult;
                }
                vItem.VerifyLimits();
                vItem.MoveRightEdge(vItem.PoseSelection.PoseIndexRight, 1);
                vItem.MoveLeftEdge(vItem.PoseSelection.PoseIndexLeft, 1);
            });
            vComponent.TPoseIndex.onValueChanged.AddListener((x) =>
            {
                int vResult = -1;
                int.TryParse(x, out vResult);
                if (vResult > -1)
                {
                    vItem.PoseSelection.PoseIndex = vResult;
                }
                vItem.VerifyLimits();
                vItem.MoveRightEdge(vItem.PoseSelection.PoseIndexRight, 1);
                vItem.MoveLeftEdge(vItem.PoseSelection.PoseIndexLeft, 1);
            });


        }





        /// <summary>
        /// returns the currently selected item.
        /// </summary>
        /// <returns></returns>
        public TPoseSliderMask GetSelectedItem()
        {
            if (SelectedIndex >= 0)
            {
                return dataSource[SelectedIndex];
            }
            return null;
        }

        public void SetItems(List<TPoseSliderMask> vRecordingItems)
        {
            //base.SetNewItems(vRecordingItems);
        }
    }
}