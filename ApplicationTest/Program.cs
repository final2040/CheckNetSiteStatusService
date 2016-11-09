using System;
using Services;

namespace ApplicationTest
{
    internal class Program
    {
        private static Test _myObj;
        private static Logger _logger = Logger.GetLogger();
        private static void Main(string[] args)
        {
            _logger.LogWriter = new ConsoleLogWriter();
            try
            {
                _myObj = new Test();
                _myObj.OnStart(null);
            }
            catch (Exception e)
            {
                Logger.Log.WriteError("{0}\r\n{1}\r\n{2}", e.Message, e.InnerException?.Message, e.InnerException?.StackTrace);
            }


            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.ReadKey();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _myObj.OnStop();
        }
    }
}
