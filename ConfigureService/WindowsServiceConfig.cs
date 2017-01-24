using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigureService
{
    [RunInstaller(true)]
    public partial class WindowsServiceConfig : System.Configuration.Install.Installer
    {
        public WindowsServiceConfig()
        {
            InitializeComponent();
        }

        private void ConfigureService(string serviceName)
        {
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = $"failure \"{serviceName}\" reset= 0 actions= restart/60000";

                process.Start();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }
            if (exitCode != 0)
                throw new InvalidOperationException("Ocurrio un error a configurar el servicio de windows," +
                                                    "el programa funcionara, pero requiere se configuren manualmente" +
                                                    "los metódos de recuperación.");
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            try
            {
                ConfigureService("NetMonitor");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
    }
}
