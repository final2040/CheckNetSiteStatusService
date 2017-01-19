using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Data.NetworkTest;
using Services.Configuration;
using Services.Encription;
using Services.Log;
using Services.Mail;

namespace Services
{
    /// <summary>
    /// Monitorea una o mas conexiones de red.
    /// </summary>
    public class Monitor 
    {

        private readonly Mail.Mail _smtpClient;
        private readonly Logger _log = Logger.GetLogger();
        private readonly Dictionary<Thread, NetworkMonitor> _monitorCollection = new Dictionary<Thread, NetworkMonitor>();
        private readonly Data.Configuration.Configuration _configuration;

        public Monitor()
        {
            ValidationHelper validator = new ValidationHelper();
            var validationResult = validator.TryValidate(ConfigManager.Configuration);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(string.Format("Ocurrío uno o mas errores al validar las configuraciones abortando..." + Environment.NewLine +
                                "{0}", validationResult.ToString()));
            }

            _configuration = ConfigManager.Configuration;

            try
            {
                _smtpClient = new Mail.Mail(GetCredentials(), _configuration.MailConfiguration.SmtpConfiguration);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Occurrio un error desencriptar la contraseña del correo " +
                                "por favor verifique la contraseña: ", ex);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Occurrio un error desencriptar la contraseña del correo " +
                                "por favor verifique la contraseña: ", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Occurrio un error al cargar las configuraciones del" +
                                                                  " correo: ", ex);
            }
        }
       
        /// <summary>
        /// Inicia la aplicación, creando los hilos encargados del monitoreo.
        /// </summary>
        public void Start()
        {
            _log.WriteInformation("========================={0}====================", DateTime.Now.ToString("G"));
            _log.WriteInformation("Inicializando Aplicación");
            _log.WriteInformation("Creando subprocesos");
            if (_configuration != null)
            {
                CreateThreads();
            }
        }

        /// <summary>
        /// Detiene los hilos creados y cierra la aplicación.
        /// </summary>
        public void Stop()
        {
            _log.WriteInformation("Deteniendo Servicio");
            _log.WriteInformation("Cerrando subprocesos...");

            foreach (var monitor in _monitorCollection)
            {
                monitor.Key.Abort();
            }
            _smtpClient.Dispose();
        }

        /// <summary>
        /// Envía un correo electrónico con los detalles necesarios, cuando la conexión se pierde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Monitor_OnConnectionLost(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            SendEmail(eventArgs, EventType.ConnectionLost);
        }

        /// <summary>
        /// Envía un correo electrónico con los detalles nesesarios, cuando la conexión se restablece.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Monitor_OnConnectionBack(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            SendEmail(eventArgs, EventType.ConnectionRecover);
        }

        /// <summary>
        /// Registra en el log cuando el monitor cambia de estado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Monitor_OnStatusChange(object sender, EventArgs e)
        {
            ChangeEventArgs eventArgs = (ChangeEventArgs)e;
            _log.WriteInformation("Ha cambiado el estado del monitor {0} a {1}",
                eventArgs.Name, eventArgs.CurrentStatus);
        }

        /// <summary>
        /// Crea los subprocesos que monitorearan los diversos servicios/conexiones de red
        /// </summary>
        private void CreateThreads()
        {
            _log.WriteInformation("Creando subprocesos");
            try
            {
                foreach (var testConfig in _configuration.Tests)
                {
                    _log.WriteInformation("Creando monitor para: {0}",
                        testConfig.ToString());

                    var monitor = CreateMonitor(testConfig);
                    Thread thread = new Thread(monitor.Start);
                    thread.Start();

                    _monitorCollection.Add(thread, monitor);
                    _log.WriteInformation("Monitor creado exitosamente");
                }
                _log.WriteInformation("Sub Procesos Creados Satisfactoriamente {0} " +
                                  "procesos creados", _monitorCollection.Count);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error mientras se creaban los procesos: ", ex);
            }
        }

        /// <summary>
        /// Crea un monitor de servicio/conexión de red
        /// </summary>
        /// <param name="testConfiguration"></param>
        /// <returns></returns>
        private NetworkMonitor CreateMonitor(TestConfigurationBase testConfiguration)
        {
            NetworkMonitor monitor = new NetworkMonitor(TestFactory.CreateInstance(testConfiguration), _configuration.TestConfig, testConfiguration.Name);

            monitor.OnStatusChange += Monitor_OnStatusChange;
            monitor.OnConnectionBack += Monitor_OnConnectionBack;
            monitor.OnConnectionLost += Monitor_OnConnectionLost;
            return monitor;
        }

        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="eventType"></param>
        private void SendEmail(TestNetworkEventArgs eventArgs, EventType eventType)
        {
            var typeOfTheEvent = eventType == EventType.ConnectionLost ? "Perdido" : "Restablecido";

            _log.WriteInformation("Se ha {0} la conección con el host: {1} ip: {2} - {3}",
                typeOfTheEvent, eventArgs.ConnectionName, eventArgs.HostNameOrAddress, eventArgs.TestConfig);

            var testResults = PrintResults(eventArgs);

            _log.WriteInformation("Enviando correo electrónico");
            MessageBuilder builder = new MessageBuilder(_configuration.MailConfiguration);
            builder.AddParam("testresults", testResults);
            builder.AddParam("date", DateTime.Now.ToString("d"));
            builder.AddParam("host", eventArgs.HostNameOrAddress);
            builder.AddParam("hostname", eventArgs.ConnectionName);
            builder.AddParam("status", typeOfTheEvent);
            builder.AddParam("testconfig", eventArgs.TestConfig);
            builder.AddParam("sendfrom", _configuration.MailConfiguration.SendFrom);
            builder.AddParam("timeout", _configuration.TestConfig.TimeOutSeconds);
            builder.AddParam("computername", Environment.MachineName);
            builder.AddParam("appname", "DreamSoft Network Monitor");
            builder.AddParam("appversion", new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version);

            try
            {
                _smtpClient.Send(builder.Build());
                _log.WriteInformation("Correo enviado correctamente");
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrío un error al envíar el correo electrónico: ", ex);
            }
        }

        /// <summary>
        /// Devuelve un string con los resultados de las pruebas realizadas.
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Obtiene las credencíales del correo electrónico de la configuración.
        /// </summary>
        /// <returns></returns>
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