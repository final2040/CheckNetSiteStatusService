using System;
using System.IO;
using System.Reflection;
using System.Text;
using Data.Log;

namespace Services.Log
{
    /// <summary>
    /// Permite registrar la actividad de la aplicación
    /// </summary>
    public class Logger
    {
        private static Logger _me;
        private ILogWriter _logWriter = new FileLogWriter();

        private string _logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "netmonitorservice.log");
        private string _template = "{0} {1}: {2}." + Environment.NewLine;
        private string _timeFormatTemplate = "yy/MM/yyyy HH:mm:ss";

        private Logger() { }

        /// <summary>
        /// Propiedad que obtiene o establece el modulo de escritura del log
        /// por defecto FileLogWriter
        /// </summary>
        public ILogWriter LogWriter
        {
            get { return _logWriter; }
            set { _logWriter = value; }
        }

        /// <summary>
        /// propiedad que obtiene o establece la ruta donde se almacenara el log.
        /// </summary>
        public string LogPath { get { return _logPath; } set { _logPath = value; } }

        /// <summary>
        /// Propiedad que obtiene o establece el formato en el que se escribira cada entrada del log
        /// por defecto "{0} {1}: {2}." donde:
        /// 0 .- Representa la fecha y hora.
        /// 1 .- Representa el tipo de log "Error","Advertencia", "Información"
        /// 2 .- El mensaje del error
        /// </summary>
        public string Template { get { return _template; } set { _template = value; } }

        /// <summary>
        /// Propiedad que obtiene una instancia de este objeto
        /// </summary>
        public static Logger Log
        {
            get { return GetLogger(); }
        }

        /// <summary>
        /// Propiedad que obtiene y establece el formato de la fecha y hora
        /// por defecto: yy/MM/yyyy HH:mm:ss
        /// </summary>
        public string TimeFormatTemplate
        {
            get { return _timeFormatTemplate; }
            set { _timeFormatTemplate = value; }
        }

        /// <summary>
        /// Obtiene una instancia de este objeto
        /// </summary>
        /// <returns>Instancia de este objeto</returns>
        public static Logger GetLogger()
        {
            if (_me == null)
            {
                _me = new Logger();
                return _me;
            }
            return _me;
        }

        /// <summary>
        /// Escribe Una linea en el log.
        /// </summary>
        /// <param name="type">Tipo de mensaje</param>
        /// <param name="message">Mensaje</param>
        public void Write(LogType type, string message)
        {
            string logMessage = string.Format(Template, DateTime.Now.ToString(TimeFormatTemplate), type.ToString().ToUpper(), message);
            _logWriter.Write(LogPath, logMessage, Encoding.UTF8);
        }
        /// <summary>
        /// Escribe una linea en el log.
        /// </summary>
        /// <param name="type">Tipo del mensaje</param>
        /// <param name="formatedMessage">Mensaje formateado</param>
        /// <param name="args">Argumentos</param>
        public void Write(LogType type, string formatedMessage, params object[] args)
        {
            Write(type, string.Format(formatedMessage, args));
        }

        /// <summary>
        /// Escribe un error en el log
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        public void WriteError(string message)
        {
            Write(LogType.Error, message);
        }

        /// <summary>
        /// Escribe un mensaje con formato en el log
        /// </summary>
        /// <param name="format">Mensaje con formato</param>
        /// <param name="args">Argumentos </param>
        public void WriteError(string format, params object[] args)
        {
            Write(LogType.Error, format, args);
        }

        /// <summary>
        /// Escribe una Advertencia en el log.
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        public void WriteWarning(string message)
        {
            Write(LogType.Warning, message);
        }

        /// <summary>
        /// Escribe una Advertencia en el log.
        /// </summary>
        /// <param name="message">Mensaje con formato</param>
        /// <param name="args">Argumentos</param>
        public void WriteWarning(string message, params object[] args)
        {
            Write(LogType.Warning, message, args);
        }

        /// <summary>
        /// Escribe un mensaje de Información en el log.
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        public void WriteInformation(string message)
        {
            Write(LogType.Information, message);
        }
        /// <summary>
        /// Escribe un mensaje de Información en el log.
        /// </summary>
        /// <param name="format">Mensaje con formato</param>
        /// <param name="args">Argumentos</param>
        public void WriteInformation(string format, params object[] args)
        {
            Write(LogType.Information, format, args);
        }
    }
}