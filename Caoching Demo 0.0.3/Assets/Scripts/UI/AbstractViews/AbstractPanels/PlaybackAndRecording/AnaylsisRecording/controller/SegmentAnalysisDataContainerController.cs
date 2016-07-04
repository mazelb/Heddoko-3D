 /**
 * @file SegmentAnalysisDataContainerController.cs
 * @brief Contains the SegmentAnalysisDataContainerController
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.Loading;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{
    [UserRolePermission(new[] { UserRoleType.Analyst })]
    public class SegmentAnalysisDataContainerController : MonoBehaviour
    {
        public SegmentAnalysisDataContainer Container;
        public Button AddSegmentButton;
        public Button ClearAllSegmentButton;
        public Button ConvertAll;
        private Body mPlaceholderBody;
        void Awake()
        {
            Container.Init();
            AddSegmentButton.onClick.AddListener(Container.AddNewSegmentSection);
            ClearAllSegmentButton.onClick.AddListener(Container.Clear);
            ConvertAll.onClick.AddListener(SerializeData);
            ConvertAll.interactable = false;
            AddSegmentButton.interactable = false;
            ClearAllSegmentButton.interactable = false;
            Container.NewConvertedFramesReceivedEvent += NewConvertedFramesReceivedEventHandler;
            BodiesManager.Instance.CreateNewBody("Placeholder analysis body");
            mPlaceholderBody = BodiesManager.Instance.GetBodyFromUUID("Placeholder analysis body");
            Container.Body = mPlaceholderBody;
            mPlaceholderBody.UpdateRenderedBody(RenderedBodyPool.RequestResource(BodyStructureMap.BodyTypes.BodyType_FullBody));

        }

        private void SerializeData()
        {
            StartCoroutine(SerializeAllDataStores());
        }

        private IEnumerator SerializeAllDataStores()
        {
            DisablingProgressBar.Instance.StartProgressBar();
            var ConvertedFrames = Container.ConvertedFrames;
            var vDataStoreList = Container.RecordingAnalysisSegmentSectionList;
            var vSegments = new List<SegmentAnalysis>();
            vSegments.AddRange(mPlaceholderBody.AnalysisSegments.Values);

            int vCurrentDataCount = 1;
            int vMaxConversionCount = vDataStoreList.Count;
            bool vStart = true;
            foreach (var vDataStore in vDataStoreList)
            {
                foreach (var vSegmentAnalysis in vSegments)
                {
                    //Clear out previous event handlers
                    vSegmentAnalysis.RemoveAnalysisCompletionListener(vDataStore.AnalysisSectionModel.DataStore.UpdateSegmentFieldInfo);
                    vSegmentAnalysis.AddAnalysisCompletionListener(vDataStore.AnalysisSectionModel.DataStore.UpdateSegmentFieldInfo);
                }

                vDataStore.AnalysisSectionModel.DataStore.Ignore = true;
                vStart = true;
                DisablingProgressBar.Instance.StartProgressBar("CONVERTING " + vCurrentDataCount + "/" + vMaxConversionCount + " jobs");
                yield return null;

                //Start updating the body between the given indices
                for (int i = vDataStore.AnalysisSectionModel.StartIndex; i < vDataStore.AnalysisSectionModel.EndIndex; i++)
                {

                    yield return null;
                    vDataStore.AnalysisSectionModel.DataStore.AddNewTimestamp(ConvertedFrames[i]);
                    mPlaceholderBody.UpdateBody(ConvertedFrames[i]);
                    if (vStart)
                    {
                        var vPreReset = BodySegment.IsUsingInterpolation;
                        BodySegment.IsUsingInterpolation = false;
                        mPlaceholderBody.View.ResetInitialFrame(ConvertedFrames[i]);
                        vStart = false;
                        BodySegment.IsUsingInterpolation = true;
                        continue;
                    }
                    Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vDic = Body.GetTracking(mPlaceholderBody);
                    Body.ApplyTracking(mPlaceholderBody, vDic);

                    //only ignore the first frame analysis
                    if (vDataStore.AnalysisSectionModel.DataStore.Ignore)
                    {
                        vDataStore.AnalysisSectionModel.DataStore.Ignore = false;

                    }
                }

                DisablingProgressBar.Instance.StartProgressBar("SAVING ANALYSIS RESULTS TO ... " + AnalysisDataStoreSerialization.GetSerializationStorePath);
                yield return new WaitForSeconds(1.5f);
                AnalysisDataStoreSerialization.WriteFile(vDataStore.AnalysisSectionModel.DataStore);
                vCurrentDataCount++;
            }

            DisablingProgressBar.Instance.StopAnimation();
            Container.Clear();
        }

        private void NewConvertedFramesReceivedEventHandler(BodyFrame[] vFrames)
        {
            ConvertAll.interactable = true;
            AddSegmentButton.interactable = true;
            ClearAllSegmentButton.interactable = true;
            Container.Clear();
        }


        void Update()
        {
         }
    }
}