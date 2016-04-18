/** 
* @file Command.cs
* @brief Contains the CommandDelegate class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
 
*/ 
using System.Collections.Generic;

namespace HeddokoLib.networking
{

    public delegate void CommandDelegate(object vSender, object vArgs);
    /**
    * Command class 
    * @brief Command class, this class's responsibility is to register commands and delegate responsibilities to other depending on the command type
    * process and register commands. 
    */
    public class Command
    {
        static Dictionary<string, CommandDelegate> smCommands;
        Dictionary<string, CommandDelegate> mCommands;
         public Command()
        {
            mCommands = new Dictionary<string, CommandDelegate>();
        }

        static Command()
        {
            smCommands = new Dictionary<string, CommandDelegate>();
        }
        /**
         * ProcessCommand(object vSender, HeddokoPacket vPacket
         * @param vSender: the vSender object, vPacket : the HeddokoPacket that was was sent
         * @brief will try to invoke a CommandDelegate based on the the passed in packet
         * @note will not invoke a CommandDelegate if the packet contains an invalid command
         * @return bool that indicates that the command delegate was succesfully invoked
         */
        public static bool ProcessCommand(object vSender, HeddokoPacket vPacket)
        {
            if (smCommands.ContainsKey(vPacket.Command))
            {
                smCommands[vPacket.Command].DynamicInvoke(vSender, vPacket);
            }
            else
            {
                return false;
            }
            return true;
        }
        /**
         * RegisterCommand(uint vCommand, CommandDelegate vFunc)
         * @param vCommand: The command type, CommandDelegate vFunc: the method that will be invoked on this command
         * @brief Safely registers a method into the dictionary
         * @note will not register a command if the passed parameter already exists in the class dictionary
         * @return bool that indicates that the command delegate was succesfully registered
         */

        public static bool RegisterCommand(string vCommand, CommandDelegate vFunc)
        {
            if (!smCommands.ContainsKey(vCommand))
            {
                smCommands.Add(vCommand, vFunc);
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
        * Process(object vSender, HeddokoPacket vPacket
        * @param vSender: the vSender object, vPacket : the HeddokoPacket that was was sent
        * @brief will try to invoke a CommandDelegate based on the the passed in packet
        * @note will not invoke a CommandDelegate if the packet contains an invalid command
        * @return bool that indicates that the command delegate was succesfully invoked
        */
        public bool  Process(object vSender, HeddokoPacket vPacket)
        {
            if (mCommands.ContainsKey(vPacket.Command))
            {
                mCommands[vPacket.Command].DynamicInvoke(vSender, vPacket);
                return true;
            }
            return false;
        }
        /**
    * RegisterCommand(uint vCommand, CommandDelegate vFunc)
    * @param vCommand: The command type, CommandDelegate vFunc: the method that will be invoked on this command
    * @brief Safely registers a method into the dictionary
    * @note will not register a command if the passed parameter already exists in the class dictionary
    * @return bool that indicates that the command delegate was succesfully registered
    */

        public bool Register(string vCommand, CommandDelegate vFunc)
        {
            if (!mCommands.ContainsKey(vCommand))
            {
                mCommands.Add(vCommand, vFunc);
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
        * Clear 
        * @brief Safely clears internal list of command delegates
        */
        public void Clear()
        {
            if (smCommands != null)
            {
                smCommands.Clear();
            }
            if (mCommands != null)
            {
                mCommands.Clear();
            }
        }


    }
    
}
