namespace Services.State
{
    /// <summary>
    /// Clase abstracta que define la interfaz de los diferentes estados de conexión
    /// </summary>
    public abstract class State
    {
        protected NetworkMonitor _context;

        protected State(NetworkMonitor context)
        {
            this._context = context;
        }
        /// <summary>
        /// Prueba la conexión
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
        /// Devuelve la representación String de este objeto.
        /// </summary>
        /// <returns>Representación en texto de este objeto.</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}