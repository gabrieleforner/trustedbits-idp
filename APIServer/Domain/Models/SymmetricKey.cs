namespace Trustedbits.APIServer.Domain.Models;

/// <summary>
/// Concrete implementation of <c>SecretBase</c> that represents
/// a secret encryption key, used with symmetric algorithms
/// </summary>
/// <see cref="SecretBase"/>
public class SymmetricKey : SecretBase
{
    /// <summary>
    /// Encryption key bytes
    /// </summary>
    public byte[] Key { get; set; } = [];
}