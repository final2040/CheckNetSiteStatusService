using System.ServiceProcess;
using Services;

namespace CheckNetSiteStatusService
{
    public partial class NetChecker : ServiceBase
    {
        private readonly Monitor _monitor;

        public NetChecker()
        {
            _monitor = new Monitor();
        }


        protected override void OnStart(string[] args)
        {
            _monitor.Start();
        }

        protected override void OnStop()
        {
           _monitor.Stop();
        }

    }
}
