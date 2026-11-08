using System.Security.Cryptography;
using System.Text;

namespace CactiShop.Api.Services;

public static class Encryptor
{
    public static (string Hash, string Salt) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(48);
        return (Compute(password, salt), Convert.ToBase64String(salt));
    }

    public static bool Verify(string password, string hash, string salt)
    {
        var computed = Convert.FromBase64String(Compute(password, Convert.FromBase64String(salt)));
        return CryptographicOperations.FixedTimeEquals(computed, Convert.FromBase64String(hash));
    }

    private static string Compute(string password, byte[] salt)
    {
        var bytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
        return Convert.ToBase64String(SHA256.HashData(bytes));
    }
}
