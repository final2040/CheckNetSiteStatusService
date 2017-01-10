using System.Security;

namespace Services.Encription
{
    /// <summary>
    /// Interfaz para los modulos de encriptación
    /// </summary>
    public interface IEncryptor
    {
        string EncryptToBase64(byte[] data);
        string EncryptToBase64(string data);
        byte[] Encrypt(string text);
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(string encryptedBase64Data);
        byte[] Decrypt(byte[] encryptedData);
        string DecryptToText(string encryptedBase64Data);
        string DecryptToText(byte[] encryptedData);
        SecureString DecryptToSecureString(string encryptedBase64Data);
        SecureString DecryptToSecureString(byte[] encryptedData);
    }
}