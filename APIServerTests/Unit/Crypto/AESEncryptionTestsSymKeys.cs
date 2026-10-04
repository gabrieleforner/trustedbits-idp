using System.Text;
using Microsoft.Extensions.Configuration;
using Trustedbits.APIServer.Domain.Errors.Crypto;
using Trustedbits.APIServer.Domain.Models;
using Trustedbits.APIServer.Infrastructure.Crypto;

namespace APIServerTests.Unit.Crypto;

public class AESEncryptionTestsSymKeys
{
    private readonly AESCryptoService _cryptoService;
    private readonly byte[] _validKey;

    public AESEncryptionTestsSymKeys()
    {
        // Must be 32 bytes for AES-256
        _validKey = Encoding.UTF8.GetBytes("01234567890123456789012345678901");
        var base64Key = Convert.ToBase64String(_validKey);

        var mockConfiguration = new Dictionary<string, string?>
        {
            { "Crypto:AESRootKey", base64Key },
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(mockConfiguration)
            .Build();
        _cryptoService = new AESCryptoService(config);
    }

    [Fact]
    public async Task EncryptAndDecryptAsync_ValidSecret_EncryptsAndDecryptsSuccessfully()
    {
        // Arrange
        var plainSecret = new SymmetricKey
        {
            SecretId = Guid.NewGuid(),
            SecretName = "TestSecret",
            Key = Encoding.UTF8.GetBytes("This is a super secret message")
        };

        // Act
        var encryptedSecret = await _cryptoService.EncryptAsync(plainSecret);
        var decryptedBytes = await _cryptoService.DecryptAsync(encryptedSecret);

        // Assert
        Assert.NotNull(encryptedSecret.Key);
        Assert.NotNull(encryptedSecret.SecretDEK);
        Assert.NotNull(encryptedSecret.SecretKEK);
        Assert.NotEqual(plainSecret.Key, encryptedSecret.Key);

        var decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
        Assert.Equal("This is a super secret message", decryptedMessage);
    }

    [Fact]
    public void EncryptAES_ValidParameters_ReturnsCipherText()
    {
        // Arrange
        var plainText = Encoding.UTF8.GetBytes("Hello World");

        // Act
        var cipherText = AESCryptoService.EncryptAES(plainText, _validKey);

        // Assert
        Assert.NotNull(cipherText);
        Assert.True(cipherText.Length > plainText.Length);
    }

    [Fact]
    public void DecryptAES_ValidParameters_ReturnsPlainText()
    {
        // Arrange
        var expectedText = "Hello World";
        var plainText = Encoding.UTF8.GetBytes(expectedText);
        var cipherText = AESCryptoService.EncryptAES(plainText, _validKey);

        // Act
        var decryptedTextBytes = AESCryptoService.DecryptAES(cipherText, _validKey);
        var decryptedText = Encoding.UTF8.GetString(decryptedTextBytes);

        // Assert
        Assert.Equal(expectedText, decryptedText);
    }

    [Fact]
    public void EncryptAES_InvalidKeyLength_ThrowsCryptoArgumentException()
    {
        // Arrange
        var plainText = Encoding.UTF8.GetBytes("Hello");
        var invalidKey = new byte[16]; // Too short for AES-256

        // Act & Assert
        var exception =
            Assert.Throws<CryptoArgumentException>(() => AESCryptoService.EncryptAES(plainText, invalidKey));
        Assert.Contains("Invalid key length", exception.Message);
    }

    [Fact]
    public void DecryptAES_InvalidCiphertextLength_ThrowsCryptoArgumentException()
    {
        // Arrange
        var invalidCipher = new byte[10]; // Too short to contain nonce + tag

        // Act & Assert
        var exception =
            Assert.Throws<CryptoArgumentException>(() => AESCryptoService.DecryptAES(invalidCipher, _validKey));
        Assert.Contains("Invalid ciphertext length", exception.Message);
    }
}