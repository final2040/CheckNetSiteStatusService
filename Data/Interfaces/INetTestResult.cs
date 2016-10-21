using System;
using System.Net.NetworkInformation;

namespace Data.Interfaces
{
    public interface INetTestResult
    {
        int Bytes { get; set; }
        string Host { get; set; }
        IPStatus Status { get; set; }
        long Time { get; set; }
        int Ttl { get; set; }
        DateTime TestDateTime { get; }
        bool Success { get; }
    }
}