using System;
using Services;
using Services.Log;

namespace ConsoleApplication
{
    internal class Program
    {
        private static Monitor _monitor;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            Logger.GetLogger().LogWriter = new ConsoleLogWriter();
            _monitor = new Monitor();
            _monitor.Start();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
           _monitor.Stop();
        }
    }
}
