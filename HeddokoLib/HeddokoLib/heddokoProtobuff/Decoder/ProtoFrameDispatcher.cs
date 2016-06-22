// /**
// * @file ProtoFrameDispatcher.cs
// * @brief Contains the  ProtoFrameDispatcher
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic; 

namespace HeddokoLib.heddokoProtobuff.Decoder
{
    public delegate void ProtoFrameDelegate(object vSender, object vArgs);

    /// <summary>
    /// A protoframe dispatcher: Contains a predefined set of Brainpack commands, as well as a list of actions for the set of protobuf messages
    /// </summary>
    public class ProtoFrameDispatcher
    {
        internal Dispatch<PacketType> mProtoPacketDispatcher = new Dispatch<PacketType>();
 
    /// <summary>
        /// Registers a protobuf packet command
        /// </summary>
        /// <param name="vPacketType">The packet type</param>
        /// <param name="vDelegate">the delegate to register</param> 
        public void AddProtobuffCommand(PacketType vPacketType, ProtoFrameDelegate vDelegate)
        {
             mProtoPacketDispatcher.Add(vPacketType, vDelegate);
        }
        /// <summary>
        ///// Registers a Brainpack command Id
        ///// </summary>
        ///// <param name="vCommandId"></param>
        ///// <param name="vDelegate"></param>
        //public void AddBrainpackCommand(BrainpackCommandId vCommandId, ProtoFrameDelegate vDelegate)
        //{
        //    mBrainpackCommandDispatcher.Add(vCommandId, vDelegate);

        //}

        ///// <summary>
        ///// Processes a command Id
        ///// </summary>
        ///// <param name="vCommandId"></param>
        ///// <param name="vSender"></param>
        ///// <param name="vArgs"></param>
        ///// <returns>was the command processed sucessfully?</returns>
        //public bool Process(BrainpackCommandId vCommandId, object vSender, object vArgs)
        //{
        //    return mBrainpackCommandDispatcher.Process(vCommandId, vSender, vArgs);
        //}

        /// <summary>
        /// Process a packet type
        /// </summary>
        /// <param name="vPacketType"></param>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        /// <returns>was the packet type processed sucessfully?</returns>
        public bool Process(PacketType vPacketType, object vSender, object vArgs)
        {
            return mProtoPacketDispatcher.Process(vPacketType, vSender, vArgs);
        }


        internal class Dispatch<T1>
        {
            internal Dictionary<Enum, ProtoFrameDelegate> DispatchCollection = new Dictionary<Enum, ProtoFrameDelegate>();

            /// <summary>
            /// Attempts to register a dispatch command. If it already exists, then false is returned
            /// </summary>
            /// <param name="vEnum"></param>
            /// <param name="vT2"></param>
            /// <returns>Was the registration succesful?</returns>
            public void Add(Enum vEnum, ProtoFrameDelegate vT2)
            {
                ProtoFrameDelegate vDel;
                DispatchCollection.TryGetValue(vEnum, out vDel);
                DispatchCollection[vEnum] = vDel + vT2;
            }

            /// <summary>
            /// Removes a given protoframe delegate event handler
            /// </summary>
            /// <param name="vEnum"></param>
            /// <param name="vProtoFrameDelegate"></param> 
            public void Remove(Enum vEnum, ProtoFrameDelegate vProtoFrameDelegate)
            {

                ProtoFrameDelegate vDel;
                DispatchCollection.TryGetValue(vEnum, out vDel);
                DispatchCollection[vEnum] = vDel - vProtoFrameDelegate;
                if (vDel != null)
                {
                    DispatchCollection[vEnum] = vDel;
                }
                else
                {
                    DispatchCollection.Remove(vEnum);
                }

            }
            /// <summary>
            /// Process at the given key
            /// </summary>
            /// <param name="vEnum"></param>
            /// <param name="vSender"></param>
            /// <param name="vArgs"></param>
            /// <returns></returns>
            public bool Process(Enum vEnum,object vSender, object vArgs)
            {
                if (DispatchCollection.ContainsKey(vEnum))
                {
                    DispatchCollection[vEnum].DynamicInvoke(vSender, vArgs);
                    return true;
                }
                return false;
            }
        }
 
    }


}