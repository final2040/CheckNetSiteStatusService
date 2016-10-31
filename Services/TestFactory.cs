using System;
using System.Collections.Generic;
using Data;

namespace Services
{
    public static class TestFactory
    {
        
        
        public static INetworkTest CreateInstance(TestConfigurationBase config)
        {
            if (config.GetType() == typeof(PingTestConfiguration))
            {
                return new PingTest((PingTestConfiguration)config);
            }
            if(config.GetType() == typeof(TcpTestConfiguration))
            {
                return new TcpTest((TcpTestConfiguration)config);
            }

            throw new InvalidOperationException("No se puede reconocer el objeto de configuración provisto.");
        }
    }
}