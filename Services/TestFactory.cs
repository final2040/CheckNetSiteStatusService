using System;
using System.Collections.Generic;
using Data;
using Data.NetworkTest;
using Services.Log;
using Services.NetworkTests;

namespace Services
{
    /// <summary>
    /// Crea instancias para los diferentes tipos de prueba de red.
    /// </summary>
    public static class TestFactory
    {
        /// <summary>
        /// Devuelve una prueba de red.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
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