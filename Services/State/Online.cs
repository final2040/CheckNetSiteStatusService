namespace Services.State
{
    /// <summary>
    /// Representa el estado En linea de la conexión probada.
    /// </summary>
    public class Online : State
    {
        public Online(NetworkMonitor context) : base(context) { }

        /// <summary>
        /// Prueba la conexión
        /// </summary>
        public override void Test()
        {
            if (!_context.NetworkTest.Test().Success)
            {
                _context.CurrentState = _context.RetryingOnline;
                _context.RetryingOnline.Reset();
            }
        }
    }
}