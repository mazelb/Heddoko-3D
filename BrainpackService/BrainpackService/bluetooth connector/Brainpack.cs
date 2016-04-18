using System; 
using System.Net.Sockets;
 using System.Text;
using System.Threading;
 using HeddokoLib.adt;
 
using BrainpackService.Tools_and_Utilities;
 
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BrainpackService.bluetooth_connector
{
    public class Brainpack
    {
        private BluetoothClient mCurrentBrainpack { get; set; }
        private BluetoothDeviceInfo mCurrentBrainpackInfo;
        private static Guid sgGuidService = BluetoothService.SerialPort; //guid required to communicate with bluetooth over serial port
        private Thread mStreamingThread;
        private bool mContinueReading;
        private const int sgStreamingTimeout = 1000; //timeout in ms 
        private const int sgBufferSize = 8096;
        private const int sgPacketByteSize = 5*173;//try to retreive 5 times the packet length from the stream
        //private const int sgPacketCheckLength = 200;
        private NetworkStream mBrainpackNetworkStream;
        private object mDisconnectFlagLock = new object();  
        public CircularQueue<byte[]> OutboundBuffer{  get;  set;  } = new CircularQueue<byte[]>(sgBufferSize,true);


        public delegate void BrainpackDisconnected();

        public event BrainpackDisconnected BpDisconnected;
         
        public byte[] DataReceived { get; set; } = new byte[sgBufferSize];
        /**
      * DisconnectBrainpack ( )
      * @brief Will attempt to disconnect the current brainpack
      * @return Braipack was successfully disconnected 
      */
        public void DisconnectBrainpack()
        {
            lock (mDisconnectFlagLock)
            {
                mContinueReading = false;
            }
            if (mCurrentBrainpack.Connected)
            {
                mCurrentBrainpack.GetStream().Flush();
                mCurrentBrainpack.Close();
                //set the current buffer to null
                OutboundBuffer = new CircularQueue<byte[]>(sgBufferSize,true);
            }
            //in case the object thread is running, tell it to stop

            BpDisconnected?.Invoke();
        }
        /**
        * Start ()
        * @brief Instantiates a new thread which will connect to the brainpack and start reading
        */
        public void Start()
        {
            mContinueReading = true;
            mStreamingThread = new Thread(ConnectToDeviceThreadedFunc);
            mStreamingThread.Start();
        }
        /**
        * Stop ()
        * @brief stops the current run
        */
        public void Stop()
        {
            mContinueReading = false;
        }
        /**
         * SetNewDevice ( BluetoothDeviceInfo vDeviceAddress)
        * @brief Will set the current brain pack to the current device
        @returns succesfull set 
        */
        public bool SetNewDevice(BluetoothAddress vDeviceAddress)
        {
            //first check if the device isn't already our paired device
            if (mCurrentBrainpackInfo != null && !mCurrentBrainpackInfo.DeviceAddress.Equals(vDeviceAddress))
            {
                //disconnect the current device 
                DisconnectBrainpack();
            }
            //check if the device is pairable
           /* if (BrainpackSearcher.PairDevice(vDeviceAddress))
            {
                //set the device as the current device
                BrainpackSearcher.gsPairedBrainpackMapping.TryGetValue(vDeviceAddress, out mCurrentBrainpackInfo);
                Start();
                return true;
            }*/
            return false;
        }
        /**
        * ConnectToDeviceThreadedFunc ()
        * @brief Thread that will attempt to connect to the current  bluetooth device
        */
        private void ConnectToDeviceThreadedFunc()
        {
            mCurrentBrainpack = new BluetoothClient();
            mCurrentBrainpack.BeginConnect(mCurrentBrainpackInfo.DeviceAddress, sgGuidService, BeginReadingBrainpackData, mCurrentBrainpack);
        }
 

        /**
        * BeginReadingBrainpackData (IAsyncResult vAR)
        * @brief Will begin reading from the brainpack
        * @param vAR: isync results, the bluetooth clients state
        */
        private void BeginReadingBrainpackData(IAsyncResult vAR)
        {

            BluetoothClient vClient = (BluetoothClient)vAR.AsyncState;
            try
            {
                vClient.EndConnect(vAR);//Asynchronously accepts an incoming connection attempt.
            }
            catch (Exception e)
            {
             BrainpackEventLogManager.InvokeEventLogError(e.ToString());    
            }
            
            //start the network stream
            mBrainpackNetworkStream = vClient.GetStream(); //capture stream from bt device
            mBrainpackNetworkStream.ReadTimeout = sgStreamingTimeout; //set a timeout

            try
            {
                mBrainpackNetworkStream.BeginRead(DataReceived, 0, sgPacketByteSize, CheckForEndOfRead, mBrainpackNetworkStream);
            }
                // ReSharper disable once UnusedVariable
            catch (InvalidOperationException)
            {
              
            }
        }
        /**
        * CheckForEndOfRead (IAsyncResult vAR)
        * @brief Checks if there is no more data to be read
        * @param vAR: isync results, the networkstream state
        */
        private void CheckForEndOfRead(IAsyncResult ar)
        {
            int numberOfBytesRead = mBrainpackNetworkStream.EndRead(ar);

            PacketParser(DataReceived);


            if (numberOfBytesRead == 0 || !mContinueReading)
            {
                // disconnected from the stream, create an event that the stream was disconnected to handle it
                //close the network stream, ?-> checks for null
                mBrainpackNetworkStream?.Close();
                DisconnectBrainpack();
                return;
            }
            else
            {
                //push the Data buffer on to OutboundBuffer
                ContinueReadingFromBrainpack();
            }

        }
        /**
        * ContinueReadingFromBrainpack ( )
        * @brief Continues reading the stream from the brainpack 
        */
        private void ContinueReadingFromBrainpack()
        {
            mBrainpackNetworkStream = mCurrentBrainpack.GetStream(); //capture stream from bt device
            mBrainpackNetworkStream.ReadTimeout = sgStreamingTimeout; //set a timeout
            mBrainpackNetworkStream.BeginRead(DataReceived, 0, sgPacketByteSize, CheckForEndOfRead, mBrainpackNetworkStream);
        }
        /**
        * SendCommand ( )
        * @brief Sends a command to the brainpack
        * todo not a priority right now
        */
        public void SendCommand(string vCommand)
        {

        }

        private void PacketParser(byte[] vPacket)
        {
            string vResult =  Encoding.ASCII.GetString(vPacket);
            OutboundBuffer.Enqueue(vPacket);
        }

        public bool IsConnected()
        {
            if (mCurrentBrainpack == null)
            {
                return false;
            }
            return mCurrentBrainpack.Connected;
        }
        /*async Task<string> ProcessString(byte[] vPacket)
        {
            StringBuilder vSb = new StringBuilder();
            vSb.Append(Encoding.ASCII,vPacket);
            string vChecker = vSb.ToString();
               while (true)
            {
                
            }
            return "";
        }*/
    }
}
