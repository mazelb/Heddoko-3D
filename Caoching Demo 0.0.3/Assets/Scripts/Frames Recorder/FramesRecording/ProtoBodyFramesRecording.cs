// /**
// * @file ProtoBodyFramesRecording.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.Frames_Recorder.FramesReader;
using HeddokoLib.heddokoProtobuff;
using ProtoBuf;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Frames_Recorder.FramesRecording
{
    public class ProtoBodyFramesRecording : BodyFramesRecordingBase
    {

        public List<BodyProtoPacketFrame> ProtobuffPackets = new List<BodyProtoPacketFrame>();
        /// <summary>
        /// Sets the unique identifiers
        /// </summary>
        public void SetUids()
        {
            BodyRecordingGuid = Guid.NewGuid().ToString();
            BodyGuid = Guid.NewGuid().ToString();
            SuitGuid = Guid.NewGuid().ToString();
        }

     

        /// <summary>
        /// Sets the ids from a file path. Only the recording uuid is set, the BodyGuid and SuitGuid are created.
        /// </summary>
        /// <param name="vFilePath"></param>
        internal void SetUids(string vFilePath)
        {
            BodyRecordingGuid = vFilePath;
            BodyGuid = Guid.NewGuid().ToString();
            SuitGuid = Guid.NewGuid().ToString();
        }

        public override int RecordingRawFramesCount
        {
            get
            {
                var vCount = 0;
                if (ProtobuffPackets != null)
                {
                    vCount = ProtobuffPackets.Count;
                }
                return vCount;
            }
        }

        public override string FormatRevision { get; set; }
        public override string Title { get; set; }

        public override BodyRawFrameBase GetBodyRawFrameAt(int vIndex)
        {
            return ProtobuffPackets[vIndex];
        }

        /// <summary>
        /// Extracts the raw frames data from a ProtoBodyRecordingReader
        /// </summary> 
        /// <param name="vRecordingReaderbase"></param>
        public override void ExtractRawFramesData(BodyRecordingReaderBase vRecordingReaderbase)
        {
            Stopwatch vStopwatch =  new Stopwatch();
            vStopwatch.Start();
            //Serializer.PrepareSerializer<Packet>();
            ProtoBodyRecordingReader vReader = (ProtoBodyRecordingReader) vRecordingReaderbase; 
            MemoryStream vStream = new MemoryStream();
            for (int i = 0; i < vReader.RawProtopackets.Count; i++)
            {
                try
                {
                    if (vReader.RawProtopackets[i].Payload[0] == 0x04)
                    {
                        vStream.Seek(0, SeekOrigin.Begin);
                        vStream.Write(vReader.RawProtopackets[i].Payload, 1, vReader.RawProtopackets[i].PayloadSize - 1);
                        vStream.Seek(0, SeekOrigin.Begin);
                        Packet vPacket = Serializer.Deserialize<Packet>(vStream);
                        ProtobuffPackets.Add(new BodyProtoPacketFrame(vPacket, i));
                        vStream.SetLength(0);
                    }
                }
                catch (Exception vE)
                {
                    Debug.Log(vE.Message);
                }
            }
            //foreach (var vRawProtopacket in vReader.RawProtopackets)
            //{
            //    try
            //    {
            //        if (vRawProtopacket.Payload[0] == 0x04)
            //        {
            //            vStream.Write(vRawProtopacket.Payload, 1, vRawProtopacket.PayloadSize - 1);
            //            vStream.Seek(0, SeekOrigin.Begin);
            //            Packet vPacket = Serializer.Deserialize<Packet>(vStream);
            //            ProtobuffPackets.Add(new BodyProtoPacketFrame(vPacket));
            //        }
            //    }
            //    catch (Exception vE)
            //    {
            //        Debug.Log(vE.Message);
            //    }
               
            //}
            vStopwatch.Stop();
            Debug.Log("completed after "+(vStopwatch.ElapsedMilliseconds/1000 )+ " seconds");
        }
    }
}