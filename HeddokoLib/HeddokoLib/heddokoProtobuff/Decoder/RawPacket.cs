/**
* @file RawPacket.cs
* @brief Contains the RawPacket class
* @author Mohammed Haider( mohammed@heddoko.com )
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;

namespace HeddokoLib.heddokoProtobuff.Decoder
{
    /// <summary>
    /// A raw vPacket received from a file stream
    /// </summary>
    public class RawPacket
    {
        const byte StartByte = 0xDE;
        const byte EscapeByte = 0xDF;
        const byte EscapedByteOffset = 0x10;
        const int MaxPacketSize = 2000;
        byte[] mPayload;
        ushort mPayloadSize;
        bool mPacketComplete;
        bool mEscapeFlag;
        ushort mBytesReceived;
        byte[] mRawPacketBytes;
        int mRawPacketBytesIndex;

        /// <summary>
        /// The RawPacket's payload
        /// </summary>
        public byte[] Payload
        {
            get
            {
                return mPayload;
            }
            private set { mPayload = value; }
        }

        /// <summary>
        /// Total payload size
        /// </summary>
        public ushort PayloadSize
        {
            get
            {
                return mPayloadSize;
            }
            private set
            {
                mPayloadSize = value;
            }
        }

        /// <summary>
        /// Number of bytes received
        /// </summary>
        public ushort BytesReceived
        {
            get
            {
                return mBytesReceived;
            }
            private set
            {
                mBytesReceived = value;
            }
        }

        public bool EscapeFlag
        {
            get { return mEscapeFlag; }
            private set { mEscapeFlag = value; }
        }

        public bool PacketComplete
        {
            get { return mPacketComplete; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RawPacket()
        {
            Payload = new byte[MaxPacketSize];
            mPayloadSize = 0;
            mPacketComplete = false;
            mEscapeFlag = false;
            mBytesReceived = 0;
            mRawPacketBytes = new byte[MaxPacketSize];
        }
        /// <summary>
        /// Constructor that accepts a byte array and the size of the payload bytes
        /// </summary>
        /// <param name="vByteArr">Byte array</param>
        /// <param name="vPayloadBytesSize">Payload byte size</param>
        public RawPacket(byte[] vByteArr, UInt16 vPayloadBytesSize)
        {
            mPayload = vByteArr;
            mPayloadSize = vPayloadBytesSize;
            mPacketComplete = false;
            mBytesReceived = 0;
            mRawPacketBytes = new byte[MaxPacketSize];
            mRawPacketBytesIndex = 0;
        }

        /// <summary>
        /// Deep copy constructor
        /// </summary>
        /// <param name="vPacket">The RawPacket to copy</param>
        public RawPacket(RawPacket vPacket)
        {
            mPayload = new byte[MaxPacketSize];
            Buffer.BlockCopy(vPacket.mPayload, 0, Payload, 0, vPacket.PayloadSize);
            PayloadSize = vPacket.PayloadSize;
            mPacketComplete = false;
            mBytesReceived = 0;
            mRawPacketBytes = new byte[MaxPacketSize];
            mRawPacketBytesIndex = 0;
        }

        /// <summary>
        /// Reset the packet. 
        /// </summary>
        /// <remarks>Does not clear out the payload, just sets the pointer to zero. If you need to clear, call the <see cref="Clear"/></remarks>
        public void ResetPacket()
        {
            PayloadSize = 0;
            mPacketComplete = false;
            mEscapeFlag = false;
            BytesReceived = 0;
        }
        /// <summary>
        /// Performs a deep clear of the packet
        /// </summary>
        public void Clear()
        {
            ResetPacket();
            Payload = new byte[MaxPacketSize];
        }

        /// <summary>
        /// Performs a deep copy of the current RawPacket
        /// </summary>
        /// <returns>A copy of the raw packet object</returns>
        public RawPacket DeepCopy()
        {
            RawPacket vOther = (RawPacket)MemberwiseClone();
            vOther.Payload = Payload;
            Buffer.BlockCopy(vOther.Payload, 0, Payload, 0, PayloadSize);
            vOther.PayloadSize = PayloadSize;
            return vOther;
        }

        /// <summary>
        /// Processes an vIncoming byte
        /// </summary>
        /// <remarks>When a new packet has been found, the packet is cleared.</remarks>
        /// <exception cref="PacketErrorException">If the packet has an error, an exception will be thrown </exception> 
        /// <param name="vIncoming">Incoming packet to process</param>
        /// <returns></returns>
        public PacketStatus ProcessByte(byte vIncoming)
        {
            PacketStatus vStatus = PacketStatus.Processing;
            //if byte is start byte
            if (vIncoming == StartByte)
            {
                if (BytesReceived > 0)
                {
                    vStatus = PacketStatus.PacketError;
                }
                else
                {
                    vStatus = PacketStatus.NewPacketDetected;

                }
                //reset the counts and everything for reception of the packet
                BytesReceived = 0;
                EscapeFlag = false;
                PayloadSize = 0;
                return vStatus;

            }
            //if byte is escape byte
            if (vIncoming == EscapeByte)
            {
                //set escape flag, so the next byte is properly offset. 
                EscapeFlag = true;
                return vStatus;
            }

            //if escape byte flag is set
            if (EscapeFlag )
            {
                //un-escape the byte and process it as any other byte.
                vIncoming = (byte)(vIncoming - EscapedByteOffset);
                //unset the flag
                EscapeFlag = false;
            }

            //if receive count is  0 
            if (BytesReceived == 0)
            {
                //this is the first byte of the payload size		
                //copy byte to LSB of the payload size
                PayloadSize |= vIncoming;
                //increment received count
                BytesReceived++;
            }
            else if (BytesReceived == 1)
            {
                //this is the second byte of the payload size
                //copy byte to MSB of the payload size
                PayloadSize |= (ushort)(vIncoming << 8);
                //increment received count
                BytesReceived++;
            }
            else
            {
                //copy byte to payload at point receivedBytes - 2
                if ((BytesReceived - 2) >= Payload.Length)
                {
                    vStatus = PacketStatus.PacketError;
                }
                else
                {
                    Payload[BytesReceived++ - 2] = vIncoming;
                    //check if we received the whole packet.
                    if (BytesReceived == (PayloadSize + 2))
                    {
                        //process the packet
                        vStatus = PacketStatus.PacketComplete;
                    }
                }
              

            }
            return vStatus;
        }

        /// <summary>
        /// Inserts a byte to the current raw packet payload
        /// </summary>
        /// <param name="vRawByte">The byte to insert</param>
        void InsertByteToRawPacket(byte vRawByte)
        {
            if (vRawByte == EscapeByte || vRawByte == StartByte)
            {
                mRawPacketBytes[mRawPacketBytesIndex++] = EscapeByte;
                mRawPacketBytes[mRawPacketBytesIndex++] = (byte)(vRawByte + EscapedByteOffset);
            }
            else
            {
                mRawPacketBytes[mRawPacketBytesIndex++] = vRawByte;
            }
        }
        /// <summary>
        /// Get the Byte Array of the raw packet. Sets the start byte and the payload size
        /// </summary>
        /// <param name="vRawSize">sets the Rawsize of the given param</param>
        /// <returns></returns>
        public byte[] GetRawPacketByteArray(out int vRawSize)
        {
            mRawPacketBytes[mRawPacketBytesIndex++] = StartByte;
            InsertByteToRawPacket((byte)(PayloadSize & 0x00ff));
            InsertByteToRawPacket((byte)((PayloadSize >> 8) & 0x00ff));

            for (int vI = 0; vI < PayloadSize; vI++)
            {
                InsertByteToRawPacket(Payload[vI]);
            }

            vRawSize = mRawPacketBytesIndex;
            return mRawPacketBytes;
        }
    }

    /// <summary>
    /// An exception thrown when a packet error has occured
    /// </summary>
    public class PacketErrorException : Exception
    {
        public PacketErrorException(string vMsg) : base(vMsg)
        {
        }
    }

    public enum PacketStatus { PacketComplete, Processing, NewPacketDetected , PacketError}
}