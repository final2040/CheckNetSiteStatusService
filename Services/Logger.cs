using System;
using System.IO;
using System.Reflection;
using System.Text;
using Data;

namespace Services
{
    public class Logger
    {
        private static Logger _me;
        private ILogWriter _logWriter = new FileLogWriter();

        private string _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "netmonitorservice.log");
        private string _template = "{0} {1}: {2}." + Environment.NewLine;
        private string _timeFormatTemplate = "yy/MM/yyyy HH:mm:ss";

        private Logger() { }

        public ILogWriter LogWriter
        {
            get { return _logWriter; }
            set { _logWriter = value; }
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
            _logWriter.Write(LogPath, logMessage, Encoding.UTF8);
        }
        public void Write(LogType type, string formatedMessage, params object[] args)
        {
            Write(type, string.Format(formatedMessage, args));
        }
        public void WriteError(string message)
        {
            Write(LogType.Error, message);
        }
        public void WriteError(string format, params object[] args)
        {
            Write(LogType.Error, format, args);
        }
        public void WriteWarning(string message)
        {
            Write(LogType.Warning, message);
        }
        public void WriteWarning(string message, params object[] args)
        {
            Write(LogType.Warning, message, args);
        }
        public void WriteInformation(string message)
        {
            Write(LogType.Information, message);
        }
        public void WriteInformation(string format, params object[] args)
        {
            Write(LogType.Information, format, args);
        }
    }
}