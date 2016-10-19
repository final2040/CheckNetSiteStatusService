using System;
using System.Security;
using System.Text;
using Data;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class AesEncrypterTests
    {
        [Test]
        public void ShouldGetHashOfKey()
        {
            // arrange
            ICryptoKey encrypter = new Md5Key("La Llaveee", 10);
            var expected = "U68P5NLuYd1ZBSU7TsH19A==";
            
            // act
            byte[] result = encrypter.GetKey();

            // assert
            Assert.AreEqual(expected, Convert.ToBase64String(result));
        }

        [Test]
        public void ShouldReturnBytesEncriptedText()
        {
            // arrange
            var key = new Md5Key("una llave", 20);
            AesEncryptor encryptor = new AesEncryptor(key);

            // act
            byte[] result = encryptor.Encrypt(Encoding.UTF8.GetBytes("Texto a encriptar"));

            // assert
            CollectionAssert.IsNotEmpty(result);
            Assert.IsTrue(result.Length > 5);
        }

        [Test]
        public void ShouldReturnUnencryptedText()
        {
            // arrange
            var key = new Md5Key("una llave", 20);
            var encryptedData = Convert.FromBase64String("Au8XtzbP1KHq7SMedhcQMYZRcUesYLgPBLxTpxHvg3nB4yywMRFNyrZFZK3AHp6q");
            var expected = "Texto a encriptar";
            AesEncryptor encryptor = new AesEncryptor(key);
           
            // act
            byte[] result = encryptor.Decrypt(encryptedData);

            // assert
            Assert.AreEqual(expected, Encoding.UTF8.GetString(result));
        }


    }
}