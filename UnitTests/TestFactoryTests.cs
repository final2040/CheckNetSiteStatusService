using Data;
using Data.NetworkTest;
using NUnit.Framework;
using Services;
using Services.Log;
using Services.NetworkTests;

namespace UnitTests
{
    [TestFixture]
    public class TestFactoryTests
    {
        [Test]
        public void ShouldCreateANewInstanceOfTestPingClass()
        {
            // arrange
            PingTestConfiguration config = new PingTestConfiguration() {Host = "172.28.129.100", Name = "Prueba"};

            // act
            INetworkTest tester = TestFactory.CreateInstance(config);

            // assert
            Assert.IsInstanceOf(typeof(PingTest), tester);
        }

        [Test]
        public void ShouldCreateANewInstanceOfTestTcpClass()
        {
            // arrange
            TestConfigurationBase config = new TcpTestConfiguration() { Host = "172.28.129.100", Name = "Prueba", Port = 20, TimeOutMilliSeconds = 1200};

            // act
            INetworkTest tester = TestFactory.CreateInstance(config);

            // assert
            Assert.IsInstanceOf(typeof(TcpTest), tester);
        }
    }
}