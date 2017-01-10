using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using NUnit.Framework;
using Services;
using Data;
using Data.NetworkTest;
using Services.Log;
using Services.NetworkTests;

namespace UnitTests
{
    [TestFixture]
    public class NetworkTests
    {
        [Test]
        public void ShouldReturnSuccessOnHostUp()
        {
            // arrange
            var network = new PingTest("172.28.129.100");
            
            // act
            var currentResult = network.Test();

            // assert
            Debug.Print(currentResult.ToString());
            Assert.AreEqual(IPStatus.Success,currentResult.Status);
        }

        [Test]
        public void ShouldReturnTimedOutOnHostDown()
        {
            // arrange
            var network = new PingTest("172.28.129.227");

            // act
            var currentResult = network.Test();

            // assert
            Debug.Print(currentResult.ToString());
            Assert.AreEqual(IPStatus.TimedOut, currentResult.Status);
        }

        [Test]
        public void ShouldReturnTcpTestResultsObject()
        {
            // arrange
            var network = new TcpTest("172.28.129.100",80);

            // act
            var result = network.Test();

            // assert
            Debug.Print(result.ToString());
            Assert.IsInstanceOf(typeof(TcpTestResult),result);
        }

        [Test]
        public void ShouldReturnTcpTestSuccessOnOpenPort()
        {
            // arrange
            var network = new TcpTest("172.28.129.100", 8080);

            // act
            var result = network.Test();

            // assert
            Debug.Print(result.ToString());
            Assert.IsTrue(((TcpTestResult)result).TcpTestSuccessed);
        }

        [Test]
        public void ShouldReturnTcpTestSuccessFalseOnClosedPort()
        {
            // arrange
            var network = new TcpTest("172.28.129.100", 123);

            // act
            var result = network.Test();

            // assert
            Debug.Print(result.ToString());
            Assert.IsFalse(((TcpTestResult)result).TcpTestSuccessed);
        }

        [Test]
        public void ShouldReturnTcpTestResultsObject1()
        {
            // arrange
            var network = new TcpTest("172.28.129.100", 80);

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
            var network = new PingTest("172.28.129.100");

            // act
            var result = network.Test();

            // assert
            Debug.Print(result.ToString());
            Assert.IsInstanceOf(typeof(PingTestResult), result);
        }
    }
}
