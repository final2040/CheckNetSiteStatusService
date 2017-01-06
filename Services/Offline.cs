namespace Services
{
    public class Offline : State
    {
        public Offline(NetworkMonitor context) : base(context)
        {
        }

        public override void Test()
        {
            if (_context.NetworkTest.Test().Success)
            {
                _context.CurrentState = _context.RetryingOffline;
                _context.RetryingOffline.ResetTimer();
            }
        }
    }
}