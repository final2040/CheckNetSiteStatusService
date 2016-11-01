namespace Data
{
    public class PingTestConfiguration : TestConfigurationBase
    {
        public override string ToString()
        {
            return $"PingTest\r\nNombre: {Name}\r\nHost: {Host}";
        }
    }
}