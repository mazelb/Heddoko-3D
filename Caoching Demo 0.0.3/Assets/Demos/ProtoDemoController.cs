﻿// /**
// * @file ProtoDemoController.cs
// * @brief Contains the ProtoDemoController class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Communication;
using Assets.Scripts.UI.RecordingLoading;
using HeddokoLib.adt;
using HeddokoLib.heddokoProtobuff;
using HeddokoLib.heddokoProtobuff.Decoder;
using UnityEngine;

namespace Assets.Demos
{
    [Serializable]
    public class ProtoDemoController : MonoBehaviour
    {
        public static bool UseProtoBuff = true;
        private Body mPlaceholder;
        public ProtoDemoConnectionController DemoConnectionController;
        private ProtoStreamDecoder mProtoStreamDecoder;
        public ProtobuffFrameRouter FrameRouter;


        void Start()
        {
            DemoConnectionController.ConnectedStateEvent += BridgeStreams;

        }

        void BridgeStreams()
        {
            if (mProtoStreamDecoder != null)
            {
                mProtoStreamDecoder.Dispose();
            }


            if (FrameRouter != null)
            {
                FrameRouter.StopIfWorking();
            }
            var vStream = DemoConnectionController.Port.BaseStream;
            mProtoStreamDecoder = new ProtoStreamDecoder(vStream, 4096);
            mProtoStreamDecoder.StartPacketizeStream(OnStreamComplete, ProtoStreamDecoderExceptionHandler);
            CircularQueue<RawPacket> mRawPacketBuffer = mProtoStreamDecoder.OutputBuffer;
            BodyFrameBuffer vbodyframebuffer = new BodyFrameBuffer(4096);
            FrameRouter = new ProtobuffFrameRouter(mRawPacketBuffer, vbodyframebuffer);
        }

        public void OnStreamComplete()
        {
            Debug.Log("Stream completed");
        }

        public void ProtoStreamDecoderExceptionHandler(Exception vE)
        {
            throw vE;
        }

        public void ChangeUsingProtobuffFlag(bool vValue)
        {
            UseProtoBuff = vValue;
            if (!UseProtoBuff)
            {
                DemoConnectionController.DisconnectBrainpack();
                DemoConnectionController.ConnectedStateEvent -= BridgeStreams;
                if (mProtoStreamDecoder != null)
                {
                    mProtoStreamDecoder.Dispose();
                    FrameRouter.StopIfWorking();
                }
            }
            else
            {
                DemoConnectionController.ConnectedStateEvent += BridgeStreams;
            }

        }

        void OnApplicationQuit()
        {

            if (mProtoStreamDecoder != null)
            {
                mProtoStreamDecoder.Dispose();
            }
            if (FrameRouter != null)
            {
                FrameRouter.StopIfWorking();
            }

        }


    }
}