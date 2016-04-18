using System.ServiceProcess;

namespace BrainpackService
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BrainpackService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}