using System;
using System.Net.Mail;
using Data;
using NUnit.Framework;
using Services;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestFixture]
    public class MailBuilderTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ConfigManager.SetXmlConfigPath(Path.Combine(Environment.CurrentDirectory, "config.xml"));
        }

        [Test]
        public void ShouldBuildMailMessage()
        {
            // arrange
            MailBuilder mailBuilder = new MailBuilder(ConfigManager.Configuration);
            List<INetTestResult> results = new List<INetTestResult>() { new PingTestResult(32, "172.28.129.100", 0, 128) };

            // act
            MailMessage result = mailBuilder.Build("Connection1", "172.28.129.100", 8080);

            // assert
            Assert.AreEqual(new MailAddress("noreply@airpak-latam.com"), result.From);
            Assert.AreEqual(3, result.To.Count);
            Assert.AreEqual("Se ha {status} la conexion con Connection1", result.Subject);
            Assert.AreEqual("\n      El equipo AIFCOMX013 ha perdido la coneccion con el equipo Connection1 con 172.28.129.100 en el puerto 8080 durante mas de 60000 segundos.\n\n      Favor de verificar el estado de la conexión.\n\n      {testresults}\n    ", result.Body);
            Assert.AreEqual(false, result.IsBodyHtml);

        }

        [Test]
        public void ShouldBuildMailMessagewithCustomParams()
        {
            // arrange
            MailBuilder mailBuilder = new MailBuilder(ConfigManager.Configuration);
            List<INetTestResult> results = new List<INetTestResult>() { new PingTestResult(32, "172.28.129.100", 0, 128) };
            mailBuilder.AddParam("status", "perdido");
            
            // act
            MailMessage result = mailBuilder.Build("Connection1", "172.28.129.100", 8080);

            // assert
            Assert.AreEqual(new MailAddress("noreply@airpak-latam.com"), result.From);
            Assert.AreEqual(3, result.To.Count);
            Assert.AreEqual("Se ha perdido la conexion con Connection1", result.Subject);
            Assert.AreEqual("\n      El equipo AIFCOMX013 ha perdido la coneccion con el equipo Connection1 con 172.28.129.100 en el puerto 8080 durante mas de 60000 segundos.\n\n      Favor de verificar el estado de la conexión.\n\n      {testresults}\n    ", result.Body);
            Assert.AreEqual(false, result.IsBodyHtml);
        }

    }
}