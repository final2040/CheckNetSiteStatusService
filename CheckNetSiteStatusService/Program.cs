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
            var ServicesToRun = new ServiceBase[]
            {
                new NetChecker()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
