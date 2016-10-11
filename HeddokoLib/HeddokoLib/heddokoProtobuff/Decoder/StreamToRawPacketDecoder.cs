/**
* @file StreamToRawPacketDecoder.cs
* @brief Contains the StreamToRawPacketDecoder class
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using HeddokoLib.adt;
namespace HeddokoLib.heddokoProtobuff.Decoder
{

    public delegate void PacketizationCompleted();
    /// <summary>
    /// A Stream Decoder,taking in a binary stream, and places the resulting raw packet to an outputbuffer. Uses a ReaderWriterLock to guarantee threadsafety for underlying stream. 
    /// </summary>
    public class StreamToRawPacketDecoder
    {
        private Stream mStream;
        private volatile bool mIsWorking = false;
        private int mBufferSize = 1024;
        private CircularQueue<RawPacket> mQueue = new CircularQueue<RawPacket>(1024, false);
        private Queue<byte[]> vQueueByte= new Queue<byte[]>();
        public PacketizationCompleted PacketizationCompletedEvent;
        public Stream Stream
        {
            get
            {
                return mStream;
            }
            set
            {
                mStream = value;
            }

        }
        /// <summary>
        /// Default constructor: takes in a stream T
        /// </summary>
        /// <exception cref="NullReferenceException">Throws a null reference exception if the passed in buffer is null</exception>
        /// <param name="vStream"></param>
        public StreamToRawPacketDecoder(Stream vStream)
        {
            if (vStream == null)
            {
                throw new NullReferenceException("A non-null stream is required.");
            }
            Stream = vStream;
        }

        /// <summary>
        /// Default constructor: takes in a stream T,sets the internal buffer size to the passed in paramater and the output buffer size is defaulted to 1024
        /// </summary>
        /// <exception cref="NullReferenceException">Throws a null reference exception if the passed in buffer is null</exception>
        /// <param name="vStream"></param>
        /// <param name="vInputBufferSize"></param>
        public StreamToRawPacketDecoder(Stream vStream, int vInputBufferSize) : this(vStream)
        {
            OutputBuffer = new CircularQueue<RawPacket>(1024, false);
            mBufferSize = vInputBufferSize;
        }

        /// <summary>
        /// takes in a stream T,sets the internal buffer size to the passed in paramater and the output buffer size is set to the output buffer size
        /// </summary>
        /// <param name="vStream"></param>
        /// <param name="vInputBufferSize"></param>
        /// <param name="vOutputBufferSize"></param>
        public StreamToRawPacketDecoder(Stream vStream, int vInputBufferSize, int vOutputBufferSize)
            : this(vStream, vInputBufferSize)
        {
            OutputBuffer = new CircularQueue<RawPacket>(vOutputBufferSize, false);
        }

        /// <summary>
        /// The size of the stream buffer
        /// </summary>
        public int BufferSize
        {
            get { return mBufferSize; }
            set { mBufferSize = value; }
        }

        /// <summary>
        /// The output buffer of RawPackets
        /// </summary>
        public CircularQueue<RawPacket> OutputBuffer
        {
            get
            {
                return mQueue;
            }
            private set { mQueue = value; }
        }


        /// <summary>
        /// Begins to read the content of the stream, searching for patterns that make up a RawPacket
        /// </summary>
        /// <remarks>Since the protostream decoder uses a stream and uses a worker function on a seperate thread, all stream related exceptions are thrown. It is up to the developer to manage these exceptions accordingly.  
        /// <exception cref="ProtoStreamDecoderIsWorkingException">A ProtoStreamDecoderIsWorkingException is thrown if the current stream is still being decoded</exception>
        /// <param name="vStreamCompletionAction">callback action when the stream has no more data to offer</param>
        /// <param name="vExceptionInvocation">Exception handler.</param>
        public void StartPacketizeStream(Action vStreamCompletionAction, Action<Exception> vExceptionInvocation)
        {
            //if the previous worker is still working, raise an exception
            if (mIsWorking)
            {
                throw new Exception("The current StreamToRawPacketDecoder object is still working.");
            }
            mIsWorking = true;
            ThreadPool.QueueUserWorkItem(WorkerFunc, vStreamCompletionAction);
        }

        public void EnqueueByteData( byte[] vByteData)
        {
            lock (vQueueByte)
            {
                vQueueByte.Enqueue(vByteData);
            }
        }
        /// <summary>
        /// Returns a list of raw packets from a file stream
        /// </summary>
        /// <param name="vStream"></param>
        /// <param name="vBufferSize"></param>
        /// <returns></returns>
        public static List<RawPacket> StartPacketizingFromFileStream(FileStream vStream, int vBufferSize)
        {
            List<RawPacket> vPacketList = new List<RawPacket>();
            byte[] vByteArrayBuffer = new byte[vBufferSize];
            RawPacket vPacket = new RawPacket();
            while (vStream.CanRead)
            {

                int vNumberOfByteRead = vStream.Read(vByteArrayBuffer, 0, vBufferSize);
                if (vNumberOfByteRead == 0)
                {
                    break;
                }
                for (int vI = 0; vI < vNumberOfByteRead; vI++)
                {
                    byte vByte = vByteArrayBuffer[vI];
                    //the byte is 0, this means that the current array buffer has received an incomplete amount of bytes
                    PacketStatus vPacketStatus = vPacket.ProcessByte(vByte);
                    if (vPacketStatus == PacketStatus.PacketComplete)
                    {
                        RawPacket vPacketCopy = new RawPacket(vPacket);
                        vPacketList.Add(vPacketCopy);
                        vPacket.ResetPacket();
                    }
                    else if (vPacketStatus == PacketStatus.PacketError)
                    {
                        vPacket.ResetPacket();
                    }
                }

            }
            vStream.Close();
            return vPacketList;
        }
        /// <summary>
        /// The working function whose duty is to decode a binary stream into a protobuf packet. Places packets on the output buffer 
        /// </summary> 
        /// <param name="vState"></param>
        private void WorkerFunc(Object vState)
        {
            byte[] vByteArrayBuffer = new byte[BufferSize];
            RawPacket vPacket = new RawPacket();
            while (Stream.CanRead)
            {
                if (OutputBuffer.IsFull())
                {
                    continue;
                }
                int vNumberOfByteRead = Stream.Read(vByteArrayBuffer, 0, BufferSize);
                if (vNumberOfByteRead == 0)
                {
                    break;
                }
                for (int vI = 0; vI < vNumberOfByteRead; vI++)
                {
                    byte vByte = vByteArrayBuffer[vI];
                    //the byte is 0, this means that the current array buffer has received an incomplete amount of bytes
                    PacketStatus vPacketStatus = vPacket.ProcessByte(vByte);
                    if (vPacketStatus == PacketStatus.PacketComplete)
                    {
                        RawPacket vPacketCopy = new RawPacket(vPacket);
                        OutputBuffer.Enqueue(vPacketCopy);
                        vPacket.ResetPacket();
                    }
                    else if (vPacketStatus == PacketStatus.PacketError)
                    {
                        vPacket.ResetPacket();
                    }
                }

            }
            Action vCallbackAction = (Action)vState;
            if (vCallbackAction != null)
            {
                vCallbackAction.Invoke();
            }
            mIsWorking = false;
            Dispose();
        }

        public void ProcessPacket(byte[] vArray)
        {
            
        }

        public void Dispose()
        {
            Stream.Close();
            mIsWorking = false;
        }


    }

    /// <summary>
    /// An exception when a protostream object is still working
    /// </summary>
    public class ProtoStreamDecoderIsWorkingException : Exception
    {
        public ProtoStreamDecoderIsWorkingException(string vMsg) : base(vMsg)
        {
        }
    }

    /// <summary>
    /// A delegate that is invoked when raw packet has been completed. Implements IDisposable
    /// </summary>
    /// <param name="vPacket"></param>
    public delegate void RawPacketCompleted(RawPacket vPacket);
}