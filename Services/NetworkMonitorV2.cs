using System;
using Data;

namespace Services
{
    public class NetworkMonitorV2:INetworkMonitor
    {
        public virtual NetworkTest NetworkTest { get; }
        public virtual int TimeBetweenTests { get; }
        public virtual int TimeoutMiliseconds { get; }
        public virtual event EventHandler OnConnectionBack;
        public virtual event EventHandler OnConnectionLost;
        public virtual event EventHandler OnStatusChange;

        public virtual void Start()
        {
            throw new NotImplementedException();
        }
    }
}