using System;
using System.Collections.Generic;
using System.Linq;
using Data.NetworkTest;

namespace Services.State
{
    /// <summary>
    /// Representa el estado de Reintento para una conexión fuera de linea que recuperó la conexion
    /// </summary>
    public class RetryingOffline : State
    {
        private const int PERCENT_RATIO = 90;
        private DateTime _currentTime;
        private readonly List<INetTestResult> _results = new List<INetTestResult>();


        public RetryingOffline(NetworkMonitor context) : base(context)
        {
        }
        /// <summary>
        /// Prueba la conexión
        /// </summary>
        public override void Test()
        {
            var testResult = _context.NetworkTest.Test();
            _results.Add(testResult);
            if (DateTime.Now.Subtract(_currentTime).TotalMilliseconds > _context.TimeOut)
            {
                int successPercent = _results.Count(r => r.Success) * 100 / _results.Count;
                if (successPercent >= PERCENT_RATIO)
                {
                    _context.CurrentState = _context.Online;
                    _context.ConnectionRestore(_results);
                }
                else
                    _context.CurrentState = _context.Offline;
            }
            
        }
        /// <summary>
        /// Reinicia el contador para el timeout interno de las pruebas y limpia los resultados de la prueba anterior
        /// </summary>
        public override void Reset()
        {
            _currentTime = DateTime.Now;
            _results.Clear();
        }
    }
}