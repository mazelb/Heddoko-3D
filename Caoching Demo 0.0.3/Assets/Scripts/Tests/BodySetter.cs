// /**
// * @file BodySetter.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 01 2017
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Demos;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    public class BodySetter : MonoBehaviour
    {
        public ProtoLanLiveView LiveFeedView;
        public RecordingPlayerView RecordingsView;
        public ProbuffMessageViewController MessagesView;
        public Button LiveviewFeedButton;
        public Button RecordingFeedButton;

        void Start()
        {
            LiveviewFeedButton.onClick.AddListener(
                () =>
                {
                    MessagesView.UpdateBody(LiveFeedView.BrainpackBody);
                }
                );
            RecordingFeedButton.onClick.AddListener(
                () =>
                {
                    MessagesView.UpdateBody(RecordingsView.CurrBody);
                }
                );

        }
    }
}