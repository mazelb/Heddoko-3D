// /**
// * @file ProtobuffRouter.cs
// * @brief Contains the ProtobuffDispatchRouter class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using heddoko;
using HeddokoLib.adt;

namespace HeddokoLib.heddokoProtobuff.Decoder
{
    /// <summary>
    /// A protobuf dispatching router. receives protbufs and dispatches events according to the message type. 
    /// </summary>
    public class ProtobuffDispatchRouter
    {
        private ProtoFrameDispatcher mDispatcher = new ProtoFrameDispatcher();
        private ProtoFrameDelegate mEvent;
        /// <summary>
        /// sets up the the dispatch router
        /// <remarks>uses reflection</remarks>
        /// </summary>
        public void Init()
        {
            var vPacketKeys = Enum.GetValues(typeof(PacketType));
            var vBrainpackCommandId = Enum.GetValues(typeof (BrainpackCommandId));
            foreach (var vPacketKey in vPacketKeys)
            {
                PacketType vKey = (PacketType)vPacketKey;
                //add a placeholder lamda
                mDispatcher.AddProtobuffCommand(vKey, (vX, vY) => { });
            }
            foreach (var vCommandKey in vBrainpackCommandId)
            {
                BrainpackCommandId vKey = (BrainpackCommandId)vCommandKey;
                //add a placeholder lamda
                mDispatcher.AddBrainpackCommand(vKey, (vX, vY) => { });
            }
        }

        public ProtoFrameDelegate GetEventHandler(PacketType vPacketType)
        {
            return mDispatcher.mProtoPacketDispatcher.DispatchCollection[vPacketType];
        }
        public void Add(PacketType vPacketType, ProtoFrameDelegate vDel)
        {
            mDispatcher.AddProtobuffCommand(vPacketType, vDel);

        }
        public void Add(BrainpackCommandId vBrainpackCommandId, ProtoFrameDelegate vDel)
        {
            mDispatcher.AddBrainpackCommand(vBrainpackCommandId, vDel);

        }

        public void RemovePacketTypeEvent(PacketType vPacketType, ProtoFrameDelegate vDel)
        {
            var vDelegate = GetEventHandler(vPacketType);
            Delegate.Remove(vDelegate, vDel);
        }

        public bool Process(PacketType vType, object vSender, object vArgs)
        {
            return mDispatcher.Process(vType, vSender, vArgs);
        }

        public bool Process(BrainpackCommandId vType, object vSender, object vArgs)
        {
            return mDispatcher.Process(vType, vSender, vArgs);
        }
        /// <summary>
        /// Removes an event handler from the dispatcher with respect to the BrainpackCommandId passed in
        /// </summary>
        /// <param name="vCommandId"></param>
        /// <returns></returns>
        public bool RemoveEventHandler(BrainpackCommandId vCommandId)
        {
            return false;
        }

        public bool RemoveEventHandler(PacketType vPacketType)
        {
            return false;

        }
    }
}