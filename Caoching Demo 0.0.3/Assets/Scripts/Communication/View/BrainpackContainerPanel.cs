// /**
// * @file BrainpackContainerPanel.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using Assets.Scripts.Utils;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public delegate void BrainpackSelected(BrainpackNetworkingModel vSelected);
    /// <summary>
    /// A view containing Brainpack models.
    /// </summary>
    public class BrainpackContainerPanel : MonoBehaviour
    {
        private Dictionary<string, BrainpackModelView> mBrainpackModelViewList = new Dictionary<string, BrainpackModelView>();
        private Transform mCurrentTransform;
        public BrainpackModelView DefaultModelView;
        public BrainpackStatusPanel StatusPanel;
        public event BrainpackSelected BrainpackSelectedEvent;
        void Start()
        {
            mCurrentTransform = GetComponent<Transform>();
            DefaultModelView.gameObject.SetActive(false);
        }
        public void AddBrainpackModel(BrainpackNetworkingModel vBrainpack)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                string vKey = vBrainpack.Id;
                if (!mBrainpackModelViewList.ContainsKey(vKey))
                {
                    var vGo = GameObject.Instantiate(DefaultModelView);
                    vGo.transform.SetParent(mCurrentTransform, false);
                    vGo.gameObject.SetActive(true);
                    vGo.Initialize(vBrainpack, SelectBrainpack);
                    mBrainpackModelViewList.Add(vKey, vGo);
                }

            });

        }

        private void SelectBrainpack(BrainpackNetworkingModel vBrainpack)
        {
            if (BrainpackSelectedEvent != null)
            {
                BrainpackSelectedEvent(vBrainpack);
            }
            StatusPanel.UpdateView(vBrainpack);
        }

        public void RemoveBrainpackModel(BrainpackNetworkingModel vBrainpack)
        {

            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                string vKey = vBrainpack.Id;
                if (mBrainpackModelViewList.ContainsKey(vKey))
                {
                    var vBpModel = mBrainpackModelViewList[vKey];
                    mBrainpackModelViewList.Remove(vKey);
                    vBpModel.Clear();
                    Destroy(vBpModel.gameObject);
                    StatusPanel.ClearIfBrainpack(vBrainpack);
                }
            });


        }
    }
}