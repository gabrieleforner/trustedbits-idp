namespace Trustedbits.APIServer.Domain.Models;

public abstract class SecretBase
{
    public Guid Id { get; set; }
    public byte[] SecretDEK { get; set; } = [];
    public byte[] SecretKEK { get; set; } = [];
}