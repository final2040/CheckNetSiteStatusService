using System;
using System.Net.NetworkInformation;

namespace Data
{
    public class PingTestResult : INetTestResult
    {

        public PingTestResult():this(0,string.Empty,0,0){}

        public PingTestResult(int bytes, string host, int time, int ttl)
        {
            Bytes = bytes;
            Host = host;
            Time = time;
            Ttl = ttl;
            TestDateTime = DateTime.Now;
        }

        public PingTestResult(string host, IPStatus status)
        {
            Host = host;
            Status = status;
        }

        public int Bytes { get; set; }
        public string Host { get; set; }
        public IPStatus Status { get; set; }
        public long Time { get; set; }
        public int Ttl { get; set; }
        public DateTime TestDateTime { get; protected set; }

        public bool Success
        {
            get { return Status == IPStatus.Success; }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType() && obj == null)
            {
                return false;
            }
            bool result = true;
            PingTestResult objectToCompare = (PingTestResult)obj;

            result &= objectToCompare.Status.Equals(Status);
            result &= objectToCompare.Host.Equals(Host);
            result &= objectToCompare.Ttl.Equals(Ttl);
            result &= objectToCompare.Bytes.Equals(Bytes);
            return result;
        }

        public override int GetHashCode()
        {
            int hash = Status.GetHashCode();
            hash ^= Host.GetHashCode();
            hash ^= Ttl.GetHashCode();
            hash ^= Bytes.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (Status == IPStatus.Success)
            {
                return string.Format("{0} Respuesta desde {1}: bytes={2} tiempo={3}ms TTL={4}", TestDateTime.ToString("G"), Host, Bytes, Time, Ttl);
            }
            return String.Format("No se pudo conectar al host {0}, {1}", Host, Status);

        }
    }
}