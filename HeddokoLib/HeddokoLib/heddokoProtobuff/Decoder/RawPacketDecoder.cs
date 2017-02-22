// /**
// * @file RawPacketDecoder.cs
// * @brief Contains the RawPacketDecoder class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeddokoLib.adt;

namespace HeddokoLib.heddokoProtobuff.Decoder
{
    /// <summary>
    ///A raw packet decoder: enqueues an array of bytes. Uses a PacketizationCompleted to produce an event that a raw packet has been created. 
    /// </summary>
    public class RawPacketDecoder : IDisposable
    {
        private volatile bool mIsWorking;
        private CircularQueue<RawPacket> mConvertedPackets = new CircularQueue<RawPacket>(1024, false);
        private Queue<byte[]> mQueueByte = new Queue<byte[]>();
        public event PacketizationCompleted PacketizationCompletedEvent;
        /// <summary>
        /// A queue of byte arrays
        /// </summary>
        internal Queue<byte[]> ByteQueue
        {
            get
            {
                lock (mQueueByte)
                {
                    return mQueueByte;

                }
            }
        }


        /// <summary>
        /// The output circular buffer of converted packets. 
        /// </summary>
        public CircularQueue<RawPacket> ConvertedPackets
        {
            get { return mConvertedPackets; }
        }

        public void Start()
        {
            mIsWorking = true;
            ThreadPool.QueueUserWorkItem(WorkerFunc, null);
        }
        /// <summary>
        /// Enqueue raw bytes to be processed
        /// </summary>
        /// <param name="vArr"></param>
        public void EnqueueRawBytes(byte[] vArr)
        {
            ByteQueue.Enqueue(vArr);
        }
        /// <summary>
        /// The worker function that converts raw bytes into a raw packet data structure
        /// </summary>
        /// <param name="vStateObject"></param>
        private void WorkerFunc(object vStateObject)
        {
            RawPacket vPacket = new RawPacket();
            while (mIsWorking)
            {
                //give up a timeslice
                Thread.Sleep(1);

                if (ByteQueue.Count == 0)
                {
                    continue;
                }
                var vHeadPacket = ByteQueue.Dequeue();
                for (int vI = 0; vI < vHeadPacket.Length; vI++)
                {
                    byte vByte = vHeadPacket[vI];
                    //the byte is 0, this means that the current array buffer has received an incomplete amount of bytes
                    PacketStatus vPacketStatus = vPacket.ProcessByte(vByte);
                    if (vPacketStatus == PacketStatus.PacketComplete)
                    {
                        RawPacket vPacketCopy = new RawPacket(vPacket);
                        mConvertedPackets.Enqueue(vPacketCopy);
                        vPacket.ResetPacket();
                        if (PacketizationCompletedEvent != null)
                        {
                            PacketizationCompletedEvent();
                        }
                    }
                    else if (vPacketStatus == PacketStatus.PacketError)
                    {
                        vPacket.ResetPacket();
                    }
                }
            }
        }

        public void Dispose()
        {
            mIsWorking = false;
        }

        /// <summary>
        /// Stop processing 
        /// </summary>
        public void Stop()

        {
            mIsWorking = false;
        }

        /// <summary>
        /// Clear out internal datastructures holding raw bytes. 
        /// </summary>
        public void Clear()
        {
            ByteQueue.Clear();
        }
    }
}