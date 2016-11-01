using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace CheckNetSiteStatusService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += ServiceInstaller_AfterInstall;
        }

        private void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            ServiceController serviceController = new ServiceController("NetMonitor");
            serviceController.Start();
        }
    }
}
