namespace Trustedbits.APIServer.Domain.Models;

/// <summary>
/// Concrete implementation of <c>SecretBase</c> that represents
/// a pair of keys, used with asymmetric algorithms
/// </summary>
/// <see cref="SecretBase"/>
public sealed class KeyPair : SecretBase
{
    /// <summary>
    /// Public key bytes
    /// </summary>
    public byte[] PublicKey { get; set; } = [];
    
    /// <summary>
    /// Private key bytes
    /// </summary>
    public byte[] PrivateKey { get; set; } = [];
}