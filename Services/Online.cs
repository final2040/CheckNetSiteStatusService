namespace Services
{
    public class Online : State
    {
        public Online(NetworkMonitor context) : base(context) { }

        public override void Test()
        {
            if (!_context.NetworkTest.Test().Success)
            {
                _context.CurrentState = _context.RetryingOnline;
                _context.RetryingOnline.ResetTimer();
            }
        }
    }
}