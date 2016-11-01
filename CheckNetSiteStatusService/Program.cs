using System;
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

            try
            {
                var ServicesToRun = new ServiceBase[]
                {
                     new NetChecker()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                Logger.Log.WriteError(e.Message);
            }
        }
    }
}
