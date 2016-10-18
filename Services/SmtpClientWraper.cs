using System;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace Services
{
    public class SmtpClientWraper : IDisposable, ISmtpClient
    {
        private readonly SmtpClient _smtpClient;

        #region Constructors

        public SmtpClientWraper(SmtpClient smtpClient)
        {
            this._smtpClient = smtpClient;
            _smtpClient.SendCompleted += _smtpClient_SendCompleted;
        }

        private void _smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SendCompleted?.Invoke(this,e);
        }

        public SmtpClientWraper()
        {
            _smtpClient = new SmtpClient();
            _smtpClient.SendCompleted += _smtpClient_SendCompleted;
        }

        public SmtpClientWraper(string host)
        {
            _smtpClient = new SmtpClient(host);
            _smtpClient.SendCompleted += _smtpClient_SendCompleted;
        }

        public SmtpClientWraper(string host, int port)
        {
            _smtpClient = new SmtpClient(host, port);
            _smtpClient.SendCompleted += _smtpClient_SendCompleted;
        }

        #endregion

        #region Properties

        public X509CertificateCollection Type
        {
            get { return _smtpClient.ClientCertificates; }
        }

        public ICredentialsByHost Credentials
        {
            get { return _smtpClient.Credentials; }
            set { _smtpClient.Credentials = value; }
        }

        public SmtpDeliveryFormat DeliveryFormat
        {
            get { return _smtpClient.DeliveryFormat; }
            set { _smtpClient.DeliveryFormat = value; }
        }

        public SmtpDeliveryMethod DeliveryMethod
        {
            get { return _smtpClient.DeliveryMethod; }
            set { _smtpClient.DeliveryMethod = value; }
        }

        public bool EnableSsl
        {
            get { return _smtpClient.EnableSsl; }
            set { _smtpClient.EnableSsl = value; }
        }

        public string Host
        {
            get { return _smtpClient.Host; }
            set { _smtpClient.Host = value; }
        }

        public string PickupDirectoryLocation
        {
            get { return _smtpClient.PickupDirectoryLocation; }
            set { _smtpClient.PickupDirectoryLocation = value; }
        }

        public int Port
        {
            get { return _smtpClient.Port; }
            set { _smtpClient.Port = value; }
        }

        public ServicePoint ServicePoint
        {
            get { return _smtpClient.ServicePoint; }
        }

        public string TargetName
        {
            get { return _smtpClient.TargetName; }
            set { _smtpClient.TargetName = value; }
        }

        public int Timeout
        {
            get { return _smtpClient.Timeout; }
            set { _smtpClient.Timeout = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return _smtpClient.UseDefaultCredentials; }
            set { _smtpClient.UseDefaultCredentials = value; }
        }

        #endregion


        public event SendCompletedEventHandler SendCompleted;

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        public void Send(MailMessage message)
        {
            _smtpClient.Send(message);
        }

        public void Send(string @from, string recipients, string subject, string body)
        {
            _smtpClient.Send(@from, recipients, subject, body);
        }

        public void SendAsync(MailMessage mailMessage, Object userToken)
        {
            _smtpClient.SendAsync(mailMessage,userToken);
        }

        public void SendAsync(String @from, String recipients, String subject, String body, Object userToken)
        {
            _smtpClient.SendAsync(@from,recipients,subject,body,userToken);
        }

        public void SendAsyncCancel()
        {
            _smtpClient.SendAsyncCancel();
        }

        public void SendMailAsync(MailMessage mailMessage)
        {
            _smtpClient.SendMailAsync(mailMessage);
        }

        public void SendMailAsync(String @from, String recipients, String subject, String body)
        {
            _smtpClient.SendMailAsync(@from, recipients, subject, body);
        }
    }
}