using System;
using System.Collections.Generic;
using System.Threading;
using Data;
using Data.NetworkTest;
using Services.Log;
using Services.NetworkTests;
using Services.State;

namespace Services
{
    /// <summary>
    /// Monitorea un servicio/conexión de red
    /// </summary>
    public class NetworkMonitor
    {
        #region Fields
        private State.State _currentStatus;
        private readonly int _timeOut;
        private readonly string _testNetworkName;
        private INetworkTest _networkTest;
        private readonly int _timeBetweenTests;
        private const string DEFAULT_NAME = "TestSite";
        private const int DEFAULT_TIME_BETWEEN_TESTS = 2000;
        private const int DEFAULT_TIMEOUT = 120000;

        private readonly State.State _online;
        private readonly State.State _offline;
        private readonly State.State _retryingOnline;
        private readonly State.State _retryingOffline;

        #endregion

        #region Events
        public event EventHandler OnStatusChange;
        public event EventHandler OnConnectionLost;
        public event EventHandler OnConnectionBack;
        #endregion

        #region Properties
        /// <summary>
        /// Propiedad que obtiene o establece la prueba de red a realizar
        /// </summary>
        public virtual INetworkTest NetworkTest
        {
            get { return _networkTest; }
            set { _networkTest = value; }
        }

        /// <summary>
        /// Propiedad que Obtiene el tiempo de espera entre cada prueba
        /// </summary>
        public virtual int TimeBetweenTests
        {
            get { return _timeBetweenTests; }
        }

        /// <summary>
        /// Propiedad que obtiene o establece el estado actual de la conexión.
        /// </summary>
        public virtual State.State CurrentState
        {
            get { return _currentStatus; }
            set
            {
                if (_currentStatus != value)
                {
                    _currentStatus = value;
                    OnStatusChange?.Invoke(this, new ChangeEventArgs(value.ToString(),_testNetworkName));
                }
            }
        }
        /// <summary>
        /// Propiedad que obtiene o establece el tiempo de espera maximo de reintentos antes de cambiar de estado
        /// en linea a fuera de linea o visceversa 
        /// </summary>
        public virtual int TimeOut
        {
            get { return _timeOut; }
        }

        /// <summary>
        /// Propiedad que obtiene el nombre del servicio/conexión de red a probar
        /// </summary>
        public virtual string TestNetworkName
        {
            get { return _testNetworkName; }
        }

        /// <summary>
        /// Propiedad que obtiene el estado en linea
        /// </summary>
        public virtual State.State Online
        {
            get { return _online; }
        }

        /// <summary>
        /// Propiedad que obtiene el estado fuera de linea
        /// </summary>
        public virtual State.State Offline
        {
            get { return _offline; }
        }

        /// <summary>
        /// Propiedad que obtiene el estado Reintentando en Linea
        /// </summary>
        public virtual State.State RetryingOnline
        {
            get { return _retryingOnline; }
        }

        /// <summary>
        /// Propiedad que obtiene el estado Reintentando Fuera de linea
        /// </summary>
        public virtual State.State RetryingOffline
        {
            get { return _retryingOffline; }
        }

        #endregion

        #region Constructors

        public NetworkMonitor(string ip) : this(new PingTest(ip), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(string ip, int port) : this(new TcpTest(ip, port), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests) : this(new TcpTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, string name) : this(new TcpTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds, string name) : this(new TcpTest(ip, port), timeBetweenTests, timeUntilFailSeconds, name) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds) : this(new TcpTest(ip, port), timeBetweenTests, timeUntilFailSeconds, DEFAULT_NAME) { }
        public NetworkMonitor(INetworkTest tester) : this(tester, DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(INetworkTest tester, int timeBetweenTests) : this(tester, timeBetweenTests, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(INetworkTest tester, TestConfig testConfiguration, string testName) : this(tester, testConfiguration.WaitTimeSeconds * 1000, testConfiguration.TimeOutSeconds * 1000, testName) { }
        public NetworkMonitor(INetworkTest tester, int timeBetweenTests, string name) : this(tester, timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitor(INetworkTest tester, int timeBetweenTests, int timeOut, string testNetworkName)
        {
            _testNetworkName = testNetworkName;
            _networkTest = tester;
            _timeBetweenTests = timeBetweenTests;
            _timeOut = timeOut;

            _online = new Online(this);
            _retryingOnline = new RetryingOnline(this);
            _offline = new Offline(this);
            _retryingOffline = new RetryingOffline(this);

            _currentStatus = _online;
        }
        #endregion

        /// <summary>
        /// Inicializa la prueba
        /// </summary>
        public virtual void Start()
        {
            while (true)
            {
                _currentStatus.Test();
                Thread.Sleep(_timeBetweenTests);
            }
            // ReSharper disable once FunctionNeverReturns
        }
        /// <summary>
        /// Dispara el evento de conexion perdida
        /// </summary>
        /// <param name="retryResults"></param>
        public virtual void ConnectionLost(List<INetTestResult> retryResults)
        {
            OnConnectionLost?.Invoke(this, new TestNetworkEventArgs(_testNetworkName, retryResults, NetworkTest.HostNameOrAddress, NetworkTest.TestConfiguration?.ToString()));
        }

        /// <summary>
        /// Dispara el evento de conexión Recuperada
        /// </summary>
        /// <param name="restryResults"></param>
        public virtual void ConnectionRestore(List<INetTestResult> restryResults)
        {
            OnConnectionBack?.Invoke(this, new TestNetworkEventArgs(_testNetworkName,restryResults,NetworkTest.HostNameOrAddress, NetworkTest.TestConfiguration?.ToString()));
        }
    }
}
