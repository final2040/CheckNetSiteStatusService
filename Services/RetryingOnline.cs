using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Services
{
    public class RetryingOnline : State
    {
        private DateTime _currentTime;
        private readonly List<INetTestResult> _results = new List<INetTestResult>();

        public RetryingOnline(NetworkMonitor context) : base(context) { }

        public override void Test()
        {
            var testResult = _context.NetworkTest.Test();
            _results.Add(testResult);
            
            if (DateTime.Now.Subtract(_currentTime).Milliseconds > _context.TimeOut)
            {
                int percentSuccess = _results.Count(r => r.Success == true)*100/_results.Count;
                if (percentSuccess < 90)
                {
                    _context.CurrentState = _context.Offline;
                    _context.ConnectionLost(_results);
                }
                else
                    _context.CurrentState = _context.Online;
            }
        }

        public override void ResetTimer()
        {
            _currentTime = DateTime.Now;
            _results.Clear();
            
        }
    }
}