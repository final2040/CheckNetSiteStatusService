using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using Services;
using Data;
using System.Security.Cryptography;
using System.Text;

namespace CheckNetSiteStatusService
{
    public partial class NetChecker : ServiceBase
    {
        
        private readonly Mail _smtpClient = new Mail();
        private readonly Logger _log = Logger.GetLogger();
        private readonly Dictionary<Thread, INetworkMonitor> _monitorCollection = new Dictionary<Thread, INetworkMonitor>();
        private readonly Configuration _configuration;

        public NetChecker()
        {
            InitializeComponent();
            ValidationHelper validator = new ValidationHelper();
            var validationResult = validator.TryValidate(ConfigManager.Configuration);
            if (validationResult.IsValid)
            {
                _configuration = ConfigManager.Configuration;
                Initialize();
            }
            else
            {
                _log.WriteError("Ocurrío uno o mas errores al validar las configuraciones abortando..." + Environment.NewLine +
                                "{0}", validationResult.ToString());
            }
        }

        private void Initialize()
        {
            try
            {
                _smtpClient.SmtpConfiguration = _configuration.MailConfiguration.SmtpConfiguration;
                _smtpClient.SmtpCredentials = GetCredentials();
            }
            catch (FormatException ex)
            {
                _log.WriteError("Occurrio un error desencriptar la contraseña del correo " +
                                "por favor verifique la contraseña {0} \r\n{1}", ex.Message, ex.StackTrace);
            }
            catch (CryptographicException ex)
            {
                _log.WriteError("Occurrio un error desencriptar la contraseña del correo " +
                                "por favor verifique la contraseña {0} \r\n{1}", ex.Message, ex.StackTrace);
            }
            catch (Exception ex)
            {
                _log.WriteError("Occurrio un error al cargar las " +
                                "configuraciones del correo {0} \r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        protected override void OnStart(string[] args)
        {
            _log.WriteInformation("========================={0}====================", DateTime.Now.ToString("G"));
            _log.WriteInformation("Inicializando Aplicación");
            _log.WriteInformation("Creando subprocesos");
            if (_configuration != null)
            {
                CreateThreads();
            }
        }

        protected override void OnStop()
        {
            _log.WriteInformation("Deteniendo Servicio");
            _log.WriteInformation("Cerrando subprocesos...");

            foreach (var monitor in _monitorCollection)
            {
                monitor.Key.Abort();
            }
            _smtpClient.Dispose();
        }

        private void Monitor_OnConnectionLost(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            SendEmail(eventArgs, EventType.ConnectionLost);
        }

        private void Monitor_OnConnectionBack(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            SendEmail(eventArgs, EventType.ConnectionRecover);
        }

        private void Monitor_OnStatusChange(object sender, EventArgs e)
        {
            ChangeEventArgs eventArgs = (ChangeEventArgs)e;
            _log.WriteInformation("Ha cambiado el estado del monitor {0} a {1}",
                eventArgs.Name, eventArgs.CurrentStatus);
        }

        private void CreateThreads()
        {
            _log.WriteInformation("Creando subprocesos");
            try
            {
                foreach (IP ip in _configuration.IpToTest)
                {
                    _log.WriteInformation("Creando monitor para: {0} ip: {1} puerto: {2}",
                        ip.Name, ip.Address, ip.Port);

                    var monitor = CreateMonitor(ip);
                    Thread thread = new Thread(monitor.Start);
                    thread.Start();

                    _monitorCollection.Add(thread, monitor);
                    _log.WriteInformation("Monitor creado exitosamente");
                }
                _log.WriteInformation("Sub Procesos Creados Satisfactoriamente {0} " +
                                  "procesos creados", _monitorCollection.Count);
            }
            catch (Exception exception)
            {
                _log.WriteError("Ocurrio un error mientras se creaban los procesos: \r\n{0}\r\n" +
                                  "StackTrace: {1}", exception.Message, exception.StackTrace);
            }
        }
        
        private INetworkMonitor CreateMonitor(IP ip)
        {
            INetworkMonitor monitor = new NetworkMonitor(ip.Address, ip.Port,
                _configuration.WaitTimeSeconds * 1000, _configuration.TimeOutSeconds * 1000, ip.Name);

            monitor.OnStatusChange += Monitor_OnStatusChange;
            monitor.OnConnectionBack += Monitor_OnConnectionBack;
            monitor.OnConnectionLost += Monitor_OnConnectionLost;
            return monitor;
        }

        private void SendEmail(TestNetworkEventArgs eventArgs, EventType eventType)
        {
            var @event = eventType == EventType.ConnectionLost ? "Perdido" : "Restablecido";

            _log.WriteInformation("Se ha {0} la conección con el host: {1} ip: {2} puerto: {3}",
                @event, eventArgs.ConnectionName, eventArgs.Ip, eventArgs.Port);
            
            var testResults = PrintResults(eventArgs);

            _log.WriteInformation("Enviando correo electrónico");
            MailBuilder builder = new MailBuilder(_configuration);
            builder.AddParam("testresults", testResults);
            builder.AddParam("date", DateTime.Now.ToString("d"));
            builder.AddParam("ip", eventArgs.Ip);
            builder.AddParam("port", eventArgs.Port);
            builder.AddParam("hostname", eventArgs.ConnectionName);
            builder.AddParam("status", @event);

            try
            {
                _smtpClient.Send(builder.Build());
                _log.WriteInformation("Correo enviado correctamente");
            }
            catch (Exception ex)
            {
                _log.WriteError("Ocurrío un error al envíar el correo electronico {0} \r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private string PrintResults(TestNetworkEventArgs eventArgs)
        {
            StringBuilder stringBuilder = new StringBuilder();

            eventArgs.NetTestResults.Reverse();
            var results = eventArgs.NetTestResults.Take(10);

            foreach (var netTestResult in results.Reverse())
            {
                stringBuilder.AppendLine(netTestResult.ToString() + Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        private NetworkCredential GetCredentials()
        {
            Md5Key key = new Md5Key("airpak-latam", 100);
            AesEncryptor encryptor = new AesEncryptor(key);
            NetworkCredential credentials = new NetworkCredential
            {
                UserName = _configuration.MailConfiguration.SmtpCredentials.UserName,
                SecurePassword = encryptor.DecryptToSecureString(
                    _configuration.MailConfiguration.SmtpCredentials.Password)
            };
            return credentials;
        }

    }
}
