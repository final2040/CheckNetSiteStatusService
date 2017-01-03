using System;
using Data;

namespace Services
{
    public interface INetworkMonitor
    {
        event EventHandler OnStatusChange;
        event EventHandler OnConnectionLost;
        event EventHandler OnConnectionBack;

        INetworkTest NetworkTest { get; set; }
        int TimeBetweenTests { get; }
        int TimeOut { get; }

        void Start();
    }
}