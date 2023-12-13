using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace PastebinLogic.Services.Implementations;

internal class PasswordService : IPasswordService
{
    public const int NumBytesSalt = 128 / 8;
    public const int IterationCount = 100000;
    public const int NumBytesHash = 256 / 8;
    public const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;

    public (string salt, string hash) GeneratePasswordSaltAndHash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var saltInBytes = RandomNumberGenerator.GetBytes(NumBytesSalt);

        var hashInBytes = GetHashInBytes(password, saltInBytes);

        var salt = Convert.ToBase64String(saltInBytes);
        var hash = Convert.ToBase64String(hashInBytes);

        return (salt, hash);
    }

    public bool PasswordIsValid(string salt, string hash, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(salt, nameof(salt));
        ArgumentException.ThrowIfNullOrWhiteSpace(hash, nameof(hash));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var saltInBytes = Convert.FromBase64String(salt);

        var calculatedHash = Convert.ToBase64String(GetHashInBytes(password, saltInBytes));

        return hash == calculatedHash;
    }

    private static byte[] GetHashInBytes(string password, byte[] saltInBytes) => KeyDerivation.Pbkdf2(password, saltInBytes, Prf, IterationCount, NumBytesHash);
}
