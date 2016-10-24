/**
* @file ProtobuffFrameRouter.cs
* @brief Contains the ProtobuffFrameRouter
* @author Mohammed Haider( mohammed @heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.IO;
using System.Threading;
using heddoko;
using HeddokoLib.adt; 
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
                var vBodyFrame = new BodyFrame(vPacket);
                OutBoundBuffer.Enqueue(vBodyFrame);
            }
        }


        /// <summary>
        /// Start pulling data from the inbound buffer specified in the constructor 
        /// </summary>
        public void Start()
        {
            StopIfWorking();
            mIsWorking = true;
            mThread = new Thread(WorkerFunc);
            mThread.IsBackground = true;
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
                        mMemoryStream.Write(vRawPacket.Payload, 1,(int) vRawPacket.PayloadSize - 1);
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
                // mThread.Join();
                try
                {
                    mThread.Abort();
                }
                catch (Exception)
                {
                    
                 }
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
