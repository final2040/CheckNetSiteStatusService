using Data;
using Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ApplicationTest
{
    class Program
    {
        public Program()
        {
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
            Console.CancelKeyPress += Console_CancelKeyPress;
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

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Deteniendo Servicio");
            Console.WriteLine("Cerrando subprocesos...");

            foreach (KeyValuePair<Thread, NetworkMonitor> monitor in _monitorCollection)
            {
                monitor.Key.Abort();
            }
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

        static void Main(string[] args)
        {
            Program pr = new Program();
            pr.OnStart();
        }

        private readonly Mail _smtpClient = new Mail();
        private readonly Logger _log = Logger.GetLogger();
        private readonly Dictionary<Thread, NetworkMonitor> _monitorCollection = new Dictionary<Thread, NetworkMonitor>();
        private readonly Configuration _configuration;

        protected  void OnStart()
        {
            Console.WriteLine("========================={0}====================", DateTime.Now.ToString("G"));
            Console.WriteLine("Inicializando Aplicación");
            if (_configuration !=null)
            {
                CreateThreads();
            }
        }

        private void CreateThreads()
        {
            Console.WriteLine("Creando subprocesos");
            try
            {
                foreach (IP ip in _configuration.Tests)
                {
                    Console.WriteLine("Creando monitor para: {0} ip: {1} puerto: {2}",
                        ip.Name, ip.Address, ip.Port);

                    var monitor = CreateMonitor(ip);
                    Thread thread = new Thread(monitor.Start);
                    thread.Start();

                    _monitorCollection.Add(thread, monitor);
                    Console.WriteLine("Monitor creado exitosamente");
                }
                Console.WriteLine("Sub Procesos Creados Satisfactoriamente {0} " +
                                  "procesos creados", _monitorCollection.Count);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Ocurrio un error mientras se creaban los procesos: \r\n{0}\r\n" +
                                  "StackTrace: {1}", exception.Message, exception.StackTrace);
            }
        }

        private NetworkMonitor CreateMonitor(IP ip)
        {
            NetworkMonitor monitor = new NetworkMonitor(ip.Address, ip.Port,
                _configuration.TestConfig.WaitTimeSeconds * 1000, _configuration.TestConfig.TimeOutSeconds * 1000,ip.Name);

            monitor.OnStatusChange += Monitor_OnStatusChange;
            monitor.OnConnectionBack += Monitor_OnConnectionBack;
            monitor.OnConnectionLost += Monitor_OnConnectionLost;
            return monitor;
        }

        private void Monitor_OnConnectionLost(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            SendEmail(eventArgs, EventType.ConnectionLost);
        }

        private void SendEmail(TestNetworkEventArgs eventArgs, EventType eventType)
        {
            var @event = eventType == EventType.ConnectionLost ? "Perdido" : "Restablecido";

            Console.WriteLine("Se ha {0} la conección con el host: {1} ip: {2} puerto: {3}",
                @event,eventArgs.ConnectionName, eventArgs.Ip, eventArgs.Port);

            StringBuilder stringBuilder = new StringBuilder();

            string testResults = "";
            foreach (var netTestResult in eventArgs.NetTestResults)
            {
                stringBuilder.AppendLine(netTestResult.ToString() + Environment.NewLine);
            }
            testResults = stringBuilder.ToString();

            Console.WriteLine("Enviando correo electrónico");
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
                Console.WriteLine("Correo enviado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrío un error al envíar el correo electronico {0} \r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void Monitor_OnConnectionBack(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            SendEmail(eventArgs, EventType.ConnectionRecover);
        }

        private void Monitor_OnStatusChange(object sender, EventArgs e)
        {
            ChangeEventArgs eventArgs = (ChangeEventArgs)e;
            Console.WriteLine("Ha cambiado el estado del monitor {0} a {1}",
                eventArgs.Name, eventArgs.CurrentStatus);
        }

        protected void OnStop()
        {
            Console.WriteLine("Deteniendo Servicio");
            Console.WriteLine("Cerrando subprocesos...");

            foreach (KeyValuePair<Thread, NetworkMonitor> monitor in _monitorCollection)
            {
                monitor.Key.Abort();
            }
            _smtpClient.Dispose();
        }
        
    }

    internal enum EventType
    {
        ConnectionLost,
        ConnectionRecover
    }
}
