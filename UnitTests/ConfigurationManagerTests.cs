using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Data;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ConfigManager.SetXmlConfigPath(Path.Combine(Environment.CurrentDirectory, "config.xml"));
        }
        [Test]
        public void ShouldAllwaysReturnSameObject()
        {
            // arrange

            // act
            Configuration config1 = ConfigManager.Configuration;
            Configuration config2 = ConfigManager.Configuration;


            // assert
            Assert.NotNull(config1);
            Assert.NotNull(config2);
            Assert.AreSame(config1, config2);
        }

        [Test]
        public void ShouldReadXml()
        {
            // arrange
            Configuration configuration;

            // act
            configuration = ConfigManager.Configuration;

            // assert
            Assert.AreEqual(1, configuration.TestConfig.WaitTimeSeconds);
            Assert.AreEqual(60, configuration.TestConfig.TimeOutSeconds);
            CollectionAssert.IsNotEmpty(configuration.IpToTest);
            Assert.AreEqual("noreply@airpak-latam.com", configuration.MailConfiguration.SendFrom);
            CollectionAssert.IsNotEmpty(configuration.MailConfiguration.Recipients);
            Assert.AreEqual("Se ha {status} la conexion con {hostname}", configuration.MailConfiguration.Subject);
            Assert.AreEqual("\n      El equipo {computername} ha perdido la coneccion con el equipo {hostname} con {ip} en el puerto {port} durante mas de {timeout} segundos.\n\n      Favor de verificar el estado de la conexión.\n\n      {testresults}\n    ", configuration.MailConfiguration.Body);
            Assert.AreEqual(false, configuration.MailConfiguration.IsHtml);
            Assert.AreEqual("olga", configuration.MailConfiguration.SmtpCredentials.UserName);
            Assert.AreEqual("algo", configuration.MailConfiguration.SmtpCredentials.Password);
            Assert.AreEqual("smtp.gmail.com", configuration.MailConfiguration.SmtpConfiguration.Host);
            Assert.AreEqual(465, configuration.MailConfiguration.SmtpConfiguration.Port);
            Assert.AreEqual(true, configuration.MailConfiguration.SmtpConfiguration.UseSsl);
        }

        [Test]
        public void ShouldValidateConfiguration()
        {
            // arrange
            var configuration = new Configuration();
            configuration.TestConfig = new TestConfig
            {
                TimeOutSeconds = 120000,
                WaitTimeSeconds = 2000
            };
            configuration.IpToTest = new List<IP>()
            {
                new IP() {Address = "172.28.129.100", Name = "test",Port = 8080},
                new IP() {Address = "192.168.0.101",Name = "Soma"}
            };
            configuration.MailConfiguration = new MailConfiguration()
            {
                SendFrom = "final20@gmail.com",
                Recipients = new List<string>() { "rene.cruz@airpak-latam.com", "gerardo.mendez@airpak-latam.com", "rene.zamorano@airpak-latam.com" },
                Subject = "El subject del mail",
                Body = "el cuerpo del mail bla bla bla",
                IsHtml = true,
                SmtpConfiguration = new SmtpConfiguration("smtp.gmail.com", 465, true),
                SmtpCredentials = new SmtpCredential("algo", "olga")
            };
            ValidationHelper validator = new ValidationHelper();

            //act
            ObjectValidationResults result = validator.TryValidate(configuration);

            // assert
            CollectionAssert.IsEmpty(result.Results);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ShouldReturnInvalidAndCollectionErrorsOnBadConfiguration()
        {
            // arrange
            var configuration = new Configuration();
            configuration.TestConfig = new TestConfig
            {
                TimeOutSeconds = 120000,
                WaitTimeSeconds = 2000
            };
            configuration.IpToTest = new List<IP>()
            {
                new IP() {Address = "172.28.129.100", Name = "test",Port = 5656323},
                new IP() {Address = "1",Name = "Soma"}
            };
            configuration.MailConfiguration = new MailConfiguration()
            {
                SendFrom = "final20@gmail.com",
                Recipients = new List<string>() { "rene.cruz@airpak-latam.com", "gerardo.mendez@airpak-latam.com", "rene.zamorano@airpak-latam.com" },
                Subject = "asdffffffffffffffffffffffffffffa3dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
                IsHtml = true,
                SmtpConfiguration = new SmtpConfiguration("smtp.gmail.com", -1, true),
                SmtpCredentials = new SmtpCredential("algo", "olga")
            };
            ValidationHelper validator = new ValidationHelper();

            //act
            ObjectValidationResults result = validator.TryValidate(configuration);

            // assert
            CollectionAssert.IsNotEmpty(result.Results);
            Assert.IsFalse(result.IsValid);

            Debug.Print(result.ToString());
        }
    }
}