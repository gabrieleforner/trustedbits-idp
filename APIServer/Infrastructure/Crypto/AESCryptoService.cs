using System.Security.Cryptography;
using Trustedbits.APIServer.Domain.Errors;
using Trustedbits.APIServer.Domain.Errors.Crypto;
using Trustedbits.APIServer.Domain.Models;
using Trustedbits.APIServer.Domain.Ports;

namespace Trustedbits.APIServer.Infrastructure.Crypto;

public partial class AESCryptoService : ICryptoService<SymmetricKey>
{
    // NIST-compliant AES-GCM parameters
    private const int AESKeyByteSize = 32;
    private const int AESNonceByteSize = 12;
    private const int AESTagByteSize = 16;

    private const string AppsettingsKeysSectionName = "Crypto";
    private const string AppsettingsAESKeyName = "AESRootKey";
    
    private readonly byte[] _rootAESKey;

    public AESCryptoService(IConfiguration configuration)
    {
        // Load AES root key
        var aesKeyStringBase64 = configuration[$"{AppsettingsKeysSectionName}:{AppsettingsAESKeyName}"] ??
                                 throw new ArgumentException("AES root key not found.");
        _rootAESKey = Convert.FromBase64String(aesKeyStringBase64);
    }

    public async Task<SymmetricKey> EncryptAsync(SymmetricKey plainSecret, CancellationToken cancellation = default)
    {
        // Wrap the decryption as an asynchronous operation on a secondary thread
        return await Task.Run(() =>
        {
            // Generate encryption keys for the two-level encryption
            byte[] DEK = RandomNumberGenerator.GetBytes(AESKeyByteSize);
            byte[] KEK = RandomNumberGenerator.GetBytes(AESKeyByteSize);

            return new SymmetricKey
            {
                SecretId = plainSecret.SecretId,
                SecretName = plainSecret.SecretName,
                
                Key = EncryptAES(plainSecret.Key, DEK),
                SecretDEK = EncryptAES(DEK, KEK),
                SecretKEK = EncryptAES(KEK, _rootAESKey),
            };
        }, cancellation);
    }

    public async Task<byte[]> DecryptAsync(SymmetricKey cipherSecret, CancellationToken cancellation = default)
    {
        // Wrap the decryption as an asynchronous operation on a secondary thread
        return await Task.Run(() =>
        {
            // Decrypt the two levels of encryption
            byte[] KEK = DecryptAES(cipherSecret.SecretKEK, _rootAESKey);
            byte[] DEK = DecryptAES(cipherSecret.SecretDEK, KEK);
            
            // Return the plain bytes, KEK/DEK never leave this class
            return DecryptAES(cipherSecret.Key, DEK);
        }, cancellation);
    }
}



/// <summary>
/// Partial AESCryptoService implementation containing the AES helper methods
/// </summary>
public partial class AESCryptoService
{
    // Helper Methods
    
    /// <summary>
    /// Helper method for encrypt a byte sequence using the AES-GCM
    /// (Galois Counter Mode) algorithm.
    /// </summary>
    /// <param name="plainText">Plain byte sequence to encrypt</param>
    /// <param name="key">Encryption key. If null it will be generated</param>
    /// <returns></returns>
    /// <exception cref="CryptoArgumentException">Thrown in case of bad arguments provided</exception>
    public static byte[] EncryptAES(byte[] plainText, byte[] key)
    {
        // Verify that a key has been provided
        if(key.Length == 0)
            throw new CryptoArgumentException($"An encryption key must be specified.");
        // Validate AES encryption key
        if (key.Length < AESKeyByteSize) 
            throw new CryptoArgumentException($"Invalid key length. Key must be at least {AESKeyByteSize} bytes.");
        
        // Generate random nonce and set up tag
        byte[] nonce = RandomNumberGenerator.GetBytes(AESNonceByteSize);
        byte[] tag = new byte[AESTagByteSize];
        
        byte[] cipherText = new byte[plainText.Length];

        // Try to encrypt using AES-GCM algorithm
        try
        {
            using (var aesAlgorithm = new AesGcm(key, AESTagByteSize))
            {
                aesAlgorithm.Encrypt(nonce, plainText, cipherText, tag);
            }
        }
        catch (ArgumentException ex)
        {
            // Cryptographic error management is delegated to consumer
            throw new Exception("Invalid implementation parameters", ex);
        }
        
        // Set up the "final" buffer for the encrypted bytes
        int finalCipherBufferSize = plainText.Length +  AESTagByteSize + AESNonceByteSize;
        byte[] finalBuffer = new byte[finalCipherBufferSize];
        
        // Build the final buffer using the following schema
        // nonce | tag | ciphertext
        Buffer.BlockCopy(nonce, 0, finalBuffer, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, finalBuffer, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText.ToArray(), 0, finalBuffer, nonce.Length+tag.Length, cipherText.Length);
        
        return finalBuffer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chiperText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static byte[] DecryptAES(byte[] chiperText, byte[] key)
    {
        // Verify that a key has been provided
        if(key.Length == 0)
            throw new CryptoArgumentException($"An encryption key must be specified.");
        // Validate AES encryption key
        if (key.Length < AESKeyByteSize) 
            throw new CryptoArgumentException($"Invalid key length. Key must be at least {AESKeyByteSize} bytes.");
        
        // Verify that a ciphertext has been provided
        if(chiperText.Length == 0)
            throw new CryptoArgumentException($"An encrypted message must be provided.");
        // Validate ciphertext length
        if (chiperText.Length < AESKeyByteSize) 
            throw new CryptoArgumentException($"Invalid ciphertext length. Ciphertext must be at least {AESNonceByteSize+AESTagByteSize+1} bytes.");

        byte[] nonce = chiperText[..AESNonceByteSize];
        byte[] tag = chiperText[AESNonceByteSize..(AESNonceByteSize + AESTagByteSize)];
        byte[] cipherText = chiperText[(AESNonceByteSize + AESTagByteSize)..];

        byte[] plainText = new byte[cipherText.Length];
        
        try
        {
            using (var aesAlgorithm = new AesGcm(key, AESTagByteSize))
            {
                aesAlgorithm.Decrypt(nonce, cipherText, tag, plainText);
            }
        }
        catch (ArgumentException ex)
        {
            // Cryptographic error management is delegated to consumer
            throw new Exception("Invalid implementation parameters", ex);
        }
        catch (CryptographicException ex)
        {
            throw new CryptoAuthenticationException("Authentication failed", ex);
        }
        
        return plainText;
    }
}