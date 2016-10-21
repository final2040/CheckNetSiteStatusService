using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security;
using NUnit.Framework;
using Moq;
using Services;
using Data;

namespace UnitTests
{
    [TestFixture]
    public class NetworkTests
    {
        [Test]
        public void ShouldReturnSuccessOnHostUp()
        {
            // arrange
            var network = new NetworkTest("172.28.129.100");
            
            // act
            var currentResult = network.TestPing();

            // assert
            Debug.Print(currentResult.ToString());
            Assert.AreEqual(IPStatus.Success,currentResult.Status);
        }

        [Test]
        public void ShouldReturnUnreachableOnHostDown()
        {
            // arrange
            var network = new NetworkTest("172.28.129.225");

            // act
            var currentResult = network.TestPing();

            // assert
            Debug.Print(currentResult.ToString());
            Assert.AreEqual(IPStatus.DestinationHostUnreachable, currentResult.Status);
        }

        [Test]
        public void ShouldReturnTcpTestResultsObject()
        {
            // arrange
            var network = new NetworkTest("172.28.129.100",80);

            // act
            var result = network.TestTcpPort();

            // assert
            Debug.Print(result.ToString());
            Assert.IsInstanceOf(typeof(TcpTestResult),result);
        }

        [Test]
        public void ShouldReturnTcpTestSuccessOnOpenPort()
        {
            // arrange
            var network = new NetworkTest("172.28.129.100", 8080);

            // act
            var result = network.TestTcpPort();

            // assert
            Debug.Print(result.ToString());
            Assert.IsTrue(result.TcpTestSuccessed);
        }

        [Test]
        public void ShouldReturnTcpTestSuccessFalseOnClosedPort()
        {
            // arrange
            var network = new NetworkTest("172.28.129.100", 123);

            // act
            var result = network.TestTcpPort();

            // assert
            Debug.Print(result.ToString());
            Assert.IsFalse(result.TcpTestSuccessed);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        [TestCase("172.28.129.257")]
        [TestCase("257.28.129.0")]
        [TestCase("172.999.129.0")]
        [TestCase("172.28.777.0")]
        public void ShouldRaiseExceptionOnInvalidIp(string ip)
        {
            // arrange
            NetworkTest networkTest = new NetworkTest(ip);

            // act

            // assert
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCase(-25)]
        [TestCase(999999)]
        [TestCase(65536)]
        [TestCase(-1)]
        public void ShouldRaiseExceptionOnInvalidPort(int remotePort)
        {
            // arrange
            NetworkTest networkTest = new NetworkTest("172.28.129.100", remotePort);

            // act

            // assert
        }

        [Test]
        public void ShouldReturnTcpTestResultsObject1()
        {
            // arrange
            var network = new NetworkTest("172.28.129.100", 80);

            // act
            var result = network.Test();

            // assert
            Debug.Print(result.ToString());
            Assert.IsInstanceOf(typeof(TcpTestResult), result);
        }

        [Test]
        public void ShouldReturnPingTestResultsObject1()
        {
            // arrange
            var network = new NetworkTest("172.28.129.100");

            // act
            var result = network.Test();

            // assert
            Debug.Print(result.ToString());
            Assert.IsInstanceOf(typeof(PingTestResult), result);
        }
    }
}
