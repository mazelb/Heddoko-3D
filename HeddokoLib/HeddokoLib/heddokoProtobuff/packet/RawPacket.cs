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
        const UInt32 MaxPacketSize = 2000;
        byte[] mPayload;
        UInt16 mPayloadSize;
        bool mPacketComplete;
        bool mEscapeFlag;
        UInt16 mBytesReceived;
        byte[] mRawPacketBytes;
        UInt16 mRawPacketBytesIndex;
        UInt16 rawPacketBytesIndex;
        public RawPacket()
        {
            mPayload = new byte[MaxPacketSize];
            mPayloadSize = 0;
            mPacketComplete = false;
            mEscapeFlag = false;
            mBytesReceived = 0;
            mRawPacketBytes = new byte[MaxPacketSize];
        }
        public RawPacket(byte[] vPayloadBytes, UInt16 vPayloadBytesSize)
        {
            mPayload = vPayloadBytes;
            mPayloadSize = vPayloadBytesSize;
            mPacketComplete = false;
            mBytesReceived = 0;
            mRawPacketBytes = new byte[MaxPacketSize];
            rawPacketBytesIndex = 0;
        }
        public RawPacket(RawPacket vPacket)
        {
            mPayload = new byte[MaxPacketSize];
            Buffer.BlockCopy(vPacket.mPayload, 0, mPayload, 0, vPacket.mPayloadSize);
            mPayloadSize = vPacket.mPayloadSize;
            mPacketComplete = false;
            mBytesReceived = 0;
            mRawPacketBytes = new byte[MaxPacketSize];
            rawPacketBytesIndex = 0;
        }

        public void ResetPacket()
        {
            mPayloadSize = 0;
            mPacketComplete = false;
            mEscapeFlag = false;
            mBytesReceived = 0;
        }
        public RawPacket DeepCopy()
        {
            RawPacket vOther = (RawPacket)this.MemberwiseClone();
            vOther.mPayload = this.mPayload;
            Buffer.BlockCopy(vOther.mPayload, 0, this.mPayload, 0, this.mPayloadSize);
            vOther.mPayloadSize = this.mPayloadSize;
            return vOther;
        }

        void AddByteToRawPacket(byte vRawByte)
        {
            if (vRawByte == EscapeByte || vRawByte == StartByte)
            {
                mRawPacketBytes[rawPacketBytesIndex++] = EscapeByte;
                mRawPacketBytes[rawPacketBytesIndex++] = (byte)((int)vRawByte + (int)EscapedByteOffset);
            }
            else
            {
                mRawPacketBytes[rawPacketBytesIndex++] = vRawByte;
            }
        }

        public byte[] CreateRawPacket(ref UInt16 vRawSize)
        {
            vRawSize = 0;
            mRawPacketBytes[rawPacketBytesIndex++] = StartByte;
            AddByteToRawPacket((byte)(mPayloadSize & 0x00ff));
            AddByteToRawPacket((byte)((mPayloadSize >> 8) & 0x00ff));

            for (int vI = 0; vI < mPayloadSize; vI++)
            {
                AddByteToRawPacket(mPayload[vI]);
            }
            //return the size of the raw bytes. 
            vRawSize = rawPacketBytesIndex;
            return mRawPacketBytes;

        }
    }
}