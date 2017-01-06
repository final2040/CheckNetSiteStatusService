using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Services
{
    public class RetryingOffline : State
    {
        private DateTime _currentTime;
        private readonly List<INetTestResult> _results = new List<INetTestResult>();


        public RetryingOffline(NetworkMonitor context) : base(context)
        {
        }

        public override void Test()
        {
            var testResult = _context.NetworkTest.Test();
            _results.Add(testResult);
            if (DateTime.Now.Subtract(_currentTime).Milliseconds > _context.TimeOut)
            {
                int failPercent = _results.Count(r => !r.Success) * 100 / _results.Count;
                if (failPercent < 90)
                {
                    _context.CurrentState = _context.Online;
                    _context.ConnectionRestore(_results);
                }
                else
                    _context.CurrentState = _context.Offline;
            }
            
        }

        public override void ResetTimer()
        {
            _currentTime = DateTime.Now;
            _results.Clear();
        }
    }
}