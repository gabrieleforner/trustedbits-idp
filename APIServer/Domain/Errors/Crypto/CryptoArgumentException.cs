using Trustedbits.APIServer.Domain.Models;
using Trustedbits.APIServer.Domain.Ports;

namespace Trustedbits.APIServer.Domain.Errors.Crypto;

/// <summary>
/// The exception that is thrown when a <see cref="ICryptoService{T}"/>
/// implementation receives an invalid argument for a cryptographic operation
/// on a <see cref="SecretBase"/>, such as a <see langword="null"/>, empty, or
/// wrong-length key, nonce, or ciphertext.
/// </summary>
/// <remarks>
/// <para>
/// This is the domain translation of the <c>System.ArgumentException</c> family
/// (<c>ArgumentException</c>, <c>ArgumentNullException</c>,
/// <c>ArgumentOutOfRangeException</c>) raised by the .NET cryptography
/// libraries when bad parameters are supplied. The original library exception
/// is retained as the <see cref="Exception.InnerException"/> when the
/// <c>Infrastructure/Crypto/</c> adapter rethrows it.
/// </para>
/// <para>
/// A single combined type is used instead of separate null and range
/// variants because the distinction is rarely actionable for cryptographic
/// callers: either the argument is valid or it is not, and the offending
/// parameter is identified by <see cref="ParamName"/> (when available) and the
/// message.
/// </para>
/// </remarks>
/// <seealso cref="CryptoServiceException"/>
/// <seealso cref="CryptoAuthenticationException"/>
/// <seealso cref="ICryptoService{T}"/>
public sealed class CryptoArgumentException : CryptoServiceException
{
    /// <summary>
    /// Name of the invalid cryptographic parameter, when available; otherwise
    /// <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// Mirrors <c>System.ArgumentException.ParamName</c> from the original
    /// library exception when the adapter can surface it.
    /// </remarks>
    public string? ParamName { get; set; }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoArgumentException"/>
    /// class with a default message.
    /// </summary>
    public CryptoArgumentException()
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoArgumentException"/>
    /// class with a specified error message.
    /// </summary>
    /// <param name="message">Message that describes the invalid argument.</param>
    public CryptoArgumentException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CryptoArgumentException"/>
    /// class with a specified error message and a reference to the inner
    /// exception that identified the invalid argument.
    /// </summary>
    /// <param name="message">Message that describes the invalid argument.</param>
    /// <param name="innerException">
    /// The <c>System.ArgumentException</c> (or derived) exception that is the
    /// cause of the current exception, or <see langword="null"/> if no inner
    /// exception is specified.
    /// </param>
    public CryptoArgumentException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}