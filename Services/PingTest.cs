using System;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Data;

namespace Services
{
    public class PingTest : INetworkTest
    {
        private const string IP_REGEX = "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        private readonly Ping _ping = new Ping();
        private string _hostNameOrAddress;

        public PingTest(string hostNameOrAddress)
        {
            _hostNameOrAddress = hostNameOrAddress;
        }

        /// <summary>
        /// Propiedad que obtiene o Establece el host a probar.
        /// </summary>
        public virtual string HostNameOrAddress
        {
            get { return _hostNameOrAddress; }
            set { _hostNameOrAddress = value; }
        }
        

        public virtual INetTestResult Test()
        {
            var pingResult = _ping.Send(_hostNameOrAddress);
            var result = new PingTestResult(_hostNameOrAddress, pingResult.Status);

            if (pingResult?.Status == IPStatus.Success)
            {
                result.Bytes = pingResult.Buffer.Length;
                result.Time = pingResult.RoundtripTime;
                result.Ttl = pingResult.Options.Ttl;
            }
            return result;
        }
    }
}