using Data;

namespace Services
{
    public interface INetworkTest
    {
        string HostNameOrAddress { get; set; }
        INetTestResult Test();
    }
}