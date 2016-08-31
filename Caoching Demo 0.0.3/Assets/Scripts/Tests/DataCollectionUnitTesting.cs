/**
* @file DataCollectionUnitTesting.cs
* @brief Contains the 
* @author Mohammed Haider( 
* @date 06 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics; 
using Assets.Scripts.Body_Data.View; 
using Assets.Scripts.Body_Pipeline.Analysis; 
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class DataCollectionUnitTesting : MonoBehaviour
    {
        public RecordingPlayerView RecordingPlayerView;
        public Body mBody;
        public Body mRecordingPlayerBoedy;
        private List<BodyFrame> mDataCollection = new List<BodyFrame>();
        private BodyFrame[] ConvertedFrame;
        private AnalysisDataStore mDataStore;
        void Start()
        {
            BodiesManager.Instance.CreateNewBody("Testerino");
            mBody = BodiesManager.Instance.GetBodyFromUUID("Testerino");
            mBody.UpdateRenderedBody(RenderedBodyPool.RequestResource(BodyStructureMap.BodyTypes.BodyType_FullBody));
            mRecordingPlayerBoedy = BodiesManager.Instance.GetBodyFromUUID("Root");
            //get all the attributes from class that are of either float or int
            var vSegments =  new List<SegmentAnalysis>();
            vSegments.AddRange(mBody.AnalysisSegments.Values);
            mDataStore = new AnalysisDataStore(vSegments); 
            foreach (var vSegmentAnalysis in vSegments)
            {
                vSegmentAnalysis.AnalysisCompletedEvent += mDataStore.UpdateSegmentFieldInfo;
            }
        }

 
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PumpData();
            }
        }

        private void PumpData()
        {   
            ConvertedFrame = mRecordingPlayerBoedy.MBodyFrameThread.PlaybackTask.ConvertedFrames.ToArray();
 
            StartCoroutine(vTesterino());
            StartCoroutine(Loadthis());

        }

        IEnumerator Loadthis()
        {
            yield return new  WaitForSeconds(0.5f);
         }
        IEnumerator vTesterino()
        {
            DisablingProgressBar.Instance.StartProgressBar("Converting");
                yield return null;

            Stopwatch vStop = new Stopwatch();
            vStop.Start();
            for (int i = 0; i < ConvertedFrame.Length; i++)
            {
                // yield return null;
                mDataStore.Update(ConvertedFrame[i]);

                mBody.UpdateBody(ConvertedFrame[i]);
                 if (i == 0)
                {
                    mBody.View.ResetInitialFrame(ConvertedFrame[0]);
                    
                }
                Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vDic = Body.GetTracking(mBody);
                Body.ApplyTracking(mBody, vDic);

            }
          //  string[,] vSerializedData = mDataStore.mSerialization.SerializeToString();
            int vTotalCount = mDataStore.SerializedList.Count;
            UnityEngine.Debug.Log("total " + (vTotalCount));

            vStop.Stop();
            UnityEngine.Debug.Log("Total time taken " + (vStop.ElapsedMilliseconds/1000f));
            UnityEngine.Debug.Log("writing to file");
            vStop.Reset();
            vStop.Start();
           AnalysisDataStoreSerialization.WriteFile(mDataStore);
            vStop.Stop();
            UnityEngine.Debug.Log("Total time taken " + (vStop.ElapsedMilliseconds / 1000f));

            DisablingProgressBar.Instance.StopAnimation();
        }
    }
}