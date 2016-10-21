using System;

namespace Data
{
    public class ChangeEventArgs : EventArgs
    {
        public ChangeEventArgs(ConnectionStatus currentStatus,string name)
        {
            CurrentStatus = currentStatus;
            Name = name;
        }

        public string Name { get; set; }
        public ConnectionStatus CurrentStatus { get; set; }
    }
}