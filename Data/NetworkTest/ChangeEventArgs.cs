﻿namespace Data.NetworkTest
{
    public class ChangeEventArgs : System.EventArgs
    {
        public ChangeEventArgs(string currentStatus,string name)
        {
            CurrentStatus = currentStatus;
            Name = name;
        }

        public ChangeEventArgs(string currentStatus):this(currentStatus,"")
        {
        }

        public string Name { get; set; }
        public string CurrentStatus { get; set; }
    }
}