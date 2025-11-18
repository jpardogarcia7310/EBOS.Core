using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EBOS.Core.Crypto;

public class CryptoEngine
{
    #region Base64
    public virtual string EncryptToBase64(string plainText)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);

        return Convert.ToBase64String(bytes);
    }
    public virtual string DecryptFromBase64(string plainText)
    {
        byte[] bytes = Convert.FromBase64String(plainText);

        return Encoding.UTF8.GetString(bytes);
    }
    #endregion

    #region AES
    public virtual Tuple<byte[], byte[]> GetSaltAndCiphertext(string cipherBase64, int nBytesSalt)
    {
        return GetSaltAndCiphertext(Convert.FromBase64String(cipherBase64), nBytesSalt);
    }
    public virtual Tuple<byte[], byte[]> GetSaltAndCiphertext(byte[] cipher, int nBytesSalt)
    {
        byte[] salt = [.. cipher.Take(nBytesSalt)];
        byte[] cypherText = [.. cipher.Skip(nBytesSalt)];

        return Tuple.Create(salt, cypherText);
    }
    public virtual byte[] GenerateIV(int nBytes)
    {
        byte[] iv = new byte[nBytes];

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        return iv;
    }
    public virtual byte[] EncryptToAES(string plainText, string key, byte[] IV)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));
        if (IV == null)
            throw new ArgumentNullException(nameof(IV));

        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.ASCII.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            int ivSize = aes.BlockSize / 8;
            if (IV.Length != ivSize)
                throw new ArgumentException($"IV must be {ivSize} bytes long.", nameof(IV));
            aes.GenerateIV();
            Buffer.BlockCopy(aes.IV, 0, IV, 0, ivSize);
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }

        return encrypted;
    }
    public virtual string DecryptFromAES(byte[] cipherText, string key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException(nameof(IV));

        string? plaintext = null;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.ASCII.GetBytes(key);
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            plaintext = srDecrypt.ReadToEnd();
        }

        return plaintext;
    }
    public virtual string EncryptToAESBase64(string plainText, string key, byte[] IV)
    {
        if (plainText == null)
            throw new ArgumentNullException(nameof(plainText));

        byte[] cipherBytes = EncryptToAES(plainText, key, IV);

        return Convert.ToBase64String(cipherBytes);
    }
    public virtual string DecryptFromAESBase64(string cipherBase64, string key, byte[] IV)
    {
        if (cipherBase64 == null)
            throw new ArgumentNullException(nameof(cipherBase64));

        byte[] cipherBytes = Convert.FromBase64String(cipherBase64);

        return DecryptFromAES(cipherBytes, key, IV);
    }
    #endregion

    #region SHA
    public virtual string GenerateSHA256(string plainText)
    {
        if (plainText == null)
            throw new ArgumentNullException(nameof(plainText));

        using SHA3_256 sha1 = SHA3_256.Create();
        byte[] input = Encoding.ASCII.GetBytes(plainText);
        byte[] hash = sha1.ComputeHash(input);

        StringBuilder sb = new(hash.Length * 2);
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));

        return sb.ToString();
    }
    public virtual string GenerateSHA384(string plainText)
    {
        if (plainText == null)
            throw new ArgumentNullException(nameof(plainText));

        using SHA3_384 sha1 = SHA3_384.Create();
        byte[] input = Encoding.ASCII.GetBytes(plainText);
        byte[] hash = sha1.ComputeHash(input);

        StringBuilder sb = new(hash.Length * 2);
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));

        return sb.ToString();
    }
    public virtual string GenerateSHA512(string plainText)
    {
        if (plainText == null)
            throw new ArgumentNullException(nameof(plainText));

        using SHA3_512 sha1 = SHA3_512.Create();
        byte[] input = Encoding.ASCII.GetBytes(plainText);
        byte[] hash = sha1.ComputeHash(input);

        StringBuilder sb = new(hash.Length * 2);
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));

        return sb.ToString();
    }
    public virtual bool VerifySHA256(string plainText, string expectedHash)
    {
        if (expectedHash == null)
            throw new ArgumentNullException(nameof(expectedHash));

        string computed = GenerateSHA256(plainText);

        return FixedTimeEquals(computed, expectedHash);
    }
    public virtual bool VerifySHA384(string plainText, string expectedHash)
    {
        if (expectedHash == null)
            throw new ArgumentNullException(nameof(expectedHash));

        string computed = GenerateSHA384(plainText);

        return FixedTimeEquals(computed, expectedHash);
    }
    public virtual bool VerifySHA512(string plainText, string expectedHash)
    {
        if (expectedHash == null)
            throw new ArgumentNullException(nameof(expectedHash));

        string computed = GenerateSHA512(plainText);

        return FixedTimeEquals(computed, expectedHash);
    }
    #endregion

    #region Token
    public virtual string GenerateUniqueKey(int maxSize = 10)
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&|_-?";
        char[] chars = allowedChars.ToCharArray();
        int size = maxSize;
        byte[] data = new byte[size];

        RandomNumberGenerator.Fill(data);
        StringBuilder result = new(size);
        foreach (byte b in data)
            result.Append(chars[b % chars.Length]);

        return result.ToString();
    }
    public virtual string CreateTokenHMACSHA256(string plainText, string secret)
    {
        secret ??= "";
        var encoding = new ASCIIEncoding();
        byte[] keyByte = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(plainText);
        using var hmacsha256 = new HMACSHA256(keyByte);
        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

        return Convert.ToBase64String(hashmessage);
    }
    public virtual bool VerifyTokenHMACSHA256(string plainText, string secret, string tokenBase64)
    {
        if (tokenBase64 == null)
            throw new ArgumentNullException(nameof(tokenBase64));

        string computedToken = CreateTokenHMACSHA256(plainText, secret);

        return FixedTimeEquals(computedToken, tokenBase64);
    }
    #endregion

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a == null || b == null)
            return false;
        if (a.Length != b.Length)
            return false;

        int diff = 0;

        for (int i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];

        return diff == 0;
    }
}
