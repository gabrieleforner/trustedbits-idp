using Trustedbits.APIServer.Domain.Models;

namespace Trustedbits.APIServer.Domain.Ports;

/// <summary>
/// Generic interface that declares the services for cryptographic secrets
/// </summary>
/// <typeparam name="T">Secret type, must extend <c>SecretBase</c></typeparam>
/// <seealso cref="SecretBase"/>
public interface ICryptoService<T> where T : SecretBase
{
    /// <summary>
    /// Encrypt the secret field with a generated DEK and KEK
    /// </summary>
    /// <param name="plainSecret"><c>T</c> instance with KEK/DEK zeroed</param>
    /// <param name="cancellation">Cancellation token</param>
    /// <returns><c>T</c> instance with encrypted secret, DEK, and KEK</returns>
    Task<T> EncryptAsync(T plainSecret, CancellationToken cancellation = default);
    
    /// <summary>
    /// Decrypt a secret field
    /// </summary>
    /// <param name="cipherSecret"><c>T</c> instance with KEK/DEK and secret value encrypted</param>
    /// <param name="cancellation">Cancellation token</param>
    /// <returns><c>T</c> instance with KEK/DEK zeroed and the secret(s) in plaintext</returns>
    Task<T> DecryptAsync(T cipherSecret, CancellationToken cancellation = default);
}