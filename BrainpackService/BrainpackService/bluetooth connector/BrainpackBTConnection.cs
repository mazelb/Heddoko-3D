using System; 
using System.IO; 
using System.Net.Sockets; 
using System.Threading; 
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.adt;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BrainpackService.bluetooth_connector
{
   public  class BrainpackBTConnection
   {
       private BluetoothClient mBtClient;
        //readonly Guid mServiceClassID = new Guid("{29913A2D-EB93-40cf-BBB8-DEEE26452197}");
        readonly string mHeddokoServiceName = "Heddoko Brainpack Service";
       private BluetoothListener mListener;
       private bool mClosing;
        private CircularQueue<string> mCircularBuffer = new CircularQueue<string>(1024,true); 

       
       public void StartBluetooth()
       {
           try
           {
               mBtClient = new BluetoothClient();
           }
           catch (Exception e)
           {
                BrainpackEventLogManager.InvokeExceptionThrowingDelegate(e.ToString());
           }
           StartListener();
       }

       private void StartListener()
       {
           BluetoothListener vListener  = new BluetoothListener(BluetoothService.SerialPort);
           vListener.ServiceName = mHeddokoServiceName;
            vListener.Start();
           mListener = vListener;
           ThreadPool.QueueUserWorkItem(ListenerAcceptRunner, vListener);
       }

       private void ListenerAcceptRunner(object vState)
       {
           BluetoothListener vListener = mListener;
            //accepts only one bluetooth connection at a time
           while (true)
           {
              BluetoothClient vBTClientConnection = mListener.AcceptBluetoothClient();
             NetworkStream vPeer =  vBTClientConnection.GetStream();
               SetConnection(vPeer, false, vBTClientConnection.RemoteEndPoint);
               ReadMessagesToEnd(vPeer);
           }
       }

       private void SetConnection(Stream vPeerStream, bool vOutbound, BluetoothEndPoint vEndPoint)
       {
           mClosing = false;
            StreamWriter vConnectionwriter = new StreamWriter(vPeerStream);
           vConnectionwriter.NewLine = "\r\n";
            BrainpackEventLogManager.InvokeEventLogMessage(vOutbound? "Connected to ":"Connection from"+ vEndPoint.Address);

       }

       public BluetoothClient ConnectToBluetooth(BluetoothAddress vAddr)
       {
           BluetoothClient vClient = new BluetoothClient();
           try
           {
               vClient.Connect(vAddr, BluetoothService.SerialPort);
               NetworkStream vPeer = vClient.GetStream();
                SetConnection(vPeer,true,vClient.RemoteEndPoint);
               ThreadPool.QueueUserWorkItem(ReadMessagesToEnd_Runner, vPeer);
           }
           catch (SocketException e)
           {
               BrainpackEventLogManager.InvokeExceptionThrowingDelegate(e.ToString());
           }
           return vClient;
       }

       private void ReadMessagesToEnd_Runner(object vState)
       {
           Stream vPeer = (Stream) vState;
           ReadMessagesToEnd(vPeer);
       }

       private void ReadMessagesToEnd(Stream vPeer)
       {
           StreamReader vReader = new StreamReader(vPeer);
           while (true)
           {
               string vLine;
               try
               {
                   vLine = vReader.ReadLine();
               }
               catch (IOException vIoex)
               {
                   if (mClosing)
                   {
                       //error is ignored, because it was closed from the service end
                   }
                   else
                   {
                       BrainpackEventLogManager.InvokeExceptionThrowingDelegate(vIoex.ToString());
                   }
                   break;
               }
               if (vLine == null)
               {
                    //todo
                   //this means the connection was closed 
                   //need to handle this
                   break;
               }
           }//end of while loop
           ConnectionCleanup();
       }

       private void ConnectionCleanup()
       {
            //clean up connection, maybe inform any interested listeners of this status
           mClosing = true;
       }
   }
}
