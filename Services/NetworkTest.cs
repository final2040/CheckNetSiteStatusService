using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Data;

namespace Services
{
    public class NetworkTest
    {
        private const int PORT_MAX_VALUE = 65535;
        private const int PORT_MIN_VALUE = 1;
        private const int NO_PORT_SELECTED = 0;
        private const string IP_REGEX = "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        private readonly Ping _ping = new Ping();
        private readonly int _tcpMillisecondsTimeout;
        private string _hostNameOrAddress;
        private readonly int _remotePort;

        public NetworkTest(string hostNameOrAddress) : this(hostNameOrAddress, 0, 2000) { }

        public NetworkTest(string hostNameOrAddress, int remotePort) : this(hostNameOrAddress, remotePort, 2000) { }

        public NetworkTest(string hostNameOrAddress, int remotePort, int tcpMillisecondsTimeout)
        {
            Regex validateIp = new Regex(IP_REGEX);

            if (remotePort != NO_PORT_SELECTED && remotePort < PORT_MIN_VALUE || remotePort > PORT_MAX_VALUE)
            {
                throw new ArgumentException("El número de puerto no es valido debe de ser entre: 1 y 65535");
            }
            if (!validateIp.IsMatch(hostNameOrAddress))
            {
                throw new ArgumentException("La ip no cumple con el formato correcto: xxx.xxx.xxx.xxx");
            }

            _hostNameOrAddress = hostNameOrAddress;
            _remotePort = remotePort;
            _tcpMillisecondsTimeout = tcpMillisecondsTimeout;
        }
        public virtual string HostNameOrAddress
        {
            get { return _hostNameOrAddress; }
            set { _hostNameOrAddress = value; }
        }

        public virtual int TcpMillisecondsTimeout
        {
            get { return _tcpMillisecondsTimeout; }
        }

        public int RemotePort => _remotePort;

        public virtual PingTestResult TestPing()
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

        public virtual TcpTestResult TestTcpPort()
        {
            var pingResult = TestPing();
            var tcpResult = new TcpTestResult(pingResult, _remotePort);
            
            using (TcpClient tcpTester = new TcpClient())
            {
                IAsyncResult result = tcpTester.BeginConnect(_hostNameOrAddress, _remotePort, null, null);
                tcpResult.TcpTestSuccessed = result.AsyncWaitHandle.WaitOne(_tcpMillisecondsTimeout);
            }

            return tcpResult;
        }

        public virtual INetTestResult Test()
        {
            if (_remotePort.Equals(NO_PORT_SELECTED))
            {
                return TestPing();
            }
            return TestTcpPort();
        }
    }
}