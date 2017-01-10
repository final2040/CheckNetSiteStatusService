using System;
using System.Collections.Generic;
using System.Linq;
using Data.NetworkTest;

namespace Services.State
{
    /// <summary>
    /// Representa el estado de Reintento a una conexión que pasó a modo offline
    /// </summary>
    public class RetryingOnline : State
    {
        private const int PERCENT_RATIO = 90;
        private DateTime _currentTime;
        private readonly List<INetTestResult> _results = new List<INetTestResult>();

        public RetryingOnline(NetworkMonitor context) : base(context) { }

        /// <summary>
        /// Prueba la conexión
        /// </summary>
        public override void Test()
        {
            var testResult = _context.NetworkTest.Test();
            _results.Add(testResult);
            
            if (DateTime.Now.Subtract(_currentTime).TotalMilliseconds > _context.TimeOut)
            {
                int percentFail = _results.Count(r => !r.Success)*100/_results.Count;
                if (percentFail >= PERCENT_RATIO)
                {
                    _context.CurrentState = _context.Offline;
                    _context.ConnectionLost(_results);
                }
                else
                    _context.CurrentState = _context.Online;
            }
        }
        /// <summary>
        /// Restablece el timer interno y limpia los resultados anteriores
        /// </summary>
        public override void Reset()
        {
            _currentTime = DateTime.Now;
            _results.Clear();
            
        }
    }
}