﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Data;

namespace Services
{
    public class Monitor 
    {

        private readonly Mail _smtpClient = new Mail();
        private readonly Logger _log = Logger.GetLogger();
        private readonly Dictionary<Thread, NetworkMonitor> _monitorCollection = new Dictionary<Thread, NetworkMonitor>();
        private readonly Configuration _configuration;

        public Monitor()
        {
            ValidationHelper validator = new ValidationHelper();
            var validationResult = validator.TryValidate(ConfigManager.Configuration);

            if (validationResult.IsValid)
            {
                _configuration = ConfigManager.Configuration;
                InitializeSmtp();
            }
            else
            {
                throw new InvalidOperationException(string.Format("Ocurrío uno o mas errores al validar las configuraciones abortando..." + Environment.NewLine +
                                "{0}", validationResult.ToString()));
            }
        }

        private void InitializeSmtp()
        {
            try
            {
                _smtpClient.SmtpConfiguration = _configuration.MailConfiguration.SmtpConfiguration;
                _smtpClient.SmtpCredentials = GetCredentials();
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

        private NetworkMonitor CreateMonitor(TestConfigurationBase testConfiguration)
        {
            NetworkMonitor monitor = new NetworkMonitor(TestFactory.CreateInstance(testConfiguration), _configuration.TestConfig, testConfiguration.Name);

            monitor.OnStatusChange += Monitor_OnStatusChange;
            monitor.OnConnectionBack += Monitor_OnConnectionBack;
            monitor.OnConnectionLost += Monitor_OnConnectionLost;
            return monitor;
        }

        private void SendEmail(TestNetworkEventArgs eventArgs, EventType eventType)
        {
            var typeOfTheEvent = eventType == EventType.ConnectionLost ? "Perdido" : "Restablecido";

            _log.WriteInformation("Se ha {0} la conección con el host: {1} ip: {2} - {3}",
                typeOfTheEvent, eventArgs.ConnectionName, eventArgs.HostNameOrAddress, eventArgs.TestConfig);

            var testResults = PrintResults(eventArgs);

            _log.WriteInformation("Enviando correo electrónico");
            MailBuilder builder = new MailBuilder(_configuration);
            builder.AddParam("testresults", testResults);
            builder.AddParam("date", DateTime.Now.ToString("d"));
            builder.AddParam("host", eventArgs.HostNameOrAddress);
            builder.AddParam("hostname", eventArgs.ConnectionName);
            builder.AddParam("status", typeOfTheEvent);
            builder.AddParam("testconfig", eventArgs.TestConfig);
            builder.AddParam("ip", "La opción Port ha quedado obsoleta por favor utilize {testconfig} en su lugar " +
                                     "para obtener la configuración de la prueba.");
            builder.AddParam("port", "La opción Port ha quedado obsoleta por favor utilize {testconfig} en su lugar " +
                                     "para obtener la configuración de la prueba.");
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