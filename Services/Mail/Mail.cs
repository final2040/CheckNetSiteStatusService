using System;
using System.Net;
using System.Net.Mail;
using Data.Mail;

namespace Services.Mail
{
    /// <summary>
    /// Envía un correo electrónico
    /// </summary>
    public class Mail:IDisposable
    {
        private SmtpClientWraper _smtpClient = new SmtpClientWraper();
        private bool _disposed;

        public Mail(){}
        public Mail(string user, string password, string host, int port, bool useSsl):this(new NetworkCredential(user,password), new SmtpConfiguration(host,port,useSsl) ){}
        public Mail(string user, string password, SmtpConfiguration configuration):this(new NetworkCredential(user,password), configuration) { }
        public Mail(ICredentialsByHost credentials, SmtpConfiguration configuration)
        {
            SmtpCredentials = credentials;
            SmtpConfiguration = configuration;
        }
        /// <summary>
        /// Propiedad que obtiene o establece el cliente que envíara el correo
        /// </summary>
        public SmtpClientWraper SmtpClient
        {
            get { return _smtpClient; }
            set { _smtpClient = value; }
        }

        /// <summary>
        /// Propiedad que obtiene o establece La configuración del cliente de correo
        /// </summary>
        public SmtpConfiguration SmtpConfiguration { get; set; }

        /// <summary>
        /// Propiedad que obtiene o establece las credenciales que utilizara el cliente SMTP
        /// </summary>
        public ICredentialsByHost SmtpCredentials { get; set; }

        /// <summary>
        /// Envía el mensaje de correo electrónico especificado
        /// </summary>
        /// <param name="message"></param>
        public void Send(MailMessage message)
        {
            SetSmtpConfiguration(_smtpClient);
            _smtpClient.Send(message);
        }

        /// <summary>
        /// Calida las configuraciónes del correo
        /// </summary>
        private void Validate()
        {
            if (SmtpConfiguration == null)
                throw new InvalidOperationException("Debe de proporcionar una configuración para el envío del correo.");
            if (SmtpCredentials == null)
                throw new InvalidOperationException("Debe de proporcionar credenciales para el envío del correo.");
        }

        /// <summary>
        /// Establece la configuración del cliente SMTP
        /// </summary>
        /// <param name="smtpClient">Cliente SMTP a configurar</param>
        private void SetSmtpConfiguration(SmtpClientWraper smtpClient)
        {
            smtpClient.Host = SmtpConfiguration.Host;
            smtpClient.Port = SmtpConfiguration.Port;
            smtpClient.EnableSsl = SmtpConfiguration.UseSsl;
            smtpClient.Credentials = SmtpCredentials;
        }

        /// <summary>
        /// Destruye la instancia de este objeto
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destruye la instancia de este objeto.
        /// </summary>
        /// <param name="disposing"></param>
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