namespace Data
{
    public interface ICryptoKey
    {
        string Key { get; set; }
        int Hashes { get; set; }
        byte[] GetKey();
        byte[] GetKey(string key);
        byte[] GetKey(byte[] keyBytes);
        string GetBase64Key();
        string GetBase64Key(string key);
        string GetBase64Key(byte[] keyBytes);
    }
}