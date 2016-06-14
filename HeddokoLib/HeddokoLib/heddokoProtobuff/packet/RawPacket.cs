// /**
// * @file RawPacket.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @ heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;

namespace HeddokoLib.heddokoProtobuff.packet
{
    public class RawPacket
    {
        const byte StartByte = 0xDE;
        const byte EscapeByte = 0xDF;
        const byte EscapedByteOffset = 0x10;
        const UInt32 maxPacketSize = 2000;
        byte[] payload;
        UInt16 payloadSize;
        bool packetComplete;
        bool escapeFlag;
        UInt16 bytesReceived;
        byte[] rawPacketBytes;
        UInt16 mRawPacketBytesIndex;
        UInt16 rawPacketBytesIndex;
        public RawPacket()
        {
            payload = new byte[maxPacketSize];
            payloadSize = 0;
            packetComplete = false;
            escapeFlag = false;
            bytesReceived = 0;
            rawPacketBytes = new byte[maxPacketSize];
        }
        public RawPacket(byte[] payloadBytes, UInt16 payloadBytesSize)
        {
            payload = payloadBytes;
            payloadSize = payloadBytesSize;
            packetComplete = false;
            bytesReceived = 0;
            rawPacketBytes = new byte[maxPacketSize];
            rawPacketBytesIndex = 0;
        }
        public RawPacket(RawPacket packet)
        {
            payload = new byte[maxPacketSize];
            Buffer.BlockCopy(packet.payload, 0, this.payload, 0, packet.payloadSize);
            payloadSize = packet.payloadSize;
            packetComplete = false;
            bytesReceived = 0;
            rawPacketBytes = new byte[maxPacketSize];
            rawPacketBytesIndex = 0;
        }

        public void resetPacket()
        {
            payloadSize = 0;
            packetComplete = false;
            escapeFlag = false;
            bytesReceived = 0;
        }
        public RawPacket DeepCopy()
        {
            RawPacket other = (RawPacket)this.MemberwiseClone();
            other.payload = this.payload;
            Buffer.BlockCopy(other.payload, 0, this.payload, 0, this.payloadSize);
            other.payloadSize = this.payloadSize;
            return other;
        }

        void addByteToRawPacket(byte rawByte)
        {
            if (rawByte == EscapeByte || rawByte == StartByte)
            {
                rawPacketBytes[rawPacketBytesIndex++] = EscapeByte;
                rawPacketBytes[rawPacketBytesIndex++] = (byte)((int)rawByte + (int)EscapedByteOffset);
            }
            else
            {
                rawPacketBytes[rawPacketBytesIndex++] = rawByte;
            }
        }

        public byte[] createRawPacket(ref UInt16 rawSize)
        {
            rawSize = 0;
            rawPacketBytes[rawPacketBytesIndex++] = StartByte;
            addByteToRawPacket((byte)(payloadSize & 0x00ff));
            addByteToRawPacket((byte)((payloadSize >> 8) & 0x00ff));

            for (int i = 0; i < payloadSize; i++)
            {
                addByteToRawPacket(payload[i]);
            }
            //return the size of the raw bytes. 
            rawSize = rawPacketBytesIndex;
            return rawPacketBytes;

        }
    }
}