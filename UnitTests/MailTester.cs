using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class MailTester
    {
        [Test]
        public void Test()
        {
            // arrange
            Mail mail = new Mail();
            mail.SmtpClient = new SmtpClientWraper();
            mail.Recipients = new List<string>() {"gerardo.mendez@airpak-latam.com", "rene.zamorano@airpak-latam.com"};
            mail.Subject = "Mensaje del correo";
            mail.Body = "El cuerpo del correo";
            mail.IsHtmlMessage = false;
            mail.SmtpConfiguration = new SmtpConfiguration();
            mail.SmtpConfiguration.Server = "smtp.gmail.com";
            mail.SmtpConfiguration.Port = 465;
            mail.SmtpConfiguration.UseSsl = true;
            mail.SmtpCredentials = new NetworkCredential();
            mail.SmtpCredentials.UserName = "final20@gmail.com";
            mail.SmtpCredentials.SecurePassword = "rene".ConvertToSecureString();

            // act
            mail.Send();

            // assert

        }
         
    }
}