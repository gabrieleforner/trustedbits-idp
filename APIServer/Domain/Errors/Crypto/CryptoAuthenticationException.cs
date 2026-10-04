using Trustedbits.APIServer.Domain.Models;
using Trustedbits.APIServer.Domain.Ports;

namespace Trustedbits.APIServer.Domain.Errors;

/// <summary>
/// The exception that is thrown when a <see cref="ICryptoService{T}"/>
/// implementation fails to authenticate or decrypt a <see cref="SecretBase"/>,
/// for example because an AEAD authentication tag did not verify, padding was
/// invalid, or a key could not be imported.
/// </summary>
/// <remarks>
/// <para>
/// This is the domain translation of
/// <c>System.Security.Cryptography.CryptographicException</c>. The original
/// library exception is retained as the <see cref="Exception.InnerException"/>
/// when the <c>Infrastructure/Crypto/</c> adapter rethrows it.
/// </para>
/// <para>
/// An authentication failure generally means the supplied ciphertext, key, or
/// associated data was tampered with, mismatched, or otherwise invalid for
/// authentication purposes and must not be trusted.
/// </para>
/// </remarks>
/// <seealso cref="CryptoServiceException"/>
/// <seealso cref="CryptographicArgumentException"/>
/// <seealso cref="ICryptoService{T}"/>
public sealed class CryptoAuthenticationException : CryptoServiceException
{
    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoAuthenticationException"/>
    /// class with a default message.
    /// </summary>
    public CryptoAuthenticationException()
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoAuthenticationException"/>
    /// class with a specified error message.
    /// </summary>
    /// <param name="message">Message that describes the authentication failure.</param>
    public CryptoAuthenticationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoAuthenticationException"/>
    /// class with a specified error message and a reference to the inner
    /// exception that caused the authentication failure.
    /// </summary>
    /// <param name="message">Message that describes the authentication failure.</param>
    /// <param name="innerException">
    /// The <c>System.Security.Cryptography.CryptographicException</c> (or other
    /// library exception) that is the cause of the current exception, or
    /// <see langword="null"/> if no inner exception is specified.
    /// </param>
    public CryptoAuthenticationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}