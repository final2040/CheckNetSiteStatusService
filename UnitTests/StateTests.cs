using System.Net.NetworkInformation;
using Data;
using Moq;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class StateTests
    {
        private PingTestResult _failResult;
        private PingTestResult _successResult;

        [SetUp]
        private void SetUp()
        {
            _failResult = new PingTestResult()
            {
                Status = IPStatus.TimedOut,
                Bytes = 0,
                Host = "172.28.129.100",
                Time = 0,
                Ttl = 128
            };

            _successResult = new PingTestResult()
            {
                Status = IPStatus.Success,
                Bytes = 32,
                Host = "172.28.129.100",
                Time = 2,
                Ttl = 128
            };
        }

        [Test]
        private void Test1()
        {
            Mock<NetworkMonitorV2> monitorMock = new Mock<NetworkMonitorV2>();
            Mock<NetworkTest>
            
        }
         
    }
}