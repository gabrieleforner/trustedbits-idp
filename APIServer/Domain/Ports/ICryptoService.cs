using Trustedbits.APIServer.Domain.Models;

namespace Trustedbits.APIServer.Domain.Ports;

public interface ICryptoService<T> where T : SecretBase
{
    Task<T> EncryptAsync(T plainSecret, CancellationToken cancellation = default);
    Task<T> DecryptAsync(T cipherSecret, CancellationToken cancellation = default);
}