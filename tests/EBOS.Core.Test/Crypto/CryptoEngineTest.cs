using EBOS.Core.Crypto;

namespace EBOS.Core.Test.Crypto;

public class CryptoEngineTests
{
    private readonly CryptoEngine _crypto = new();

    #region Base64
    [Fact]
    public void EncryptToBase64_And_DecryptFromBase64_Roundtrip()
    {
        const string original = "Test text with n and EUR";
        string base64 = _crypto.EncryptToBase64(original);
        string result = _crypto.DecryptFromBase64(base64);

        Assert.Equal(original, result);
    }

    [Fact]
    public void EncryptToBase64_Throws_On_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.EncryptToBase64(null!));
    }

    [Fact]
    public void DecryptFromBase64_Throws_On_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromBase64(null!));
    }

    [Fact]
    public void DecryptFromBase64_Throws_On_Invalid_Base64()
    {
        Assert.Throws<FormatException>(() => _crypto.DecryptFromBase64("!!!not-base64!!!"));
    }
    #endregion

    #region AES - GetSaltAndCiphertext / IV
    [Fact]
    public void GetSaltAndCiphertext_FromBytes_Splits_Correctly()
    {
        byte[] salt = [1, 2, 3, 4, 5];
        byte[] cipher = [10, 20, 30, 40];
        byte[] combined = [.. salt, .. cipher];
        var result = _crypto.GetSaltAndCiphertext(combined, salt.Length);

        Assert.Equal(salt, result.Item1);
        Assert.Equal(cipher, result.Item2);
    }

    [Fact]
    public void GetSaltAndCiphertext_FromBase64_Splits_Correctly()
    {
        byte[] salt = [9, 8, 7];
        byte[] cipher = [11, 22, 33, 44];
        byte[] combined = [.. salt, .. cipher];
        string base64 = Convert.ToBase64String(combined);
        var result = _crypto.GetSaltAndCiphertext(base64, salt.Length);

        Assert.Equal(salt, result.Item1);
        Assert.Equal(cipher, result.Item2);
    }

    [Fact]
    public void GetSaltAndCiphertext_Throws_On_Null_Bytes()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.GetSaltAndCiphertext((byte[])null!, 1));
    }

    [Fact]
    public void GetSaltAndCiphertext_Throws_On_Null_Base64()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.GetSaltAndCiphertext((string)null!, 1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)] // greater than the buffer length
    public void GetSaltAndCiphertext_Throws_On_Invalid_Salt_Length(int saltLength)
    {
        byte[] cipher = [1, 2, 3, 4, 5];

        Assert.Throws<ArgumentOutOfRangeException>(() => _crypto.GetSaltAndCiphertext(cipher, saltLength));
    }

    [Fact]
    public void GenerateIV_Returns_Correct_Length_And_Is_Not_All_Zero()
    {
        const int size = 16;
        byte[] iv = _crypto.GenerateIV(size);

        Assert.NotNull(iv);
        Assert.Equal(size, iv.Length);
        Assert.Contains(iv, b => b != 0);
    }

    [Fact]
    public void GenerateIV_Throws_On_Invalid_Size()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _crypto.GenerateIV(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => _crypto.GenerateIV(-1));
    }
    #endregion

    #region AES - Encrypt / Decrypt (bytes)
    [Fact]
    public void EncryptToAES_And_DecryptFromAES_Roundtrip()
    {
        const string key = "1234567890ABCDEF"; // 16 bytes
        const string plainText = "Texto secreto de prueba";
        byte[] iv = new byte[16]; // buffer filled inside EncryptToAES
        byte[] cipher = _crypto.EncryptToAES(plainText, key, iv);
        string result = _crypto.DecryptFromAES(cipher, key, iv);

        Assert.Equal(plainText, result);
    }

    [Fact]
    public void EncryptToAES_Sets_IV_Buffer()
    {
        const string key = "1234567890ABCDEF";
        const string plainText = "Otro texto";
        byte[] iv = new byte[16];
        byte[] cipher = _crypto.EncryptToAES(plainText, key, iv);

        Assert.NotNull(cipher);
        Assert.Equal(16, iv.Length);
        Assert.Contains(iv, b => b != 0);
    }

    [Fact]
    public void EncryptToAES_Throws_On_Null_Or_Empty()
    {
        byte[] iv = new byte[16];
        const string key = "1234567890ABCDEF";

        Assert.Throws<ArgumentNullException>(() => _crypto.EncryptToAES(null!, key, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.EncryptToAES("texto", null!, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.EncryptToAES("texto", string.Empty, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.EncryptToAES("texto", key, null!));
    }

    [Fact]
    public void EncryptToAES_Throws_On_Invalid_IV_Length()
    {
        const string key = "1234567890ABCDEF";
        const string plainText = "Texto";
        byte[] wrongIv = new byte[8]; // invalid length

        Assert.Throws<ArgumentException>(() => _crypto.EncryptToAES(plainText, key, wrongIv));
    }

    [Fact]
    public void DecryptFromAES_Throws_On_Invalid_Arguments()
    {
        const string key = "1234567890ABCDEF";
        byte[] iv = new byte[16];

        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAES(null!, key, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAES([], key, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAES(new byte[1], null!, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAES(new byte[1], string.Empty, iv));
        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAES(new byte[1], key, null!));
        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAES(new byte[1], key, []));
    }
    #endregion

    #region AES - Encrypt / Decrypt (Base64)
    [Fact]
    public void EncryptToAESBase64_And_DecryptFromAESBase64_Roundtrip()
    {
        const string key = "1234567890ABCDEF";
        const string plainText = "Texto secreto con Base64";
        byte[] iv = new byte[16];
        string cipherBase64 = _crypto.EncryptToAESBase64(plainText, key, iv);
        string result = _crypto.DecryptFromAESBase64(cipherBase64, key, iv);

        Assert.Equal(plainText, result);
    }

    [Fact]
    public void EncryptToAESBase64_Throws_On_Null()
    {
        byte[] iv = new byte[16];

        Assert.Throws<ArgumentNullException>(() => _crypto.EncryptToAESBase64(null!, "1234567890ABCDEF", iv));
    }

    [Fact]
    public void DecryptFromAESBase64_Throws_On_Null_Cipher()
    {
        byte[] iv = new byte[16];

        Assert.Throws<ArgumentNullException>(() => _crypto.DecryptFromAESBase64(null!, "1234567890ABCDEF", iv));
    }

    [Fact]
    public void DecryptFromAESBase64_Throws_On_Invalid_Base64()
    {
        byte[] iv = new byte[16];

        Assert.Throws<FormatException>(() => _crypto.DecryptFromAESBase64("**not-base64**", "1234567890ABCDEF", iv));
    }
    #endregion

    #region SHA / SHA3
    [Theory]
    [InlineData("test")]
    [InlineData("")]
    [InlineData("Text with accents aeio u")]
    public void GenerateSHA256_Returns_NonEmpty_Hex(string input)
    {
        string hash = _crypto.GenerateSHA256(input);

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.True(hash.All(c => Uri.IsHexDigit(c)));
    }

    [Fact]
    public void GenerateSHA256_Throws_On_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.GenerateSHA256(null!));
    }

    [Fact]
    public void GenerateSHA384_Throws_On_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.GenerateSHA384(null!));
    }

    [Fact]
    public void GenerateSHA512_Throws_On_Null()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.GenerateSHA512(null!));
    }

    [Fact]
    public void VerifySHA256_Returns_True_For_Correct_Hash()
    {
        const string input = "password";
        string hash = _crypto.GenerateSHA256(input);
        bool result = _crypto.VerifySHA256(input, hash);

        Assert.True(result);
    }

    [Fact]
    public void VerifySHA256_Returns_False_For_Wrong_Hash()
    {
        const string input = "password";
        string hash = _crypto.GenerateSHA256(input);
        bool result = _crypto.VerifySHA256("otra-cosa", hash);

        Assert.False(result);
    }

    [Fact]
    public void VerifySHA256_Throws_On_Null_ExpectedHash()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.VerifySHA256("test", null!));
    }

    [Fact]
    public void VerifySHA384_Works_As_Expected()
    {
        const string input = "test384";
        string hash = _crypto.GenerateSHA384(input);

        Assert.True(_crypto.VerifySHA384(input, hash));
        Assert.False(_crypto.VerifySHA384(input + "x", hash));
    }

    [Fact]
    public void VerifySHA512_Works_As_Expected()
    {
        const string input = "test512";
        string hash = _crypto.GenerateSHA512(input);

        Assert.True(_crypto.VerifySHA512(input, hash));
        Assert.False(_crypto.VerifySHA512(input + "x", hash));
    }

    [Fact]
    public void VerifySHA384_Throws_On_Null_ExpectedHash()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.VerifySHA384("test", null!));
    }

    [Fact]
    public void VerifySHA512_Throws_On_Null_ExpectedHash()
    {
        Assert.Throws<ArgumentNullException>(() => _crypto.VerifySHA512("test", null!));
    }
    #endregion

    #region Token / HMAC
    [Fact]
    public void GenerateUniqueKey_Returns_Correct_Length_And_AllowedChars()
    {
        const int size = 24;
        string token = _crypto.GenerateUniqueKey(size);

        Assert.NotNull(token);
        Assert.Equal(size, token.Length);

        const string allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&|_-?";
        Assert.All(token, c => Assert.Contains(c, allowed));
    }

    [Fact]
    public void GenerateUniqueKey_Throws_On_Invalid_Size()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _crypto.GenerateUniqueKey(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => _crypto.GenerateUniqueKey(-5));
    }

    [Fact]
    public void CreateTokenHMACSHA256_Is_Deterministic()
    {
        const string plainText = "payload";
        const string secret = "super-secret";
        string token1 = _crypto.CreateTokenHMACSHA256(plainText, secret);
        string token2 = _crypto.CreateTokenHMACSHA256(plainText, secret);

        Assert.Equal(token1, token2);
    }

    [Fact]
    public void CreateTokenHMACSHA256_Changes_With_Different_Payload_Or_Secret()
    {
        const string plainText = "payload";
        const string secret = "super-secret";
        string token1 = _crypto.CreateTokenHMACSHA256(plainText, secret);
        string token2 = _crypto.CreateTokenHMACSHA256(plainText + "x", secret);
        string token3 = _crypto.CreateTokenHMACSHA256(plainText, secret + "x");

        Assert.NotEqual(token1, token2);
        Assert.NotEqual(token1, token3);
    }

    [Fact]
    public void CreateTokenHMACSHA256_Allows_Null_Secret()
    {
        const string plainText = "payload";
        string token1 = _crypto.CreateTokenHMACSHA256(plainText, null!);
        string token2 = _crypto.CreateTokenHMACSHA256(plainText, string.Empty);

        Assert.Equal(token2, token1);
    }

    [Fact]
    public void VerifyTokenHMACSHA256_Returns_True_For_Correct_Token()
    {
        const string plainText = "payload";
        const string secret = "super-secret";
        string token = _crypto.CreateTokenHMACSHA256(plainText, secret);
        bool result = _crypto.VerifyTokenHMACSHA256(plainText, secret, token);

        Assert.True(result);
    }

    [Fact]
    public void VerifyTokenHMACSHA256_Returns_False_For_Wrong_Token()
    {
        const string plainText = "payload";
        const string secret = "super-secret";
        string token = _crypto.CreateTokenHMACSHA256(plainText, secret);
        bool result = _crypto.VerifyTokenHMACSHA256(plainText, secret, token + "x");

        Assert.False(result);
    }

    [Fact]
    public void VerifyTokenHMACSHA256_Throws_On_Null_Token()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _crypto.VerifyTokenHMACSHA256("payload", "secret", null!));
    }
    #endregion
}
