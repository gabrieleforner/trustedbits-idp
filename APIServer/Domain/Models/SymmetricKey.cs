namespace Trustedbits.APIServer.Domain.Models;

public class SymmetricKey : SecretBase
{
    public byte[] Key { get; set; } = [];
}