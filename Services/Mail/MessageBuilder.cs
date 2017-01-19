using System;
using System.Collections.Generic;
using System.Net.Mail;
using Data.Configuration;

namespace Services.Mail
{
    /// <summary>
    /// Construye un mensaje de correo electr�nico a partir de la configuraci�n de la aplicaci�n
    /// </summary>
    public class MessageBuilder
    {
        private readonly Dictionary<string,object>  _paramList = new Dictionary<string, object>();
        private readonly string _sendFrom;
        private readonly string _subject;
        private readonly string _body;
        private readonly List<MailAddress> _recipients = new List<MailAddress>();

        public MessageBuilder(MailConfiguration configuration):this(configuration.SendFrom,
            configuration.Subject, configuration.Body, configuration.Recipients){}

        public MessageBuilder(string sendFrom, string subject, string body, List<string> recipients)
        {
            if (string.IsNullOrWhiteSpace(sendFrom) || string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(body) || _recipients == null)
                throw new ArgumentException("No se puede inicalizar el objeto con argumentos nulos...");

            _sendFrom = sendFrom;
            _subject = subject;
            _body = body;
            recipients.ForEach(r => _recipients.Add(new MailAddress(r)));
        }

        /// <summary>
        /// Construye el mensaje de correo electr�nico
        /// </summary>
        /// <returns></returns>
        public MailMessage Build()
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(_sendFrom),
                Subject = ParseText(_subject),
                Body = ParseText(_body)
            };
            _recipients.ForEach(m => message.To.Add(m));

            return message;
        }

        /// <summary>
        /// A�ade una direcci�n de correo al mensaje de correo electr�nico
        /// </summary>
        /// <param name="mailAddress">Direcci�n de correo a a�adir</param>
        public void AddRecipient(string mailAddress)
        {
            AddRecipient(new MailAddress(mailAddress));
        }

        /// <summary>
        /// A�ade una direcci�n de correo al mensaje de correo electr�nico
        /// </summary>
        /// <param name="mailAddress">Direcci�n de correo a a�adir</param>
        public void AddRecipient(MailAddress mailAddress)
        {
            _recipients.Add(mailAddress);
        }

        /// <summary>
        /// Realiza los reemplazos en la cadena de correo electr�nico con los parametros a�adidos mediante,
        /// el m�todo AddParam
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
        /// A�ade un parametro a la lista de parametros
        /// </summary>
        /// <param name="name">Nombre del parametro a cambiar</param>
        /// <param name="value">Valor</param>
        public void AddParam(string name, object value)
        {
            _paramList.Add(name,value);
        }

        /// <summary>
        /// Remueve un par�metro a la lista de parametros
        /// </summary>
        /// <param name="name">Nombre del parametro a remover</param>
        public void RemoveParam(string name)
        {
            if (_paramList.ContainsKey(name))
            {
                _paramList.Remove(name);
            }
        }
    }
}