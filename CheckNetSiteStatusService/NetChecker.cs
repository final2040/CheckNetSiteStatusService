using System.Collections.Generic;
using System.Net;
using System.ServiceProcess;
using Services;
using Data;

namespace CheckNetSiteStatusService
{
    public partial class NetChecker : ServiceBase
    {
        private Logger _log = Logger.GetLogger();

        public NetChecker()
        {
            InitializeComponent();

            Mail mailService = new Mail();
            SmtpConfiguration smtpConfiguration = new SmtpConfiguration("smtp.gmail.com", 465, true);
            NetworkCredential smtpCredential = new NetworkCredential("final20@gmail.com", "gooR3n3z4m0r4n0");
            NetworkMonitor monitor = new NetworkMonitor(new NetworkTest("172.28.129.100",8080),2000,12000,"ArchivosAifcoMx webservice");

            Configuration configuration = new Configuration();
            configuration.WaitTime = 2000;
            configuration.TimeOut = 120000;
            configuration.IpToTest = new List<IP>()
            {
                new IP() {Address = "172.28.129.100", Port = 8080, Name = "ArchivosAifcomx"}
            };
        }

        protected override void OnStart(string[] args)
        {
            _log.Write(LogType.Information, "Inicializando Aplicación");   
        }

        protected override void OnStop()
        {
        }
    }
}
