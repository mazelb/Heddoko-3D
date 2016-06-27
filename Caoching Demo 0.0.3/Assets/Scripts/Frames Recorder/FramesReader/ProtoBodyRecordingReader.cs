/**
* @file ProtoBodyRecordingReader.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed @heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections.Generic;
using System.IO;
using HeddokoLib.heddokoProtobuff.Decoder;

namespace Assets.Scripts.Frames_Recorder.FramesReader
{
    /// <summary>
    /// Recording reader 
    /// </summary>
    public class ProtoBodyRecordingReader : BodyRecordingReaderBase
    { 
        public List<RawPacket> RawProtopackets;

        public ProtoBodyRecordingReader(string vPath)
        {
            FilePath = vPath;
        }
        /// <summary>
        /// Reads a protobuf file
        /// </summary>
        /// <param name="vFilePath"></param>
        /// <returns></returns>
        public override int ReadFile(string vFilePath)
        {
            FilePath = vFilePath;
            FileStream vInputStream = File.OpenRead(vFilePath);
            RawProtopackets = ProtoStreamDecoder.StartPacketizingFromFileStream(vInputStream, 4096);
            return RawProtopackets.Count;
        }


    }
}