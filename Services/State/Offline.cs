namespace Services.State
{
    /// <summary>
    /// Representa el estado Fuera de Linea de la conexión probada
    /// </summary>
    public class Offline : State
    {
        public Offline(NetworkMonitor context) : base(context)
        {
        }

        /// <summary>
        /// Prueba la conexión
        /// </summary>
        public override void Test()
        {
            if (_context.NetworkTest.Test().Success)
            {
                _context.CurrentState = _context.RetryingOffline;
                _context.RetryingOffline.Reset();
            }
        }
    }
}