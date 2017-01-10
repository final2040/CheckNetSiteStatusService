using System;
using System.ServiceProcess;
using Services;
using Services.Log;

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
                Logger.Log.WriteError("{0}\r\n{1}\r\n{2}",e.Message, e.InnerException?.Message, e.InnerException?.StackTrace);
            }
        }
    }
}
