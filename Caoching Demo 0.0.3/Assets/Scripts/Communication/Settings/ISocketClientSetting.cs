using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/** 
* @file ISocketClientSetting.cs
* @brief Contains the ISocketClientSetting  interface
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/
namespace Assets.Scripts.Communication
{
    /**
    * ISocketClientSetting interface 
    * @brief Provides an interface that can be used for Socket Client settings
    * @note something 
    */

    public interface ISocketClientSetting
    {
        /// <summary>
        /// The name of the connection
        /// </summary>
        /**
        * ConnectionName 
        * @brief Property: returns the connection name
        * @return: the server's ip
        */
        string ConnectionName { get; set; }
        /// <summary>
        ///  The interested server's ip
        /// </summary>
        /**
        * ConnectionName 
        * @brief Property: returns the ServerIP 
        * @return:  The server's ip  
        */
        string ServerIp { get; set; }
        /// <summary>
        /// The port number to connect to 
        /// </summary>
        /**
        * ConnectionName 
        * @brief Property: returns the Server port
        * @return:  The server's port
        */
        int Port { get; set; }
    }
}
