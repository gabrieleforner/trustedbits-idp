namespace Trustedbits.APIServer.Domain.Models;

public class KeyPair : SecretBase
{
    public byte[] PublicKey { get; set; } = [];
    public byte[] PrivateKey { get; set; } = [];
}