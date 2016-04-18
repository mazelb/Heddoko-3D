using System;
using System.Collections.Generic;
using System.Text;

namespace HeddokoLib.utils
{
    /// <summary>
    ///Todo: create a generic state machine
    /// </summary>
    public class GenericStateMachine
    {
        
    }
    public enum ProcessState
    {
        Inactive,
        Active,
        Pause,
        Terminated
    }
    public enum CommandType
    {
        Begin,
        End,
        Pause,
        Resume,
        Exit
    }   
}
