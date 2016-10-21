using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Xml.Schema;
using Data;

namespace Services
{
    public class Mail
    {
        private SmtpClientWraper _smtpClient = new SmtpClientWraper();
        private MailMessage _message;

        public SmtpClientWraper SmtpClient
        {
            get { return _smtpClient; }
            set { _smtpClient = value; }
        }

        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsMessageHtml { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public NetworkCredential SmtpCredentials { get; set; }
        
        public MailMessage Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string From { get; set; }

        public void Send()
        {
            Validate();
            SetSmtpConfiguration(_smtpClient);
            if (Message == null)
            {
                Message = GetMessage();
            }
            _smtpClient.Send(Message);
        }

        public void Send(MailMessage message)
        {
            _smtpClient.Send(Message);
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(Subject))
                throw new InvalidOperationException("El Asunto del correo no puede ir en blanco.");
            if (string.IsNullOrEmpty(Body))
                throw new InvalidOperationException("El Cuerpo del correo no puede ir en blanco.");
            if (SmtpConfiguration == null)
                throw new InvalidOperationException("Debe de proporcionar una configuración para el envío del correo.");
            if (SmtpCredentials == null)
                throw new InvalidOperationException("Debe de proporcionar credenciales para el envío del correo.");
        }

        private void SetSmtpConfiguration(SmtpClientWraper smtpClient)
        {
            smtpClient.Host = SmtpConfiguration.Host;
            smtpClient.Port = SmtpConfiguration.Port;
            smtpClient.EnableSsl = SmtpConfiguration.UseSsl;
            smtpClient.Credentials = SmtpCredentials;
        }

        private MailMessage GetMessage()
        {
            _message = new MailMessage();
            foreach (var recipient in Recipients)
            {
                _message.To.Add(new MailAddress(recipient));
            }
            _message.Subject = Subject;
            _message.Body = Body;
            _message.IsBodyHtml = IsMessageHtml;
            _message.From = new MailAddress(From);
            return _message;
        }
    }
}