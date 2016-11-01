using System;
using System.Text;

namespace Services
{
    public class ConsoleLogWriter : ILogWriter
    {
        public void Write(string path, string contents, Encoding encoding)
        {
            Console.Write(contents);
        }
    }
}