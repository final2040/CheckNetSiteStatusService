using System;
using System.Net.NetworkInformation;
using Data;
using Data.NetworkTest;

namespace Services.NetworkTests
{
    /// <summary>
    /// Realiza una prueba de ping en el host indicado.
    /// </summary>
    public class PingTest : INetworkTest
    {
        private readonly Ping _ping = new Ping();
        private string _hostNameOrAddress;

        public PingTest(string hostNameOrAddress)
        {
            _hostNameOrAddress = hostNameOrAddress;
        }

        public PingTest(PingTestConfiguration configuration) : this(configuration.Host)
        {
            TestConfiguration = configuration;
        }

        /// <summary>
        /// Propiedad que obtiene o Establece el host a probar.
        /// </summary>
        public virtual string HostNameOrAddress
        {
            get { return _hostNameOrAddress; }
            set { _hostNameOrAddress = value; }
        }

        /// <summary>
        /// Propiedad que obtiene la configuración de la clase.
        /// </summary>
        public TestConfigurationBase TestConfiguration { get; }

        /// <summary>
        /// Realiza una prueba ping
        /// </summary>
        /// <returns>Resultados de la prueba</returns>
        public virtual INetTestResult Test()
        {
            PingReply pingResult;
            IPStatus pingStatus;
            try
            {
                pingResult = _ping.Send(_hostNameOrAddress);
                pingStatus = pingResult.Status;
            }
            catch (Exception)
            {
                pingResult = null;
                pingStatus = IPStatus.Unknown;
            }

            var result = new PingTestResult(_hostNameOrAddress, pingStatus);

            if (pingStatus == IPStatus.Success)
            {
                result.Bytes = pingResult.Buffer.Length;
                result.Time = pingResult.RoundtripTime;
                result.Ttl = pingResult.Options.Ttl;
            }
            return result;
        }
    }
}