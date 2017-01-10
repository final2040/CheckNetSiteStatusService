using System;
using System.Net.NetworkInformation;

namespace Data.NetworkTest
{
    public class TcpTestResult:INetTestResult
    {
        public int Bytes { get; set; }
        public string Host { get; set; }
        public IPStatus Status { get; set; }
        public long Time { get; set; }
        public int Ttl { get; set; }
        public DateTime TestDateTime { get; }
        public bool TcpTestSuccessed { get; set; }
        public int RemotePort { get; set; }

        public bool Success
        {
            get { return TcpTestSuccessed; }
        }

        public TcpTestResult(PingTestResult pingResult, int remotePort)
        {
            this.RemotePort = remotePort;
            this.Host = pingResult.Host;
            this.Bytes = pingResult.Bytes;
            this.Status = pingResult.Status;
            this.Time = pingResult.Time;
            this.Ttl = pingResult.Ttl;
            this.TestDateTime = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            var objectToCompare = (TcpTestResult) obj;
            var result = base.Equals(obj);
            result &= objectToCompare.TcpTestSuccessed.Equals(this.TcpTestSuccessed);
            result &= objectToCompare.RemotePort.Equals(this.RemotePort);

            return result;
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            hashCode ^= RemotePort.GetHashCode();
            hashCode ^= TcpTestSuccessed.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format(
                "Test Time: {0}" + Environment.NewLine +
                "Remote Address: {1}" + Environment.NewLine +
                "Remote Port: {2}" + Environment.NewLine +
                "PingStatus: {3}" + Environment.NewLine +
                "PingReplyTime: {4}" + Environment.NewLine +
                "TcpTestSuccessed: {5}",TestDateTime.ToString("G"), Host, RemotePort, Status, Time, TcpTestSuccessed);
        }
        
    }
}