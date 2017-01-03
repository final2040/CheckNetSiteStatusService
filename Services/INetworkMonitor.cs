using System;
using Data;

namespace Services
{
    public interface INetworkMonitor
    {
        NetworkTest NetworkTest { get; }
        int TimeBetweenTests { get; }
        int TimeoutMiliseconds { get; }

        event EventHandler OnConnectionBack;
        event EventHandler OnConnectionLost;
        event EventHandler OnStatusChange;

        void Start();
    }
}