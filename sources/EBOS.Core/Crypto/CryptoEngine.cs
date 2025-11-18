using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EBOS.Core.Crypto;

public class CryptoEngine
{
    private static readonly Encoding Utf8Encoding = Encoding.UTF8;
    private static readonly Encoding AsciiEncoding = Encoding.ASCII;

    private const string TokenAllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&|_-?";

    #region Base64
    public virtual string EncryptToBase64(string plainText)
    {
        if (plainText is null)
            throw new ArgumentNullException(nameof(plainText));

        byte[] bytes = Utf8Encoding.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }
    public virtual string DecryptFromBase64(string plainText)
    {
        if (plainText is null)
            throw new ArgumentNullException(nameof(plainText));

        byte[] bytes = Convert.FromBase64String(plainText);
        return Utf8Encoding.GetString(bytes);
    }
    #endregion

    #region AES
    public virtual Tuple<byte[], byte[]> GetSaltAndCiphertext(string cipherBase64, int nBytesSalt)
    {
        if (cipherBase64 is null)
            throw new ArgumentNullException(nameof(cipherBase64));

        return GetSaltAndCiphertext(Convert.FromBase64String(cipherBase64), nBytesSalt);
    }
    public virtual Tuple<byte[], byte[]> GetSaltAndCiphertext(byte[] cipher, int nBytesSalt)
    {
        if (cipher is null)
            throw new ArgumentNullException(nameof(cipher));
        if (nBytesSalt < 0 || nBytesSalt > cipher.Length)
            throw new ArgumentOutOfRangeException(nameof(nBytesSalt));

        byte[] salt = [.. cipher.Take(nBytesSalt)];
        byte[] cipherText = [.. cipher.Skip(nBytesSalt)];

        return Tuple.Create(salt, cipherText);
    }
    public virtual byte[] GenerateIV(int nBytes)
    {
        if (nBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(nBytes));

        byte[] iv = new byte[nBytes];
        RandomNumberGenerator.Fill(iv);

        return iv;
    }
    public virtual byte[] EncryptToAES(string plainText, string key, byte[] IV)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));
        if (IV is null)
            throw new ArgumentNullException(nameof(IV));

        using Aes aes = Aes.Create();
        aes.Key = AsciiEncoding.GetBytes(key);
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

        return msEncrypt.ToArray();
    }
    public virtual string DecryptFromAES(byte[] cipherText, string key, byte[] IV)
    {
        if (cipherText is null || cipherText.Length == 0)
            throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));
        if (IV is null || IV.Length == 0)
            throw new ArgumentNullException(nameof(IV));

        using Aes aes = Aes.Create();
        aes.Key = AsciiEncoding.GetBytes(key);
        aes.IV = IV;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using MemoryStream msDecrypt = new(cipherText);
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        string plaintext = srDecrypt.ReadToEnd();

        return plaintext;
    }
    public virtual string EncryptToAESBase64(string plainText, string key, byte[] IV)
    {
        if (plainText is null)
            throw new ArgumentNullException(nameof(plainText));

        byte[] cipherBytes = EncryptToAES(plainText, key, IV);

        return Convert.ToBase64String(cipherBytes);
    }
    public virtual string DecryptFromAESBase64(string cipherBase64, string key, byte[] IV)
    {
        if (cipherBase64 is null)
            throw new ArgumentNullException(nameof(cipherBase64));

        byte[] cipherBytes = Convert.FromBase64String(cipherBase64);

        return DecryptFromAES(cipherBytes, key, IV);
    }
    #endregion

    #region SHA
    public virtual string GenerateSHA256(string plainText)
        => ComputeSha3HashHex(plainText, SHA3_256.Create);
    public virtual string GenerateSHA384(string plainText)
        => ComputeSha3HashHex(plainText, SHA3_384.Create);
    public virtual string GenerateSHA512(string plainText)
        => ComputeSha3HashHex(plainText, SHA3_512.Create);
    public virtual bool VerifySHA256(string plainText, string expectedHash)
        => VerifyHash(plainText, expectedHash, GenerateSHA256);
    public virtual bool VerifySHA384(string plainText, string expectedHash)
        => VerifyHash(plainText, expectedHash, GenerateSHA384);
    public virtual bool VerifySHA512(string plainText, string expectedHash)
        => VerifyHash(plainText, expectedHash, GenerateSHA512);
    #endregion

    #region Token
    public virtual string GenerateUniqueKey(int maxSize = 10)
    {
        if (maxSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxSize));

        char[] chars = TokenAllowedChars.ToCharArray();
        byte[] data = new byte[maxSize];

        RandomNumberGenerator.Fill(data);
        StringBuilder result = new(maxSize);
        foreach (byte b in data)
            result.Append(chars[b % chars.Length]);

        return result.ToString();
    }
    public virtual string CreateTokenHMACSHA256(string plainText, string secret)
    {
        secret ??= string.Empty;
        byte[] keyBytes = AsciiEncoding.GetBytes(secret);
        byte[] messageBytes = AsciiEncoding.GetBytes(plainText);

        using HMACSHA256 hmacsha256 = new(keyBytes);
        byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);

        return Convert.ToBase64String(hashMessage);
    }
    public virtual bool VerifyTokenHMACSHA256(string plainText, string secret, string tokenBase64)
    {
        if (tokenBase64 is null)
            throw new ArgumentNullException(nameof(tokenBase64));

        string computedToken = CreateTokenHMACSHA256(plainText, secret);

        return FixedTimeEquals(computedToken, tokenBase64);
    }
    #endregion

    #region Helpers
    private static string BytesToHex(byte[] bytes)
    {
        if (bytes is null)
            throw new ArgumentNullException(nameof(bytes));

        StringBuilder sb = new(bytes.Length * 2);

        for (int i = 0; i < bytes.Length; i++)
            sb.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));

        return sb.ToString();
    }
    private static bool FixedTimeEquals(string a, string b)
    {
        if (a is null || b is null)
            return false;
        if (a.Length != b.Length)
            return false;

        int diff = 0;

        for (int i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];

        return diff == 0;
    }
    private static string ComputeSha3HashHex(string plainText, Func<HashAlgorithm> algorithmFactory)
    {
        if (plainText is null)
            throw new ArgumentNullException(nameof(plainText));
        if (algorithmFactory is null)
            throw new ArgumentNullException(nameof(algorithmFactory));

        using HashAlgorithm hasher = algorithmFactory();
        byte[] input = AsciiEncoding.GetBytes(plainText);
        byte[] hash = hasher.ComputeHash(input);

        return BytesToHex(hash);
    }
    private static bool VerifyHash(string plainText, string expectedHash, Func<string, string> hashGenerator)
    {
        if (expectedHash is null)
            throw new ArgumentNullException(nameof(expectedHash));
        if (hashGenerator is null)
            throw new ArgumentNullException(nameof(hashGenerator));

        string computed = hashGenerator(plainText);
        return FixedTimeEquals(computed, expectedHash);
    }
    #endregion
}