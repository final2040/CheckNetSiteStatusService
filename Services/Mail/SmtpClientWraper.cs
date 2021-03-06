﻿using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;

namespace Services.Mail
{
    /// <summary>
    /// Envuelve a la clase SmtpClient para mas información ver directamente 
    /// la documentación oficial.
    /// </summary>
    public class SmtpClientWraper : IDisposable
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

        public virtual X509CertificateCollection Type
        {
            get { return _smtpClient.ClientCertificates; }
        }

        public virtual ICredentialsByHost Credentials
        {
            get { return _smtpClient.Credentials; }
            set { _smtpClient.Credentials = value; }
        }

        public virtual SmtpDeliveryFormat DeliveryFormat
        {
            get { return _smtpClient.DeliveryFormat; }
            set { _smtpClient.DeliveryFormat = value; }
        }

        public virtual SmtpDeliveryMethod DeliveryMethod
        {
            get { return _smtpClient.DeliveryMethod; }
            set { _smtpClient.DeliveryMethod = value; }
        }

        public virtual bool EnableSsl
        {
            get { return _smtpClient.EnableSsl; }
            set { _smtpClient.EnableSsl = value; }
        }

        public virtual string Host
        {
            get { return _smtpClient.Host; }
            set { _smtpClient.Host = value; }
        }

        public virtual string PickupDirectoryLocation
        {
            get { return _smtpClient.PickupDirectoryLocation; }
            set { _smtpClient.PickupDirectoryLocation = value; }
        }

        public virtual int Port
        {
            get { return _smtpClient.Port; }
            set { _smtpClient.Port = value; }
        }

        public virtual ServicePoint ServicePoint
        {
            get { return _smtpClient.ServicePoint; }
        }

        public virtual string TargetName
        {
            get { return _smtpClient.TargetName; }
            set { _smtpClient.TargetName = value; }
        }

        public virtual int Timeout
        {
            get { return _smtpClient.Timeout; }
            set { _smtpClient.Timeout = value; }
        }

        public virtual bool UseDefaultCredentials
        {
            get { return _smtpClient.UseDefaultCredentials; }
            set { _smtpClient.UseDefaultCredentials = value; }
        }

        #endregion


        public virtual event SendCompletedEventHandler SendCompleted;

        public virtual void Dispose()
        {
            _smtpClient.Dispose();
        }

        public virtual void Send(MailMessage message)
        {
            _smtpClient.Send(message);
        }

        public virtual void Send(string @from, string recipients, string subject, string body)
        {
            _smtpClient.Send(@from, recipients, subject, body);
        }

        public virtual void SendAsync(MailMessage mailMessage, Object userToken)
        {
            _smtpClient.SendAsync(mailMessage,userToken);
        }

        public virtual void SendAsync(String @from, String recipients, String subject, String body, Object userToken)
        {
            _smtpClient.SendAsync(@from,recipients,subject,body,userToken);
        }

        public virtual void SendAsyncCancel()
        {
            _smtpClient.SendAsyncCancel();
        }

        public virtual void SendMailAsync(MailMessage mailMessage)
        {
            _smtpClient.SendMailAsync(mailMessage);
        }

        public virtual void SendMailAsync(String @from, String recipients, String subject, String body)
        {
            _smtpClient.SendMailAsync(@from, recipients, subject, body);
        }
    }
}