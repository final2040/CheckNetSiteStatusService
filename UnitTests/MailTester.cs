using System.Net;
using System.Net.Mail;
using Data;
using Data.Mail;
using Moq;
using NUnit.Framework;
using Services;
using Services.Encription;
using Services.Mail;

namespace UnitTests
{
    [TestFixture]
    public class MailTester
    {
        private Mock<SmtpClientWraper> _mockMail = new Mock<SmtpClientWraper>();

        [Test]
        public void ShouldSendEmail()
        {
            // arrange
            MailMessage message = new MailMessage();
            Mail mail = new Mail();
            mail.SmtpClient = _mockMail.Object;
            message.From = new MailAddress("rene.cruz@airpak-latam.com");
            message.To.Add("gerardo.mendez@airpak-latam.com");
            message.To.Add("rene.cruz@airpak-latam.com");
            message.To.Add("rene.zamorano@airpak-latam.com");
            message.Subject = "Mensaje del correo";
            message.Body = "El cuerpo del correo";
            message.IsBodyHtml = false;
            mail.SmtpConfiguration = new SmtpConfiguration();
            mail.SmtpConfiguration.Host = "smtp.gmail.com";
            mail.SmtpConfiguration.Port = 465;
            mail.SmtpConfiguration.UseSsl = true;
            mail.SmtpCredentials = new NetworkCredential("final20@gmail.com","rene");
            
            _mockMail.Setup(mm => mm.Send(message)).Verifiable();
            
            // act
            mail.Send(message);

            // assert
            _mockMail.Verify();

            _mockMail.VerifySet(wraper => wraper.Host = "smtp.gmail.com");
            _mockMail.VerifySet(wraper => wraper.Port = 465);
            _mockMail.VerifySet(wraper => wraper.EnableSsl = true);
            _mockMail.VerifySet(wraper => wraper.Credentials = It.IsAny<NetworkCredential>());
        }
       
         
    }
}