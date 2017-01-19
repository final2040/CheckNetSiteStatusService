using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using Data.Configuration;
using Data.NetworkTest;
using NUnit.Framework;
using Services.Configuration;
using Services.Mail;

namespace UnitTests
{
    [TestFixture]
    public class MessageBuilderTests
    {
        private Configuration _configuration;

        [TestFixtureSetUp]
        public void Setup()
        {
            ConfigManager.SetXmlConfigPath(Path.Combine(Environment.CurrentDirectory, "config.xml"));
            _configuration = ConfigManager.Configuration;
        }

        [Test]
        public void ShouldBuildMailMessage()
        {
            // arrange
            MessageBuilder messageBuilder = new MessageBuilder(_configuration.MailConfiguration);
            List<INetTestResult> results = new List<INetTestResult>() { new PingTestResult(32, "172.28.129.100", 0, 128) };
            messageBuilder.AddParam("ip","172.28.129.100");
            messageBuilder.AddParam("hostname", "Connection1");
            messageBuilder.AddParam("port",8080);
            messageBuilder.AddParam("date", DateTime.Now.ToString("d"));
            messageBuilder.AddParam("sendfrom", _configuration.MailConfiguration.SendFrom);
            messageBuilder.AddParam("timeout", _configuration.TestConfig.TimeOutSeconds);
            messageBuilder.AddParam("computername", Environment.MachineName);
            messageBuilder.AddParam("appname", "DreamSoft Network Monitor");
            messageBuilder.AddParam("appversion", new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version);
 
            // act
            MailMessage result = messageBuilder.Build();

            // assert
            Assert.AreEqual(new MailAddress("noreply@airpak-latam.com"), result.From);
            Assert.AreEqual(3, result.To.Count);
            Assert.AreEqual("Se ha {status} la conexion con Connection1", result.Subject);
            Assert.AreEqual("\n      El equipo AIFCOMX013 ha perdido la coneccion con el equipo Connection1 con 172.28.129.100 en el puerto 8080 durante mas de 60 segundos.\n\n      Favor de verificar el estado de la conexión.\n\n      {testresults}\n    ", result.Body);
            Assert.AreEqual(false, result.IsBodyHtml);

        }

        [Test]
        public void ShouldBuildMailMessagewithCustomParams()
        {
            // arrange
            MessageBuilder messageBuilder = new MessageBuilder(_configuration.MailConfiguration);
            List<INetTestResult> results = new List<INetTestResult>() { new PingTestResult(32, "172.28.129.100", 0, 128) };
            messageBuilder.AddParam("status", "perdido");
            messageBuilder.AddParam("ip", "172.28.129.100");
            messageBuilder.AddParam("hostname", "Connection1");
            messageBuilder.AddParam("port", 8080);
            messageBuilder.AddParam("sendfrom", _configuration.MailConfiguration.SendFrom);
            messageBuilder.AddParam("timeout", _configuration.TestConfig.TimeOutSeconds);
            messageBuilder.AddParam("computername", Environment.MachineName);
            messageBuilder.AddParam("appname", "DreamSoft Network Monitor");
            messageBuilder.AddParam("appversion", new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version);
            
            // act
            MailMessage result = messageBuilder.Build();

            // assert
            Assert.AreEqual(new MailAddress("noreply@airpak-latam.com"), result.From);
            Assert.AreEqual(3, result.To.Count);
            Assert.AreEqual("Se ha perdido la conexion con Connection1", result.Subject);
            Assert.AreEqual("\n      El equipo AIFCOMX013 ha perdido la coneccion con el equipo Connection1 con 172.28.129.100 en el puerto 8080 durante mas de 60 segundos.\n\n      Favor de verificar el estado de la conexión.\n\n      {testresults}\n    ", result.Body);
            Assert.AreEqual(false, result.IsBodyHtml);
        }

    }
}