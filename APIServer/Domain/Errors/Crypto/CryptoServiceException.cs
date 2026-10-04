using Trustedbits.APIServer.Domain.Models;
using Trustedbits.APIServer.Domain.Ports;

namespace Trustedbits.APIServer.Domain.Errors;

/// <summary>
/// Base class for errors raised by <see cref="ICryptoService{T}"/>
/// implementations while encrypting or decrypting <see cref="SecretBase"/>
/// secrets.
/// </summary>
/// <remarks>
/// <para>
/// Concrete <c>Infrastructure/Crypto/</c> implementations translate
/// <c>System.Security.Cryptography</c> library exceptions into derived
/// <see cref="CryptoServiceException"/> types so that callers depend only on
/// domain exceptions and never on the underlying .NET cryptography library.
/// </para>
/// <para>
/// Callers may catch <see cref="CryptoServiceException"/> to handle any
/// <c>ICryptoService</c> failure, or catch a specific derived type for
/// finer-grained handling.
/// </para>
/// </remarks>
/// <seealso cref="CryptoAuthenticationException"/>
/// <seealso cref="CryptographicArgumentException"/>
/// <seealso cref="ICryptoService{T}"/>
public class CryptoServiceException : Exception
{
    /// <summary>
    /// Name of the <see cref="SecretBase"/> involved in the failed
    /// cryptographic operation, when available; otherwise <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// Carries only the logical secret name, never the secret value or key
    /// material, so it is safe to surface in logs and error responses.
    /// </remarks>
    public string? SecretName { get; set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoServiceException"/> class.
    /// </summary>
    public CryptoServiceException()
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoServiceException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">Message that describes the error.</param>
    public CryptoServiceException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoServiceException"/> class
    /// with a specified error message and a reference to the inner exception that
    /// is the cause of this exception.
    /// </summary>
    /// <param name="message">Message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or
    /// <see langword="null"/> if no inner exception is specified.
    /// </param>
    public CryptoServiceException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}