// /**
// * @file UdpSocketListener.cs
// * @brief Contains the UdpSocketListener class
// * @author Mohammed Haider(mohammed@heddoko.com) 
// * @date September 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Scripts.Communication.View;
using HeddokoLib.adt;
using HeddokoLib.heddokoProtobuff.Decoder;
using RawPacket = HeddokoLib.heddokoProtobuff.Decoder.RawPacket;

namespace Assets.Scripts.Communication
{

    /// <summary>
    /// Listens to a socket for brainpacks, routing packets to a frame router
    /// </summary>
    public class UdpSocketListener
    {
        public static bool UseProtoBuff = true;
        private StreamToRawPacketDecoder mProtoStreamDecoder;
        private Socket mSocket;
        public ProtobuffFrameRouter FrameRouter;
        public CircularQueue<RawPacket> RawPacketBuffer { get; private set; }
        public int Port;
        private bool mIsWorking;

         private object mLockObj = new object();
        private StreamToRawPacketDecoder mDecoder;
         private UdpClient mUdpClient;

        /// <summary>
        /// Pass in a default port number, an inbound queue and an outbound queue
        /// </summary>
        /// <param name="vPortNumber"></param>
        /// <param name="vInboundBuffer"></param>
        /// <param name="vOutterBuffer"></param>
        public UdpSocketListener(int vPortNumber = 6669)
        {
            Port = vPortNumber;
       
            mDecoder = new StreamToRawPacketDecoder(new MemoryStream());
            RawPacketBuffer = mDecoder.OutputBuffer;
            BodyFrameBuffer vBodyframebuffer = new BodyFrameBuffer(256);
            FrameRouter = new ProtobuffFrameRouter(RawPacketBuffer, vBodyframebuffer);
        }
        /// <summary>
        /// Thread safe accessor/setter 
        /// </summary>
        public bool IsWorking
        {
            get
            {
                lock (mLockObj)
                {
                    return mIsWorking;
                }
            }
            set
            {
                lock (mLockObj)
                {
                    mIsWorking = value;
                }
            }
        }

     

        public void Start()
        {
            IsWorking = true;
            FrameRouter.Start();
            ThreadPool.QueueUserWorkItem(ListeningWorker);
        }

        public void Stop()
        {
            IsWorking = false;
            FrameRouter.StopIfWorking();
            if (mDecoder != null)
            {
                mDecoder.Dispose();
            }
            if (mUdpClient != null)
            {
                mUdpClient.Close();
            }
        }
        /// <summary>
        /// Udp listening worker
        /// </summary>
        private void ListeningWorker(object vCallbackObj)
        {
            try
            {
                mUdpClient = new UdpClient(port: Port);
                while (mIsWorking)
                {
                    IPEndPoint vRemoteEp = new IPEndPoint(IPAddress.Any, 0);
                    if (!mPacketProcessed)
                    {
                        continue;
                    }
                    
                    // Blocks until a message returns on this socket from a remote host.
                    byte[] vReceivedBytes = mUdpClient.Receive(ref vRemoteEp);
                    //write to memory stream
                    //reset the stream pointer, write and reset.
                    MemoryStream vStream = new MemoryStream();
                    vStream.Write(vReceivedBytes, 0, vReceivedBytes.Length);
                    vStream.Seek(0, SeekOrigin.Begin);
                    mPacketProcessed = false;
                    mDecoder.Stream = vStream;
                    mDecoder.StartPacketizeStream(ActionCompleted, ExceptionHandler);
                }
            }
            catch (Exception vE)
            {
                ExceptionHandler(vE); 
            }
        }
        bool mPacketProcessed = true;
        private void ActionCompleted()
        {
            
            mPacketProcessed = true;
        }

        private void ExceptionHandler(Exception vObj)
        {
            UnityEngine.Debug.Log(vObj.Message);
            mPacketProcessed = true;
        }
    }
}