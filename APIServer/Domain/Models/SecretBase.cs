namespace Trustedbits.APIServer.Domain.Models;

/// <summary>
/// Abstract class that contains the common attributes shared across
/// different types of secrets.
/// </summary>
public abstract class SecretBase
{
    /// <summary>
    /// Unique identifier of the secret
    /// </summary>
    public Guid SecretId { get; set; } = Guid.Empty;

    /// <summary>
    /// Unique name of the secret
    /// </summary>
    public string SecretName { get; set; } = "";
    
    /// <summary>
    /// Secret Data Encryption Key
    /// </summary>
    public byte[] SecretDEK { get; set; } = [];
    
    /// <summary>
    /// Secret Key Encryption Key
    /// </summary>
    public byte[] SecretKEK { get; set; } = [];
}