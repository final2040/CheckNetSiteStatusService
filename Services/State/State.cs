namespace Services.State
{
    /// <summary>
    /// Clase abstracta que define la interfaz de los diferentes estados de conexi�n
    /// </summary>
    public abstract class State
    {
        protected NetworkMonitor _context;

        protected State(NetworkMonitor context)
        {
            this._context = context;
        }
        /// <summary>
        /// Prueba la conexi�n
        /// </summary>
        public abstract void Test();
        /// <summary>
        /// Restablece el timer interno y limpia los estados anteriores.
        /// No soportada en los estados Online y Offline
        /// </summary>
        public virtual void Reset()
        {
            throw new System.NotSupportedException("Operation not suported for this state");
        }

        /// <summary>
        /// Devuelve la representaci�n String de este objeto.
        /// </summary>
        /// <returns>Representaci�n en texto de este objeto.</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}