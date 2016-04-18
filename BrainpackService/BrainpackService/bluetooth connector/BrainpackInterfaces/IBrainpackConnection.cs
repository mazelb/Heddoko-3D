 

namespace BrainpackService.bluetooth_connector.BrainpackInterfaces
{
    public interface IBrainpackConnection
    {
 
        
        void Initialize(string vBrainpackName);
        void Start();
        void Stop();

        bool IsConnected { get; }

        string GetNextFrame();
        void SendCommandToBrainpack(string vMsg);
        
        void ThreadedFunction();
    }
}
