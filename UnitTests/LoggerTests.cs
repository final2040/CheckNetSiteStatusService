using System;
using System.Text;
using Data;
using Moq;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    class LoggerTests
    {
        [Test]
        public void ShouldWriteInformationToTheLog()
        {
            // arrange
            Mock<FileLogWriter> mockFileHelper = new Mock<FileLogWriter>();
            string message = string.Format("{0} {1}: {2}." + Environment.NewLine, DateTime.Now.ToString("yy/MM/yyyy HH:mm"), "INFORMATION", "Se ha enviado un mensaje al log");

            Logger.Log.LogWriter = mockFileHelper.Object;
            Logger.Log.TimeFormatTemplate = "yy/MM/yyyy HH:mm";
            mockFileHelper.Setup(m => m.Write(It.IsAny<string>(),message,Encoding.UTF8)).Verifiable();

            // act
            Logger.Log.Write(LogType.Information, "Se ha enviado un mensaje al log");

            // assert
            mockFileHelper.Verify();
        }

        [Test]
        public void ShouldWriteWarningToTheLog()
        {
            // arrange
            Mock<FileLogWriter> mockFileHelper = new Mock<FileLogWriter>();
            string message = string.Format("{0} {1}: {2}." + Environment.NewLine, DateTime.Now.ToString("yy/MM/yyyy HH:mm"), "WARNING", "Se ha enviado una advertencia al log");

            Logger.Log.LogWriter = mockFileHelper.Object;
            Logger.Log.TimeFormatTemplate = "yy/MM/yyyy HH:mm";
            mockFileHelper.Setup(m => m.Write(It.IsAny<string>(), message, Encoding.UTF8)).Verifiable();

            // act
            Logger.Log.Write(LogType.Warning, "Se ha enviado una advertencia al log");

            // assert
            mockFileHelper.Verify();
        }

        [Test]
        public void ShouldWriteErrorToTheLog()
        {
            // arrange
            Mock<FileLogWriter> mockFileHelper = new Mock<FileLogWriter>();
            string message = string.Format("{0} {1}: {2}." + Environment.NewLine, DateTime.Now.ToString("yy/MM/yyyy HH:mm"), "ERROR", "Se ha enviado un error al log");

            Logger.Log.LogWriter = mockFileHelper.Object;
            Logger.Log.TimeFormatTemplate = "yy/MM/yyyy HH:mm";
            mockFileHelper.Setup(m => m.Write(It.IsAny<string>(), message, Encoding.UTF8)).Verifiable();

            // act
            Logger.Log.Write(LogType.Error, "Se ha enviado un error al log");

            // assert
            mockFileHelper.Verify();
        }
    }
}
