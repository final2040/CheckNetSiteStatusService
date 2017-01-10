using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using Data;
using Data.NetworkTest;
using Moq;
using NUnit.Framework;
using Services;
using Services.NetworkTests;
using Services.State;

namespace UnitTests
{
    [TestFixture]
    public class StateTests
    {
        private PingTestResult _successPingResult;
        private PingTestResult _unsuccessPingResult;
        private Mock<INetworkTest> _testMock;
        private Mock<NetworkMonitor> _monitorMock;
        private Mock<State> _stateMock;

        [SetUp]
        public void Setup()
        {
            _successPingResult = new PingTestResult
            {
                Bytes = 32,
                Host = "172.28.129.100",
                Status = IPStatus.Success,
                Time = 2,
                Ttl = 128
            };
            _unsuccessPingResult = new PingTestResult
            {
                Bytes = 0,
                Host = "172.28.129.100",
                Status = IPStatus.TimedOut,
                Time = 0,
                Ttl = 128
            };
            _testMock = new Mock<INetworkTest>();
            _monitorMock = new Mock<NetworkMonitor>(_testMock.Object);
            _monitorMock.Setup(mm => mm.Online).Returns(new Online(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.RetryingOnline).Returns(new RetryingOnline(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.Offline).Returns(new Offline(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.RetryingOffline).Returns(new RetryingOffline(_monitorMock.Object));
            _stateMock = new Mock<State>(_monitorMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _testMock = null;
            _monitorMock = null;
            _stateMock = null;
        }

        [Test]
        public void Online_OnConnectionFail_ShouldChangeStateToOnlineRetrying()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new Online(_monitorMock.Object));

            Online state = new Online(_monitorMock.Object);
            
            // act
            state.Test();

            // assert
            Assert.IsInstanceOf(typeof(RetryingOnline), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void Online_OnConnectionFail_ShouldResetRetryingOnlineTimer()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            
            _stateMock.Setup(sm => sm.Reset()).Verifiable();

            _monitorMock.Setup(mm => mm.RetryingOnline).Returns(_stateMock.Object);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new Online(_monitorMock.Object));

            Online state = new Online(_monitorMock.Object);

            // act
            state.Test();

            // assert
            _stateMock.Verify();
        }

        [Test]
        public void Online_OnConnectionSuccess_ShouldMaintainOnlineState()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new Online(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            Online state = new Online(_monitorMock.Object);

            // act
            state.Test();

            // assert
            Assert.IsInstanceOf(typeof(Online), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void RetryingOnline_On90PercentOfConnectionSuccess_ShouldSetOnlineState()
        {
            // arrange
            State state = new RetryingOnline(_monitorMock.Object);
            state.Reset();
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(100);
            _monitorMock.SetupProperty(mm => mm.CurrentState, state);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

           
            // act
            state.Test();
            Thread.Sleep(10);
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            for (int i = 0; i < 9; i++)
            {
                state.Test();
                Thread.Sleep(11);
            }
            // assert
            Assert.IsInstanceOf(typeof(Online), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void RetryingOnline_On90PercentOfConnectionFail_ShouldSetOfflineState()
        {
            // arrange
            State state = new RetryingOnline(_monitorMock.Object);
            state.Reset();
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(100);
            _monitorMock.SetupProperty(mm => mm.CurrentState, state);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            // act
            state.Test();
            
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);

            for (int i = 0; i < 8; i++)
            {
                state.Test();
            }
            Thread.Sleep(150);
            state.Test();
         
            // assert
            Assert.IsInstanceOf(typeof(Offline), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void RetryingOnline_OnConnectionNotRestoreBeforeTimeout_ShouldCallConnectionLostMethod()
        {
            // arrange
            State state = new RetryingOnline(_monitorMock.Object);
            state.Reset();
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(15);
            _monitorMock.SetupProperty(mm => mm.CurrentState, state);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);
            _monitorMock.Setup(mm => mm.ConnectionLost(It.IsAny<List<INetTestResult>>())).Verifiable();
            
            // act
            state.Test();
            Thread.Sleep(15);
            state.Test();

            // assert
            Assert.IsInstanceOf(typeof(Offline), _monitorMock.Object.CurrentState);
            _monitorMock.Verify();
        }

        [Test]
        public void RetryingOnline_OnConnectionNotRestoreBeforeTimeout_ShouldPassTestResultsToConnectionLostMethod()
        {
            // arrange
            State state = new RetryingOnline(_monitorMock.Object);
            state.Reset();
            List<INetTestResult> result = new List<INetTestResult>();
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(15);
            _monitorMock.SetupProperty(mm => mm.CurrentState, state);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);
            _monitorMock.Setup(mm => mm.ConnectionLost(It.IsAny<List<INetTestResult>>())).Callback((List<INetTestResult> r) => result = r);
            
            // act
            state.Test();
            Thread.Sleep(10);
            state.Test();
            Thread.Sleep(15);
            state.Test();
            Thread.Sleep(10);
            state.Test();
            Thread.Sleep(15);
            
            // assert
            Assert.AreEqual(4, result.Count);
            result.ForEach(r => Debug.Print(r.ToString()));
            _monitorMock.Verify();
        }

        [Test]
        public void Ofline_OnConnectionSuccess_ShouldChangeStateToOfflineRetrying()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new Offline(_monitorMock.Object));

            var state = new Offline(_monitorMock.Object);

            // act
            state.Test();

            // assert
            Assert.IsInstanceOf(typeof(RetryingOffline), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void Offline_OnConnectionSuccess_ShouldResetRetryingOfflineTimer()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            _stateMock.Setup(sm => sm.Reset()).Verifiable();

            _monitorMock.Setup(mm => mm.RetryingOffline).Returns(_stateMock.Object);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new Offline(_monitorMock.Object));

            var state = new Offline(_monitorMock.Object);

            // act
            state.Test();

            // assert
            _stateMock.Verify();
        }

        [Test]
        public void Offline_OnConnectionFail_ShouldMaintainOfflineState()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new Offline(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            var state = new Offline(_monitorMock.Object);

            // act
            state.Test();

            // assert
            Assert.IsInstanceOf(typeof(Offline), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void RetryingOffline_On90PercentOfConnectionFail_ShouldSetOfflineState()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(100);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new RetryingOffline(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            State state = new RetryingOffline(_monitorMock.Object);
            state.Reset();
            // act
            state.Test();
            Thread.Sleep(10);
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            for (int i = 0; i < 9; i++)
            {
                state.Test();
                Thread.Sleep(11);
            }
         
            // assert
            Assert.IsInstanceOf(typeof(Offline), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void RetryingOffline_On90PercentOfConnectionSuccess_ShouldSetOnlineState()
        {
            // arrange
            _testMock.Setup(tm => tm.Test()).Returns(_unsuccessPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(100);
            _monitorMock.SetupProperty(mm => mm.CurrentState, new RetryingOnline(_monitorMock.Object));
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);

            State state = new RetryingOnline(_monitorMock.Object);
            state.Reset();

            // act
            state.Test();
            Thread.Sleep(10);
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);

            for (int i = 0; i < 9; i++)
            {
                state.Test();
                Thread.Sleep(11);
            }

            // assert
            Assert.IsInstanceOf(typeof(Online), _monitorMock.Object.CurrentState);
        }

        [Test]
        public void RetryingOffline_OnConnectionNotRestoreBeforeTimeout_ShouldCallConnectionRestoreMethod()
        {
            // arrange
            State state = new RetryingOffline(_monitorMock.Object);
            state.Reset();
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(15);
            _monitorMock.SetupProperty(mm => mm.CurrentState, state);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);
            _monitorMock.Setup(mm => mm.ConnectionRestore(It.IsAny<List<INetTestResult>>())).Verifiable();

            // act
            Thread.Sleep(20);
            state.Test();
            

            // assert
            Assert.IsInstanceOf(typeof(Online), _monitorMock.Object.CurrentState);
            _monitorMock.Verify();
        }

        [Test]
        public void RetryingOffline_OnConnectionRestoreBeforeTimeout_ShouldPassTestResultsToConnectionLostMethod()
        {
            // arrange

            List<INetTestResult> result = new List<INetTestResult>();
            State state = new RetryingOffline(_monitorMock.Object);
            _testMock.Setup(tm => tm.Test()).Returns(_successPingResult);
            _monitorMock.Setup(mm => mm.TimeOut).Returns(15);
            _monitorMock.SetupProperty(mm => mm.CurrentState, state);
            _monitorMock.Setup(mm => mm.NetworkTest).Returns(_testMock.Object);
            _monitorMock.Setup(mm => mm.ConnectionRestore(It.IsAny<List<INetTestResult>>())).Callback((List<INetTestResult> r) => result = r);
            
            // act
            state.Test();
            Thread.Sleep(10);
            state.Test();
            Thread.Sleep(15);
            state.Test();
            Thread.Sleep(10);
            state.Test();
            Thread.Sleep(15);

            // assert
            Assert.AreEqual(4, result.Count);
            result.ForEach(r => Debug.Print(r.ToString()));
            _monitorMock.Verify();
        }

    }
}