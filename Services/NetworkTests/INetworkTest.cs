using Data;
using Data.NetworkTest;

namespace Services.NetworkTests
{
    /// <summary>
    /// Interfaz para los modulos de prueba de red
    /// </summary>
    public interface INetworkTest
    {
        string HostNameOrAddress { get; set; }
        INetTestResult Test();
        TestConfigurationBase TestConfiguration { get; }
    }
}