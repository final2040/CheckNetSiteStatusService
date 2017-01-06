namespace Services
{
    public abstract class State
    {
        protected NetworkMonitor _context;

        protected State(NetworkMonitor context)
        {
            this._context = context;
        }
        
        public abstract void Test();
        public virtual void ResetTimer()
        {
            throw new System.NotSupportedException("Operation not suported for this state");
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}