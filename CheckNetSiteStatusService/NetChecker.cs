﻿using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using Services;
using Data;


namespace CheckNetSiteStatusService
{
    public partial class NetChecker : ServiceBase
    {
        private readonly Logger _log = Logger.GetLogger();
        private readonly Dictionary<Thread, NetworkMonitor> _monitorCollection = new Dictionary<Thread, NetworkMonitor>();

        public NetChecker()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _log.WriteInformation("========================={0}====================", DateTime.Now.ToString("G"));
            _log.WriteInformation("Inicializando Aplicación");
            _log.WriteInformation("Creando subprocesos");
            try
            {
                foreach (IP ip in ConfigManager.Configuration.IpToTest)
                {
                    _log.WriteInformation("Creando monitor para: {0} ip: {1} puerto: {2}",
                        ip.Name, ip.Address, ip.Port);

                    var monitor = CreateMonitor(ip);
                    var thread = CreateThread(monitor);

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

        private static Thread CreateThread(NetworkMonitor monitor)
        {
            Thread thread = new Thread(monitor.Start);
            thread.Start();
            return thread;
        }

        private NetworkMonitor CreateMonitor(IP ip)
        {
            NetworkMonitor monitor = new NetworkMonitor(ip.Address, ip.Port,
                ConfigManager.Configuration.WaitTime, ConfigManager.Configuration.TimeOut);

            monitor.OnStatusChange += Monitor_OnStatusChange;
            monitor.OnConnectionBack += Monitor_OnConnectionBack;
            monitor.OnConnectionLost += Monitor_OnConnectionLost;
            return monitor;
        }

        private void Monitor_OnConnectionLost(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            _log.WriteError("Se ha perdido la conección con el host: {0} ip: {1} puerto: {2}",
                eventArgs.ConnectionName, eventArgs.Ip, eventArgs.Port);

            _log.WriteInformation("Enviando correo electrónico");

            _log.WriteInformation("Correo enviado correctamente");
        }

        private void Monitor_OnConnectionBack(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            _log.WriteInformation("Se ha restablecido la conección con el host: {0} ip: {1} puerto: {2}",
                eventArgs.ConnectionName, eventArgs.Ip, eventArgs.Port);

            _log.WriteInformation("Enviando correo electrónico");

            _log.WriteInformation( "Correo enviado correctamente");
        }

        private void Monitor_OnStatusChange(object sender, EventArgs e)
        {
            ChangeEventArgs eventArgs = (ChangeEventArgs)e;
            _log.WriteWarning("Ha cambiado el estado del monitor {0} a {1}", 
                eventArgs.Name, eventArgs.CurrentStatus);
        }

        protected override void OnStop()
        {
            _log.WriteInformation("Deteniendo Servicio");
            _log.WriteInformation("Cerrando subprocesos...");

            foreach (KeyValuePair<Thread, NetworkMonitor> monitor in _monitorCollection)
            {
                monitor.Key.Abort();
            }
        }
    }
}
