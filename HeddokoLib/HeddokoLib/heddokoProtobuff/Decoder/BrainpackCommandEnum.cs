// /**
// * @file BrainpackCommandEnum.cs
// * @brief Contains the BrainpackCommandEnum
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace HeddokoLib.heddokoProtobuff.Decoder
{
   public enum BrainpackCommandId
    {
        Update = 0x11,
        GetFrame,
        GetFrameResp,
        SetupMode,
        ButtonPress,
        SetImuId,
        SetImuIdResp,
        GetStatus,
        GetStatusResp
    };
}