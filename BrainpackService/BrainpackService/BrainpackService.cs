using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

using System.Threading;
using BrainpackService.brainpack_serial_connect;
using BrainpackService.BrainpackServer;
using BrainpackService.BrainpackServer.client;
using BrainpackService.Tools_and_Utilities;
using BrainpackService.Tools_and_Utilities.Debugging;

namespace BrainpackService
{
    public partial class BrainpackService : ServiceBase
    {
        private IBrainpackServer mServer; 
        private Thread mNetworkThread; 
        private ServerCommandRouter mServerCommandRouter;
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr vHandle, ref ServiceStatus vServiceStatus);

        public BrainpackService(string[] vArgs)
        {
            InitializeComponent();
            string vEventSourceName = DirectoryPathRepository.sEventLogSource;
            string vLogName = DirectoryPathRepository.sBrainpackLog;

            if (vArgs.Length > 0)
            {
                vEventSourceName = vArgs[0];
            }
            if (vArgs.Length > 1)
            {
                vLogName = vArgs[1];
            }

            mEventLog = new EventLog();
            if (!EventLog.SourceExists(vEventSourceName))
            {
                EventLog.CreateEventSource(vEventSourceName, vLogName);

            }

            mEventLog.Source = vEventSourceName;
            mEventLog.Log = vLogName;

        }


        public enum ServiceState
        {
            // ReSharper disable once InconsistentNaming
            SERVICE_STOPPED = 0x00000001,
            // ReSharper disable once InconsistentNaming

            SERVICE_START_PENDING = 0x00000002,            // ReSharper disable once InconsistentNaming

            SERVICE_STOP_PENDING = 0x00000003,            // ReSharper disable once InconsistentNaming

            SERVICE_RUNNING = 0x00000004,            // ReSharper disable once InconsistentNaming

            SERVICE_CONTINUE_PENDING = 0x00000005,            // ReSharper disable once InconsistentNaming

            SERVICE_PAUSE_PENDING = 0x00000006,            // ReSharper disable once InconsistentNaming

            SERVICE_PAUSED = 0x00000007,            // ReSharper disable once InconsistentNaming

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        /**
         * OnStart(string[] args)
         * @param string[] args
         * @brief Defines what happens when the service starts   
         */
        protected override void OnStart(string[] vArgs)
        {
            DebugLogger.Instance.Start();
            mServer = new AsynchronousSocketListener();//  new PersistenListener();
            UdpSender vUdpSender = new UdpSender();
            mServerCommandRouter = new ServerCommandRouter
            {
                BrainpackServer = mServer,
                UdpSender = vUdpSender
            };
            mServerCommandRouter.BeginRegistration();
            BrainpackSerialConnector.Instance.ServerCommandRouter = mServerCommandRouter;
            mServer.ServerCommandRouter = mServerCommandRouter;
            mNetworkThread = new Thread(() =>
            {
                mServer.Start();
            });
            
        

            //change the service status
            ServiceStatus vServiceStatus = new ServiceStatus();
            vServiceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            vServiceStatus.dwWaitHint = 10000;
            SetServiceStatus(ServiceHandle, ref vServiceStatus);
            //update the service state to running
            vServiceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref vServiceStatus); 

            mNetworkThread.Start();
     
        }

        private void WriteEventLogMessage(string vMsg)
        {
            mEventLog.WriteEntry(vMsg);
        }

 
        /**
        * OnStop() 
        * @brief Defines what happens when the stops starts   
        */
        protected override void OnStop()
        {
            //mBrainPackBtPoller.Stop(); // stop the connector
            mServer.Stop();
            BrainpackEventLogManager.DeRegisterEventLogMessage(WriteEventLogMessage);
            BrainpackSerialConnector.Instance.Stop();
            DebugLogger.Instance.Stop();
        }


        /**
        * OnStop() 
        * @brief Defines what happens when the service is running 
        */
        protected override void OnContinue()
        {

        }

        protected override void OnCustomCommand(int vCommand)
        {
            switch (vCommand)
            {
                case 128:
                    mEventLog.WriteEntry("Command " + vCommand + " successfully called.");
                    break;
            }
        }
    }
}
