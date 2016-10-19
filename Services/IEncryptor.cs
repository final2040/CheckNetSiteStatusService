using System.Security;

namespace Services
{
    public interface IEncryptor
    {
        string Base64Encrypt(byte[] data);
        string Base64Encrypt(string data);
        byte[] Encrypt(string data);
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(string encryptedBase64Data);
        byte[] Decrypt(byte[] encryptedData);
        string DecrypttoText(string encryptedBase64Data);
        string DecrypttoText(byte[] encryptedData);
        SecureString DecryptToSecureString(string encryptedBase64Data);
        SecureString DecryptToSecureString(byte[] encryptedData);
    }
}