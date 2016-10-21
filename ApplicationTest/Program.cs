using Data;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pr = new Program();
            pr.OnStart();
            Console.ReadKey();
        }

        private readonly Logger _log = Logger.GetLogger();
        private readonly Dictionary<Thread, NetworkMonitor> _monitorCollection = new Dictionary<Thread, NetworkMonitor>();
        
        protected  void OnStart()
        {
            Console.WriteLine("========================={0}====================", DateTime.Now.ToString("G"));
            Console.WriteLine("Inicializando Aplicación");
            Console.WriteLine("Creando subprocesos");
            try
            {
                foreach (IP ip in ConfigManager.Configuration.IpToTest)
                {
                    Console.WriteLine("Creando monitor para: {0} ip: {1} puerto: {2}",
                        ip.Name, ip.Address, ip.Port);

                    var monitor = CreateMonitor(ip);
                    var thread = CreateThread(monitor);

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
            Console.WriteLine("Se ha perdido la conección con el host: {0} ip: {1} puerto: {2}",
                eventArgs.ConnectionName, eventArgs.Ip, eventArgs.Port);

            Console.WriteLine("Enviando correo electrónico");

            Console.WriteLine("Correo enviado correctamente");
        }

        private void Monitor_OnConnectionBack(object sender, EventArgs e)
        {
            TestNetworkEventArgs eventArgs = (TestNetworkEventArgs)e;
            Console.WriteLine("Se ha restablecido la conección con el host: {0} ip: {1} puerto: {2}",
                eventArgs.ConnectionName, eventArgs.Ip, eventArgs.Port);

            Console.WriteLine("Enviando correo electrónico");

            Console.WriteLine("Correo enviado correctamente");
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
        }
    }
}
