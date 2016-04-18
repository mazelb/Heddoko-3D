using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoLib.networking;


namespace Assets.Scripts.Communication.Communicators
{
    /// <summary>
    /// A Listener that retrieves incoming datagrams from the brainpack service
    /// </summary>
    public class UdpListener
    {
        private UdpClient mClient;
        private PacketCommandRouter mCommandRouter;
        private Thread mThread;
        private const int gListeningPort = 11000;
        private bool mDone;
        private IPEndPoint mEndPoint;
        public bool Start()
        {
            mDone = false;
            mClient = new UdpClient(gListeningPort);
            IPHostEntry vIpHostEntry = Dns.Resolve("localhost");
            IPAddress vIpAddress = vIpHostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            mEndPoint = new IPEndPoint(vIpAddress, 11000);
            mThread = new Thread(ThreadedJob);
            mThread.Start();
            return true;
        }

        /// <summary>
        /// The threaded job that will listen
        /// </summary>
        private void ThreadedJob()
        {
            DebugLogger.Instance.LogMessage(LogType.SocketClientSend, "Udp Listener beginning job");
            byte[] vByteArray;
            while (!mDone)
            {
                try
                {
                    vByteArray = mClient.Receive(ref mEndPoint);
                    //convert to a Heddoko packet
                    HeddokoPacket vHPacket = new HeddokoPacket(vByteArray, PacketSetting.PacketCommandSize);
                    PacketCommandRouter.Instance.Process(this, vHPacket);
                }
                catch (Exception vE)
                {
                    string vMsg = "Exception caught in UDP listener" + vE+"\n"+vE.InnerException;
                    DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);
                    UnityEngine.Debug.Log(vMsg);
                }
            }
            
        }

        /// <summary>
        /// Stops the listener
        /// </summary>
        public void Stop()
        {
            mDone = true;
            try
            {
                mClient.Close();
            }
            catch (Exception vE)
            {
                string vMsg = "Exception caught in UDP listener while closing" + vE + "\n" + vE.InnerException;
                DebugLogger.Instance.LogMessage(LogType.SocketClientError, vMsg);

            }

        }

    }
}