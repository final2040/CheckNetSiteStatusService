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
            try
            {
                Console.CancelKeyPress += Console_CancelKeyPress;
                Logger.GetLogger().LogWriter = new ConsoleLogWriter();
                _monitor = new Monitor();
                _monitor.Start();
            }
            catch(Exception ex)
            {
                Logger.GetLogger().WriteError(ex.Message);
                Logger.GetLogger().WriteWarning("La aplicación finalizó con errores...");
            }

            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
           _monitor.Stop();
        }
    }
}
