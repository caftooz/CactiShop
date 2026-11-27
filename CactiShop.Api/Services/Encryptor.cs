using System.Security.Cryptography;
using System.Text;

namespace CactiShop.Api.Services;

/// <summary>
/// Password hashing with PBKDF2. Legacy SHA-256 hashes are verified during login
/// and upgraded transparently after a successful authentication.
/// </summary>
public static class Encryptor
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 210_000;
    private const string VersionPrefix = "pbkdf2-sha256$";

    public static (string Hash, string Salt) HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (Convert.ToBase64String(hash), VersionPrefix + Convert.ToBase64String(salt));
    }

    public static bool Verify(string password, string hash, string salt)
    {
        try
        {
            var isCurrent = salt.StartsWith(VersionPrefix, StringComparison.Ordinal);
            var saltValue = isCurrent ? salt[VersionPrefix.Length..] : salt;
            var saltBytes = Convert.FromBase64String(saltValue);
            var expected = Convert.FromBase64String(hash);

            var actual = isCurrent
                ? Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA256, expected.Length)
                : SHA256.HashData(Encoding.UTF8.GetBytes(password).Concat(saltBytes).ToArray());

            return actual.Length == expected.Length &&
                   CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static bool NeedsUpgrade(string salt) =>
        !salt.StartsWith(VersionPrefix, StringComparison.Ordinal);
}
