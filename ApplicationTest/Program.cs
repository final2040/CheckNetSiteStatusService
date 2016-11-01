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
                _logger.WriteError(e.Message);
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
