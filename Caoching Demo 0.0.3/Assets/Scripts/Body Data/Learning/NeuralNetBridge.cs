using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using TPoseDetection.Model;
using System.Threading;
using System;

public class NeuralNetBridge : MonoBehaviour
{
    public Assets.Scripts.UI.RecordingLoading.CloudLocalStorageViewManager ViewManagerRef;
    public List<BodyFrame> mBodyFrames;
    private Thread mWorkerThread;
    Trainer mTrainer;

    /// <summary>
    /// Begin the learning process.
    /// </summary>
    /// <param name="vExamples">The list of examples containing the START, END, ISTPOSE CODE </param>
    internal void StartLearning(int[][] vExamples)
    {
        if (mBodyFrames == null)
        {
            return;
        }
        mWorkerThread = new Thread(BeginLearning);
        mWorkerThread.IsBackground = true;
        mWorkerThread.Start(vExamples);
        //System.Threading.ThreadPool.QueueUserWorkItem(BeginLearning, vExamples);
    }

    void OnApplicationQuit()
    {
        Debug.Log("appli quit");
        if (mTrainer != null)
        {
            mTrainer.Stop();
        }
        StopThread();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StopThread();
        }
    }

    void StopThread()
    {
        if (mTrainer != null)
        {
            mTrainer.Stop();
        }
        try
        {
            mWorkerThread.Abort();
        }
        catch (Exception vE)
        {
            Debug.Log(vE);
        }
    }

    private void BeginLearning(object state)
    {
        System.Diagnostics.Stopwatch vWatch = new System.Diagnostics.Stopwatch();
        vWatch.Start();
        int[][] vExamples = (int[][])state;
        List<OrientationSensorsCaptureFrame> vListOfSensorCaptureFrames = new List<OrientationSensorsCaptureFrame>();

        for (int i = 0; i < vExamples.Length; i++)
        {
            int vStartIndex = vExamples[i][0] - 6;
            int vEndIndex = vExamples[i][1] - 6;


            for (int j = vStartIndex; j <= vEndIndex; j++)
            {
                var vBodyFrame = mBodyFrames[j];
                var vFrameData = vBodyFrame.FrameData;
                List<OrientationSensorCapture> vFeatureVectorList = new List<OrientationSensorCapture>();
                foreach (var vKVal in vFrameData)
                {
                    int vPos = GetIntPositionOfSensor(vKVal.Key);
                    if(vPos == -1)
                    {
                        continue;
                    }
                    var vVal = vKVal.Value;
                    OrientationSensorCapture vOrientationSensorCapture = new OrientationSensorCapture(vBodyFrame.Index, vPos, vVal.x, vVal.y, vVal.z);
                    vFeatureVectorList.Add(vOrientationSensorCapture);
                }
                OrientationSensorsCaptureFrame vOrientationSensorsCaptureFrame = new OrientationSensorsCaptureFrame(vBodyFrame.Index, vFeatureVectorList);
                vListOfSensorCaptureFrames.Add(vOrientationSensorsCaptureFrame);
            }
        }

        if (mTrainer != null)
        {
            mTrainer.Stop();
        }

        try
        {
            FeatureVectorMap vFvm = new FeatureVectorMap(vListOfSensorCaptureFrames, vExamples);

            mTrainer = new Trainer(vFvm, "Sig", 0.001f, 0.6f, 1, 20, OnTrainingCompletion, ErrorProgressUpdate);
            mTrainer.Begin();
            mTrainer.Validation();
            vWatch.Stop();
        }
        catch (Exception vE)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                () =>
                {
                    UnityEngine.Debug.Log(vE.Message);
                    UnityEngine.Debug.Log(vE.StackTrace);
                });

        }
        Debug.Log("time taken to learn : " + vWatch.ElapsedMilliseconds / 1000f);

    }

    internal void Stop()
    {
        if (mTrainer != null)
        {
            mTrainer.Stop();
        }

        StopThread();
    }

    private void ErrorProgressUpdate(double obj)
    {
        OutterThreadToUnityThreadIntermediary.EnqueueOverwrittableActionInUnity("ErrorProgressUpdate",() =>
        {
            Debug.Log("Error progress: " + obj);
        });
    }

    private void OnTrainingCompletion(TrainingResults obj)
    {

        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
        {
            Debug.Log(" OnTrainingCompletion Done");
            Debug.Log(" PER: " + obj.Performance + "\n");
            Debug.Log(" ERR: " + obj.Error + "\n");
            Debug.Log(" TP: " + obj.TruePositive + " FP: " + obj.FalsePositive + "\n");
            Debug.Log(" TN: " + obj.TrueNegative + " FN: " + obj.FalseNegative + "\n");
        });


    }

    /// <summary>
    /// Returns an integer representation of a sensorposition
    /// if not valid, will return -1
    /// </summary>
    /// <param name="vPosition"></param>
    /// <returns></returns>
    static int GetIntPositionOfSensor(BodyStructureMap.SensorPositions vPosition)
    {
        switch (vPosition)
        {
            case BodyStructureMap.SensorPositions.SP_UpperSpine:
                return 0;
            case BodyStructureMap.SensorPositions.SP_RightUpperArm:
                return 1;
            case BodyStructureMap.SensorPositions.SP_RightForeArm:
                return 2;
            case BodyStructureMap.SensorPositions.SP_LeftUpperArm:
                return 3;
            case BodyStructureMap.SensorPositions.SP_LeftForeArm:
                return 4;
            case BodyStructureMap.SensorPositions.SP_RightThigh:
                return 5;
            case BodyStructureMap.SensorPositions.SP_RightCalf:
                return 6;
            case BodyStructureMap.SensorPositions.SP_LeftThigh:
                return 7;
            case BodyStructureMap.SensorPositions.SP_LeftCalf:
                return 8;
        }
        return -1;
    }

    public struct IndexTuple
    {
        int StartIndex;
        int EndIndex;
    }
}
