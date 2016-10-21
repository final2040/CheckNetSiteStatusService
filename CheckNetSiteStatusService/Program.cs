using System;
using System.IO;
using System.ServiceProcess;
using Services;

namespace CheckNetSiteStatusService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new NetChecker()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
