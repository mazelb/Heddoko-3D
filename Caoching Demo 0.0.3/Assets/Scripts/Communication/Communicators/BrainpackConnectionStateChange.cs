// /**
// * @file BrainpackConnectionStateChange.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace Assets.Scripts.Communication.Communicators
{

    /// <summary>
    /// structure indicating a Brainpack connection State change
    /// </summary>
    public class BrainpackConnectionStateChange
    {
        public BrainpackConnectionState OldState;
        public BrainpackConnectionState NewState;
    }

    public enum BrainpackConnectionState
    {
        Disconnected,
        Connected,
        Connecting
    }
}