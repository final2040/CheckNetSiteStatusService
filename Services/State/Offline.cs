namespace Services.State
{
    /// <summary>
    /// Representa el estado Fuera de Linea de la conexi�n probada
    /// </summary>
    public class Offline : State
    {
        public Offline(NetworkMonitor context) : base(context)
        {
        }

        /// <summary>
        /// Prueba la conexi�n
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