using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;

namespace Services.Mail
{
    /// <summary>
    /// Construye un mensaje de correo electrónico a partir de la configuración de la aplicación
    /// </summary>
    public class MailBuilder
    {
        private Data.Configuration.Configuration _configuration;
        private readonly Dictionary<string,object>  _paramList = new Dictionary<string, object>();
        public MailBuilder(Data.Configuration.Configuration configuration)
        {
            _configuration = configuration;
            
        }

        /// <summary>
        /// Propiedad que obtiene la lista de parametros del correo electrónico
        /// dichos parametros serán reemplazados en la cadena con el mensaje.
        /// </summary>
        public Dictionary<string, object> Params
        {
            get { return _paramList; }
        }

        /// <summary>
        /// Construye el mensaje de correo electrónico
        /// </summary>
        /// <returns></returns>
        public MailMessage Build()
        {
            MailMessage message = new MailMessage();;
            GenerateParameters();
            message.From = new MailAddress(_configuration.MailConfiguration.SendFrom);
            AddRecipients(message);
            message.Subject = ParseText(_configuration.MailConfiguration.Subject);
            message.Body = ParseText(_configuration.MailConfiguration.Body);

            return message;
        }

        /// <summary>
        /// Añade un Destinatario al mensaje de correo.
        /// </summary>
        /// <param name="message">El mensaje al que se le añadirá el destinatario</param>
        private void AddRecipients(MailMessage message)
        {
            foreach (var recipient in _configuration.MailConfiguration.Recipients)
            {
                message.To.Add(new MailAddress(recipient));
            }
        }

        /// <summary>
        /// Realiza los reemplazos en la cadena de correo electrónico con los parametros indicados por la
        /// propiedad Params
        /// </summary>
        /// <param name="messageSubject">Titulo del mensaje</param>
        /// <returns>Cadena de texto con el mensaje completado</returns>
        private string ParseText(string messageSubject)
        {
            foreach (var param in _paramList)
            {
                string replace = "{" + param.Key + "}";
                messageSubject = messageSubject.Replace(replace, param.Value.ToString());
            }
            return messageSubject;
        }

        /// <summary>
        /// Genera los parametros por defecto
        /// </summary>
        private void GenerateParameters()
        {
            _paramList.Add("sendfrom", _configuration.MailConfiguration.SendFrom);
            _paramList.Add("timeout", _configuration.TestConfig.TimeOutSeconds);
            _paramList.Add("computername", Environment.MachineName);
            _paramList.Add("appname", "DreamSoft Network Monitor");
            _paramList.Add("appversion", new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version);
        }

        /// <summary>
        /// Añade un parametro a la lista de parametros
        /// </summary>
        /// <param name="name">Nombre del parametro a cambiar</param>
        /// <param name="value">Valor</param>
        public void AddParam(string name, object value)
        {
            _paramList.Add(name,value);
        }
    }
}