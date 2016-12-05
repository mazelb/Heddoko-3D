// /**
// * @file ProtobuffFrameBodyFrameConverter.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using heddoko;
using HeddokoLib.adt;
using HeddokoLib.heddokoProtobuff.Decoder;
using ProtoBuf;

namespace Assets.Scripts.Body_Data
{
    /// <summary>
    /// Converts a protobuff frame in a Body Frame
    /// </summary>
    public class ProtobuffFrameBodyFrameConverter : ThreadedJob
    {
        private BodyFrameBuffer mOutBoundBuffer;
        private CircularQueue<Packet> mPacketBuffer;
        private Queue<RawPacket> mInBoundBuffer = new Queue<RawPacket>();  

        /// <summary>
        /// Constructor needing an inbound and outbound buffer. Call Start to start the process. 
        /// </summary>
        /// <param name="vPacketBuffer"></param>
        /// <param name="vBodyFrameBuffer"></param>

        public ProtobuffFrameBodyFrameConverter(CircularQueue<Packet> vPacketBuffer, BodyFrameBuffer vBodyFrameBuffer)
        {
            Serializer.PrepareSerializer<Packet>();
            mPacketBuffer = vPacketBuffer;
            mOutBoundBuffer = vBodyFrameBuffer;
        }
        

        /// <summary>
        /// The worker function that pulls data from the inbound buffer
        /// </summary>
        protected override void ThreadFunction()
        {
            while (!mIsDone)
            {
                try
                {
                    if (mPacketBuffer.Count == 0)
                    {
                        continue;
                    }

                    //dequeue the inbound buffer
                    var vPacket = mPacketBuffer.Dequeue();
                    if (vPacket != null)
                    {
                        BodyFrame vBodyFrame = new BodyFrame(vPacket);
                        mOutBoundBuffer.Enqueue(vBodyFrame);
                    }
                }

                catch (System.Exception vException)
                {
                    UnityEngine.Debug.Log("Error " + vException);
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

        public Queue<RawPacket> InBoundBuffer
        {
            get { return mInBoundBuffer; }
        }
    }
}