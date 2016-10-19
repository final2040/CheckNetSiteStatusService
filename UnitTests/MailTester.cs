using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security;
using Data;
using Moq;
using NUnit.Framework;
using Services;

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
            Mail mail = new Mail();
            mail.SmtpClient = _mockMail.Object;
            mail.From = "rene.cruz@airpak-latam.com";
            mail.Recipients = new List<string>() {"gerardo.mendez@airpak-latam.com", "rene.zamorano@airpak-latam.com"};
            mail.Subject = "Mensaje del correo";
            mail.Body = "El cuerpo del correo";
            mail.IsMessageHtml = false;
            mail.SmtpConfiguration = new SmtpConfiguration();
            mail.SmtpConfiguration.Server = "smtp.gmail.com";
            mail.SmtpConfiguration.Port = 465;
            mail.SmtpConfiguration.UseSsl = true;
            mail.SmtpCredentials = new NetworkCredential();
            mail.SmtpCredentials.UserName = "final20@gmail.com";
            mail.SmtpCredentials.SecurePassword = "rene".ConvertToSecureString();
            
            _mockMail.Setup(mm => mm.Send(It.IsAny<MailMessage>())).Verifiable();
            
            // act
            mail.Send();

            // assert
            _mockMail.Verify();
            Assert.AreEqual(2, mail.Message.To.Count);
            Assert.AreEqual("rene.cruz@airpak-latam.com",mail.Message.From.Address);
            Assert.AreEqual("Mensaje del correo", mail.Message.Subject);
            Assert.AreEqual("El cuerpo del correo", mail.Message.Body);
            Assert.AreEqual(false, mail.Message.IsBodyHtml);

            _mockMail.VerifySet(wraper => wraper.Host = "smtp.gmail.com");
            _mockMail.VerifySet(wraper => wraper.Port = 465);
            _mockMail.VerifySet(wraper => wraper.EnableSsl = true);
            _mockMail.VerifySet(wraper => wraper.Credentials = It.IsAny<NetworkCredential>());
        }
         
    }
}