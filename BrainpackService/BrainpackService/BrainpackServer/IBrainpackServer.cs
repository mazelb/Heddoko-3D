using System.Net.Sockets;
using InTheHand.Net.Sockets;

namespace BrainpackService.BrainpackServer
{
    public interface IBrainpackServer
    {
        ServerCommandRouter ServerCommandRouter { get; set; }
        bool Start();
        bool Send(object vHandler,string vData);

        bool Send(object vHandler, byte[] vData);

        void Stop();
    }
}