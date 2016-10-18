using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;

namespace Services
{
    public interface ISmtpClient
    {
        ICredentialsByHost Credentials { get; set; }
        SmtpDeliveryFormat DeliveryFormat { get; set; }
        SmtpDeliveryMethod DeliveryMethod { get; set; }
        bool EnableSsl { get; set; }
        string Host { get; set; }
        string PickupDirectoryLocation { get; set; }
        int Port { get; set; }
        ServicePoint ServicePoint { get; }
        string TargetName { get; set; }
        int Timeout { get; set; }
        X509CertificateCollection Type { get; }
        bool UseDefaultCredentials { get; set; }

        event SendCompletedEventHandler SendCompleted;

        void Dispose();
        void Send(MailMessage message);
        void Send(string from, string recipients, string subject, string body);
        void SendAsync(MailMessage mailMessage, object userToken);
        void SendAsync(string from, string recipients, string subject, string body, object userToken);
        void SendAsyncCancel();
        void SendMailAsync(MailMessage mailMessage);
        void SendMailAsync(string from, string recipients, string subject, string body);
    }
}