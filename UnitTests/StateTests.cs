using Moq;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class StateTests
    {
        [Test]
        public void Test1()
        {
            Mock<INetworkTest> testMock = new Mock<INetworkTest>();
            Mock<NetworkMonitor2> netMock = new Mock<NetworkMonitor2>(testMock.Object);

        }
    }
}