using System.Security;
using NUnit.Framework;
using Services;
using Services.Encription;

namespace UnitTests
{
    [TestFixture]
    public class SecureStringTests
    {
        [Test]
        public void ShouldConvertToSecureString()
        {
            // arrange
            var textToSecure = "this is my text";
            
            // act 
            var secureString = textToSecure.ConvertToSecureString();
            
            // assert
            Assert.IsInstanceOf(typeof(SecureString),secureString);
        }

        [Test]
        public void ShouldReturnSecureString()
        {
            // arrange

            var secureString = new SecureString();
            var expected = "Expected Text";

            // act 
            foreach (char c in expected)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();

            var result = secureString.ConvertToString();

            // assert
            Assert.AreEqual(expected,result);
        }

        [Test]
        public void ShouldConvertBetwenExtensions()
        {
            // arrange
            var expected = "SecureTextString";

            // act
            var result1 = expected.ConvertToSecureString();
            var result2 = result1.ConvertToString();

            // assert
            Assert.AreEqual(expected, result2);
        }
    }
}