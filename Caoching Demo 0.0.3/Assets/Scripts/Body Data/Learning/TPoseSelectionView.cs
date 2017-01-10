using System;
using System.Collections.Generic;
using Assets.Scripts.UI.RecordingLoading; 
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Body_Data.Learning
{
    public class TPoseSelectionView : MonoBehaviour
    {
        public List<TposeSelectionItemView> ItemViewList = new List<TposeSelectionItemView>();
        public TposeSelectionItemView DefaultItem;
        public RectTransform Parent;
        public NeuralNetBridge NeuralNetBridge;
        public RecordingPlayerView RecordingPlayerView;
        Frames_Pipeline.RecordingPlaybackTask mRecordingPlaybackTask;
        public Button StartButton;

        void Start()
        {
            RecordingPlayerViewConstructedHandler(RecordingPlayerView);
            DefaultItem.gameObject.SetActive(false);
        }

        void RecordingPlayerViewConstructedHandler(RecordingPlayerView vRecordingPlayerView)
        {
            vRecordingPlayerView.PbControlPanel.RecordingPlaybackTaskUpdated += PbControlPanel_RecordingPlaybackTaskUpdated;
        }

        private void PbControlPanel_RecordingPlaybackTaskUpdated(Frames_Pipeline.RecordingPlaybackTask obj)
        {
            mRecordingPlaybackTask = obj;
            mRecordingPlaybackTask.BodyFrameConversionCompletedEvent += MRecordingPlaybackTask_BodyFrameConversionCompletedEvent;
        }

        private void MRecordingPlaybackTask_BodyFrameConversionCompletedEvent(List<BodyFrame> vBodyFrames)
        {
            Assets.Scripts.Utils.OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
                     {
                         NeuralNetBridge.mBodyFrames = vBodyFrames;
                         StartButton.interactable = true;
                     });
        }

        public void Add()
        {
            var vGo = GameObject.Instantiate(DefaultItem);
            vGo.transform.SetParent(Parent, false);
            ItemViewList.Add(vGo);
            vGo.gameObject.SetActive(true);
        }

        public void Clear()
        {
            while (ItemViewList.Count > 0)
            {
                var vGo = ItemViewList[0].gameObject;
                ItemViewList.RemoveAt(0);
                Destroy(vGo);
            }
        }

        public void StartLearn()
        {
            int[][] vExamples = new int[ItemViewList.Count][];
            for (int i = 0; i < vExamples.Length; i++)
            {
                vExamples[i] = new int[3];
                vExamples[i][0] = ItemViewList[i].StartInterval;
                vExamples[i][1] = ItemViewList[i].EndInterval;
                vExamples[i][2] = ItemViewList[i].IsTPoseValue;
                Debug.Log(" VExamples: ");
                Debug.Log(vExamples[i][0] + " " + vExamples[i][1] + " " + vExamples[i][2] + "\n");
            }
            NeuralNetBridge.Stop();
            NeuralNetBridge.StartLearning(vExamples);
            StartButton.interactable = false;
            Clear();
        }

        /// <summary>
        /// Interupts learning
        /// </summary>
        public void StopLearn()
        {
            StartButton.interactable = true;
            NeuralNetBridge.Stop();
        }

    }
}
