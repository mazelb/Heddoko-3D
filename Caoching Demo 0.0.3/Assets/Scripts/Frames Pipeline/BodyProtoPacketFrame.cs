/**
* @file BodyProtoPacketFrame.cs
* @brief Contains the BodyProtoPacketFrame
* @author Mohammed Haider( 
* @date 06 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections.Generic;
 using HeddokoLib.body_pipeline;
using HeddokoLib.heddokoProtobuff;

namespace Assets.Scripts.Frames_Pipeline
{
    /// <summary>
    /// A raw body frame received from a protocol buffer packet
    /// </summary>
    public class BodyProtoPacketFrame : BodyRawFrameBase
    {
        public Packet Packet;

        public BodyProtoPacketFrame(Packet vPacket)
        {
            Packet = vPacket;
        }
 
    }
}