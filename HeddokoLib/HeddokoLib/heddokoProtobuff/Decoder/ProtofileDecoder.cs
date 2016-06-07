// /**
// * @file ProtofileDecoder.cs
// * @brief Contains the 
// * @author Mohammed Haider(mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Net;

namespace HeddokoLib.heddokoProtobuff.Decoder
{
    public delegate void FileCompletedEvent(object vSender, object vArgs);

    /// <summary>
    /// A protofile decoder. 
    /// </summary>
    public class ProtofileDecoder
    {
        /// <summary>
        /// File read completion event
        /// </summary>
        private event FileCompletedEvent mFileCompletedEvent;
        /// <summary>
        /// A Protoframe decoder
        /// </summary>
        private ProtoFrameDecoder mFrameDecoder =new ProtoFrameDecoder();
 
        /// <summary>
        /// Registers a handler
        /// </summary>
        /// <param name="vHandler"></param>
        public void RegisterFileCompletedEventHandler(FileCompletedEvent vHandler)
        {
            mFileCompletedEvent += vHandler;
        }

        /// <summary>
        /// Removes a file completed event handler
        /// </summary>
        /// <param name="vHandler"></param>
        public void DeRegisterFileCompletedEventHandler(FileCompletedEvent vHandler)
        {
            mFileCompletedEvent -= vHandler;
        }

        /// <summary>
        /// Reads a Heddoko protobuf file
        /// </summary>
        /// <param name="vPath"></param>
        public void ReadFile(string vPath)
        {
            
        }


    }

   
}