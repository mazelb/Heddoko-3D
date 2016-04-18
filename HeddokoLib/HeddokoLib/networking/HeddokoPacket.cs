/** 
* @file HeddokoPacket.cs
* @brief Contains the HeddokoPacket class, the Command Type  
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using System;
using System.Net.Sockets; 
using System.Text;
using HeddokoLib.utils;

namespace HeddokoLib.networking
{
    /**
    * HeddokoPacket   
    * @brief HeddokoPacket class, contains a packet that is to be decoded by a client or server client. 
    * Clients are responsible for handling the commands that are used in the HeddokoPacket
    * @note something 
    */
    public class HeddokoPacket
    {
       
        public byte[]  Payload;
        public string Command;
        public Socket Socket;

        public HeddokoPacket()
        {
            Payload = new byte[1024];
            
        }
        public HeddokoPacket(string vCommand, byte[] vPayload)
        {
            Command = vCommand;
           Payload = vPayload;
        }

        public HeddokoPacket(string vCommand, string vPacket)
        {
            Command = vCommand;
            Payload = PacketSetting.Encoding.GetBytes(vPacket);
        }

   
        /// <summary>
        /// get the current payload and unwrap it into its command and payload components
        /// </summary>
        /// <returns></returns>
         
        public HeddokoPacket(byte[] dataBytes, int vCommandLength)
        {
            Command = HeddokoCommands.ExtractCommandFromBytes(0, vCommandLength, dataBytes); //extract the command from provided byte array
            int vCopyLength = dataBytes.Length - vCommandLength; //Copy the payload into the current payload
            if (vCopyLength <= 0)
            {
                Payload = new byte[0];
            }
            else
            {
                Payload = new byte[vCopyLength];
                int vStartIndex = vCommandLength;
                int vCopyIdx = 0;
                Array.Copy(dataBytes, vStartIndex, Payload, 0, vCopyLength); 
            }
        }
 
        /// <summary>
        /// Unwrap a byte array that is in the wrapped HeddokoPacket format
        /// </summary>
        /// <param name="vBytes">The byte array HeddokoPacket format </param>
        /// <returns> The unwrapped byte array </returns>
        public static string Unwrap(byte[] vBytes)
        {
            StringBuilder vSb = new StringBuilder();
            vSb.Append(PacketSetting.Encoding.GetString(vBytes));
            string vUnwrappedString = vSb.ToString();
            vUnwrappedString= vUnwrappedString.Replace(PacketSetting.EndOfPacketDelim, null);

            vUnwrappedString = vUnwrappedString.Replace(PacketSetting.EndOfCommandDelim + "", null);
            vUnwrappedString = vUnwrappedString.Replace("\r", null);
            vUnwrappedString = vUnwrappedString.Replace("\n", null); 
            vUnwrappedString = vUnwrappedString.TrimEnd((char) 0);
            return vUnwrappedString;
        }
        /// <summary>
        /// Unwrap a byte array that is in the wrapped HeddokoPacket format
        /// </summary>
        /// <param name="vBytes">The byte array HeddokoPacket format </param>
        /// <returns> The unwrapped byte array </returns>
        public static string Unwrap(string vWrappedString)
        {
            string vUnwrappedString  =vWrappedString;
            vUnwrappedString = vUnwrappedString.Replace(PacketSetting.EndOfPacketDelim, null); 
            vUnwrappedString = vUnwrappedString.Replace(PacketSetting.EndOfCommandDelim + "", null);
            vUnwrappedString = vUnwrappedString.Replace("\r", null);
            vUnwrappedString = vUnwrappedString.Replace("\n", null);
            vUnwrappedString = vUnwrappedString.TrimEnd((char)0);
            return vUnwrappedString;
        }

        /// <summary>
        /// Unwrap a byte array that is in the wrapped HeddokoPacket format
        /// </summary>
        /// <param name="vBytes">The byte array HeddokoPacket format </param>
        /// <returns> The unwrapped byte array </returns>
        public static string UnwrapRemovedCommand(string vWrappedString)
        {
            string vUnwrappedString = vWrappedString;
            vUnwrappedString = vUnwrappedString.Replace(PacketSetting.EndOfPacketDelim, null);
            vUnwrappedString = vUnwrappedString.Replace(PacketSetting.EndOfCommandDelim+"", null);
            vUnwrappedString = vUnwrappedString.Replace("\r", null);
            vUnwrappedString = vUnwrappedString.Replace("\n", null);
            vUnwrappedString = vUnwrappedString.TrimEnd((char)0);
            return vUnwrappedString;
        }
        /// <summary>
        /// Wraps a HeddokoPacket for transmission
        /// </summary>
        /// <param name="vPacket">The heddoko packet that needs to be wrapped </param>
        /// <returns></returns>
        public static string Wrap(HeddokoPacket vPacket)
        {
            StringBuilder vSb = new StringBuilder();
            vSb.Append(vPacket.Command);
            vSb.Append(PacketSetting.EndOfCommandDelim);
            vSb.Append(PacketSetting.Encoding.GetString(vPacket.Payload));
            vSb.Append(PacketSetting.EndOfPacketDelim);
            return vSb.ToString();
        }
        /// <summary>
        /// Wraps a comman and payload for transmission
        /// </summary>
        /// <param name="vCommand">The command</param>
        /// <param name="vPayload">The payload</param>
        /// <returns></returns>
        public static string Wrap(string vCommand, string vPayload)
        {
            StringBuilder vSb = new StringBuilder();
            vSb.Append(vCommand);
            vSb.Append(PacketSetting.EndOfCommandDelim);
            vSb.Append(vPayload);
            vSb.Append(PacketSetting.EndOfPacketDelim);
            return vSb.ToString();
        }
    }


}
