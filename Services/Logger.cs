using System;
using System.IO;
using System.Text;

namespace Services
{
    public class Logger
    {
        private static Logger _me;
        private FileWrapper _fileWrapper = new FileWrapper();
        private string _logPath = Path.Combine(Environment.CurrentDirectory, "log.txt");
        private string _template = "{0} {1}: {2}.";
        private string _timeFormatTemplate = "yy/MM/yyyy HH:mm:ss";

        private Logger() { }

        public FileWrapper FileWrapper
        {
            get { return _fileWrapper; }
            set { _fileWrapper = value; }
        }

        public string LogPath { get { return _logPath; } set { _logPath = value; } }
        public string Template { get { return _template; } set { _template = value; } }

        public static Logger Log
        {
            get { return GetLogger(); }
        }

        public string TimeFormatTemplate
        {
            get { return _timeFormatTemplate; }
            set { _timeFormatTemplate = value; }
        }

        public static Logger GetLogger()
        {
            if (_me == null)
            {
                _me = new Logger();
                return _me;
            }
            return _me;
        }

        public void Write(LogType type, string message)
        {
            string logMessage = string.Format(Template, DateTime.Now.ToString(TimeFormatTemplate), type.ToString().ToUpper(), message);
            _fileWrapper.AppendAllText(LogPath, logMessage, Encoding.UTF8);
        }
    }
}