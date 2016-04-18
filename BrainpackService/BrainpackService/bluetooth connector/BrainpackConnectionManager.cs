using System; 
using InTheHand.Net;

namespace BrainpackService.bluetooth_connector
{
    public class BrainpackConnectionManager
    {
        private static BrainpackConnectionManager sInstance;
        private Brainpack mCurrentBrainpack;
        public static BrainpackConnectionManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new BrainpackConnectionManager();
                }
                return sInstance;
            }
        }

        public bool ConnectToBrainpack(string vAddress)
        {
            if (mCurrentBrainpack == null)
            {
                mCurrentBrainpack = new Brainpack();
            }
            BluetoothAddress vBtAddress = BluetoothAddress.Parse(vAddress);
            mCurrentBrainpack.SetNewDevice(vBtAddress);
            return mCurrentBrainpack.IsConnected();

        }

        public byte[] GetBrainPackData()
        {
            byte[] vBrainpackData = new byte[200];
            try
            {
                vBrainpackData = mCurrentBrainpack.OutboundBuffer.Dequeue();
            }
            catch (Exception  )
            {
                //todo, if null do something
                
            }
            return vBrainpackData;
        }
        /// <summary>
        /// Stops the current brainpack from pulling data
        /// </summary>
        public void Stop()
        {
            mCurrentBrainpack?.Stop();
        }

    }
}
