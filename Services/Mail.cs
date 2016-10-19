using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

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

        [Obsolete("For testing purposes only don't use possible null")]
        public MailMessage Message
        {
            get { return _message; }
        }

        public string From { get; set; }

        public void Send()
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

            _smtpClient.Host = SmtpConfiguration.Server;
            _smtpClient.Port = SmtpConfiguration.Port;
            _smtpClient.EnableSsl = SmtpConfiguration.UseSsl;
            _smtpClient.Credentials = SmtpCredentials;

            _smtpClient.Send(_message);
        }
    }

    public class SmtpConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
    }
}