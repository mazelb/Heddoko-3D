
/** 
* @file PersistentConnection.cs
* @brief Contains the PersistentConnection class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Net.Sockets;
using System.Threading; 
namespace Assets.Scripts.Communication.Communicators
{
    /// <summary>
    /// A persistent connection to the brainpack service
    /// </summary>
    public class PersistentConnection
    {
        private const int sTimeout = 30000;
        private Semaphore mSemaphore = new Semaphore(1, 1);
        private TcpClient mClient;
        public PersistentConnection()
        {
            mClient = new TcpClient();
        }

    }
}