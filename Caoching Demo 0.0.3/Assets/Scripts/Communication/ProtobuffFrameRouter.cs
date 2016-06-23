﻿/**
* @file ProtobuffFrameRouter.cs
* @brief Contains the ProtobuffFrameRouter
* @author Mohammed Haider( mohammed @heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using HeddokoLib.adt;
using HeddokoLib.heddokoProtobuff;
using HeddokoLib.heddokoProtobuff.Decoder;
using ProtoBuf;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Communication
{
    /// <summary>
    /// Reroutes protobuff frames to different tasks
    /// </summary>
    public class ProtobuffFrameRouter
    {

        private volatile bool mIsWorking;
        private Thread mThread;
        private CircularQueue<RawPacket> mInboundPacketBuffer;
        private BodyFrameBuffer mOutBoundBuffer;
        private ProtobuffDispatchRouter mProtDispatchRouter;
        //private static Dictionary<int, BodyStructureMap.SensorPositions> sSensorPositionList;
        private MemoryStream mMemoryStream = new MemoryStream();
        /// <summary>
        /// Constructor needing an inbound and outbound buffer. Call Start to start the process. 
        /// </summary>
        /// <param name="vInboundBuffer"></param>
        /// <param name="vOutterBuffer"></param>
        public ProtobuffFrameRouter(CircularQueue<RawPacket> vInboundBuffer, BodyFrameBuffer vOutterBuffer)
        {
            Serializer.PrepareSerializer<Packet>();

            mInboundPacketBuffer = vInboundBuffer;
            mOutBoundBuffer = vOutterBuffer;
            mProtDispatchRouter = new ProtobuffDispatchRouter();
            mProtDispatchRouter.Init();
            mProtDispatchRouter.Add(PacketType.DataFrame, EnqueueDataFrame);
            //initialize the static list of sensor positions if it is null. 
            //if (sSensorPositionList == null)
            //{
            //    sSensorPositionList = new Dictionary<int, BodyStructureMap.SensorPositions>();
            //    sSensorPositionList.Add(0, BodyStructureMap.SensorPositions.SP_UpperSpine);
            //    sSensorPositionList.Add(1, BodyStructureMap.SensorPositions.SP_RightUpperArm);
            //    sSensorPositionList.Add(2, BodyStructureMap.SensorPositions.SP_RightForeArm);
            //    sSensorPositionList.Add(3, BodyStructureMap.SensorPositions.SP_LeftUpperArm);
            //    sSensorPositionList.Add(4, BodyStructureMap.SensorPositions.SP_LeftForeArm);
            //    sSensorPositionList.Add(5, BodyStructureMap.SensorPositions.SP_RightThigh);
            //    sSensorPositionList.Add(6, BodyStructureMap.SensorPositions.SP_RightCalf);
            //    sSensorPositionList.Add(7, BodyStructureMap.SensorPositions.SP_LeftThigh);
            //    sSensorPositionList.Add(8, BodyStructureMap.SensorPositions.SP_LeftCalf);
            //}
        }

        /// <summary>
        /// Enqueues a data frame to the outbound buffer
        /// </summary> 
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void EnqueueDataFrame(object vSender, object vArgs)
        {
            Packet vPacket = (Packet)vArgs;

            if (OutBoundBuffer.AllowOverflow || (!OutBoundBuffer.AllowOverflow && !OutBoundBuffer.IsFull()))
            {
                var vBodyFrame = new BodyFrame(vPacket);// ConvertPacketToBodyFrame(vPacket);
                OutBoundBuffer.Enqueue(vBodyFrame);
            }
        }

        ///// <summary>
        ///// Converts a packet to a bodyframe
        ///// </summary>
        ///// <param name="vPacket"></param>
        ///// <returns></returns>
        //private BodyFrame ConvertPacketToBodyFrame(Packet vPacket)
        //{
        //    var vImuDataFrame = (ImuDataFrame)vPacket.fullDataFrame.imuDataFrame[0];
        //    var vBodyFrame = new BodyFrame(vPacket);
        //    vBodyFrame.Timestamp = vPacket.fullDataFrame.timeStamp;
        //    //The packet passed in is a data frame. need to check if the data passed in is in quaternion form or euler
        //    if (vImuDataFrame.Rot_xSpecified)
        //    {
        //        foreach (var vKeyPair in sSensorPositionList)
        //        {
        //            var vFrameData = vBodyFrame.FrameData[vKeyPair.Value];
        //            vFrameData.x = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].Rot_x;
        //            vFrameData.y = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].Rot_y;
        //            vFrameData.z = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].Rot_z;
        //            vBodyFrame.FrameData[vKeyPair.Value] = vFrameData;
        //        }
        //    }
        //    else if (vImuDataFrame.quat_wSpecified)
        //    {
        //        foreach (var vKeyPair in sSensorPositionList)
        //        {
        //            var vFrameData = vBodyFrame.FrameData[vKeyPair.Value];
        //            vFrameData.x = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].quat_x_yaw;
        //            vFrameData.y = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].quat_y_pitch;
        //            vFrameData.z = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].quat_z_roll;
        //            vFrameData.w = vPacket.fullDataFrame.imuDataFrame[vKeyPair.Key].quat_w;
        //            vBodyFrame.FrameData[vKeyPair.Value] = vFrameData;
        //        }
        //    }
        //    return vBodyFrame;
        //}


        /// <summary>
        /// Start pulling data from the inbound buffer specified in the constructor 
        /// </summary>
        public void Start()
        {
            StopIfWorking();
            mIsWorking = true;
            mThread = new Thread(WorkerFunc);
            mThread.Start();
        }

        /// <summary>
        /// The worker function that pulls data from the inbound buffer
        /// </summary>
        private void WorkerFunc()
        {
            while (mIsWorking)
            {
                try
                {
                    if (mInboundPacketBuffer.Count == 0)
                    {
                        continue;
                    }
                    //dequeue the inbound buffer
                    var vRawPacket = mInboundPacketBuffer.Dequeue();
                    if (vRawPacket.Payload[0] == 0x04)
                    {
                        //reset the stream pointer, write and reset.
                        mMemoryStream.Seek(0, SeekOrigin.Begin);
                        mMemoryStream.Write(vRawPacket.Payload, 1, vRawPacket.PayloadSize - 1);
                        mMemoryStream.Seek(0, SeekOrigin.Begin);
                        Packet vProtoPacket = Serializer.Deserialize<Packet>(mMemoryStream); 
                        mProtDispatchRouter.Process(vProtoPacket.type, this, vProtoPacket);
                        mMemoryStream.SetLength(0);
                    }
                }
                catch (System.Exception vException)
                {

                    Debug.Log("Error " + vException);
                }
            }
        }
         
        /// <summary>
        /// Stops pulling data from an inbound buffer
        /// </summary>
        public void StopIfWorking()
        {
            if (mIsWorking)
            {
                mIsWorking = false;
                mThread.Join();
            }
        }
        /// <summary>
        /// The outbound buffer
        /// </summary>
        public BodyFrameBuffer OutBoundBuffer
        {
            get { return mOutBoundBuffer; }
            private set { mOutBoundBuffer = value; }
        }
    }
}