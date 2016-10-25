using System;
using System.Net;
using System.Net.Mail;
using Data;

namespace Services
{
    public class Mail:IDisposable
    {
        private SmtpClientWraper _smtpClient = new SmtpClientWraper();
        private bool _disposed;

        public SmtpClientWraper SmtpClient
        {
            get { return _smtpClient; }
            set { _smtpClient = value; }
        }

        public SmtpConfiguration SmtpConfiguration { get; set; }
        public NetworkCredential SmtpCredentials { get; set; }


        public string From { get; set; }

        public void Send(MailMessage message)
        {
            SetSmtpConfiguration(_smtpClient);
            _smtpClient.Send(message);
        }

        private void Validate()
        {
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                SmtpClient.Dispose();
            }
            _disposed = true;
        }

        ~Mail()
        {
            Dispose(false);
        }
    }
}