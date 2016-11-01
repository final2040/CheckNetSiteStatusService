using System;
using System.Net.Sockets;
using Data;

namespace Services
{
    public class TcpTest:INetworkTest
    {
        private readonly PingTest _ping;
        private int _remoteTcpPort;
        private int _tcpMillisecondsTimeout;
        private string _hostNameOrAddress;
        
        private const int PORT_MAX_VALUE = 65535;
        private const int PORT_MIN_VALUE = 1;
        private const int DEFAULT_TIMEOUT = 1500;

        /// <summary>
        /// Crea un a instancia del objeto TcpTest
        /// </summary>
        /// <param name="hostNameOrAddress">Dirección del host a probar.</param>
        /// <param name="remoteTcpPort">Puerto remoto que se probará.</param>
        public TcpTest(string hostNameOrAddress, int remoteTcpPort) :this(hostNameOrAddress,remoteTcpPort, DEFAULT_TIMEOUT){}

        /// <summary>
        /// Crea un a instancia del objeto TcpTest
        /// </summary>
        public TcpTest(TcpTestConfiguration configuration)
            : this(configuration.Host, configuration.Port, configuration.TimeOutMilliSeconds)
        {
            TestConfiguration = configuration;
        }

        /// <summary>
        /// Crea un a instancia del objeto TcpTest
        /// </summary>
        /// <param name="hostNameOrAddress">Dirección del host a probar.</param>
        /// <param name="remoteTcpPort">Puerto TCP remoto que se probará.</param>
        /// <param name="tcpMillisecondsTimeout">Tiempo máximo de respuesta.</param>
        public TcpTest(string hostNameOrAddress, int remoteTcpPort, int tcpMillisecondsTimeout)
        {
            if (remoteTcpPort < PORT_MIN_VALUE || remoteTcpPort > PORT_MAX_VALUE)
            {
                throw new ArgumentException("El número de puerto no es valido debe de ser entre: 1 y 65535");
            }

            _tcpMillisecondsTimeout = tcpMillisecondsTimeout == 0 ? DEFAULT_TIMEOUT : tcpMillisecondsTimeout;
            _hostNameOrAddress = hostNameOrAddress;
            _remoteTcpPort = remoteTcpPort;
            _ping = new PingTest(_hostNameOrAddress);
        }
        
        /// <summary>
        /// Propiedad que obtiene o Establece el tiempo de espera máximo para la conexión TCP.
        /// </summary>
        public virtual int TcpMillisecondsTimeout
        {
            set { _tcpMillisecondsTimeout = value; }
            get { return _tcpMillisecondsTimeout; }
        }

        /// <summary>
        /// Propiedad que obtiene o Establece el puerto TCP que se probará
        /// </summary>
        public int RemoteTcpPort
        {
            get { return _remoteTcpPort; }
            set
            {
                if (value < PORT_MIN_VALUE || value > PORT_MAX_VALUE)
                {
                    throw new ArgumentException("El número de puerto no es valido debe de ser entre: 1 y 65535");
                }
                _remoteTcpPort = value;
            }
        }
        /// <summary>
        /// Propiedad que obtiene o Establece el host a probar.
        /// </summary>
        public string HostNameOrAddress
        {
            get { return _hostNameOrAddress; }
            set { _hostNameOrAddress = value; }
        }

        /// <summary>
        /// Prueba la conexión al puerto TCP especificado
        /// </summary>
        /// <returns>Objeto TcpTestResult que representa el resultado de la prueba.</returns>
        public virtual INetTestResult Test()
        {
            var pingResult = _ping.Test() as PingTestResult;
            var tcpResult = new TcpTestResult(pingResult, _remoteTcpPort);
            
            using (TcpClient tcpTester = new TcpClient())
            {
                IAsyncResult result = tcpTester.BeginConnect((string)_hostNameOrAddress, _remoteTcpPort, null, null);
                tcpResult.TcpTestSuccessed = result.AsyncWaitHandle.WaitOne(_tcpMillisecondsTimeout);
            }
            
            return tcpResult;
        }

        public TestConfigurationBase TestConfiguration { get; }
    }
}