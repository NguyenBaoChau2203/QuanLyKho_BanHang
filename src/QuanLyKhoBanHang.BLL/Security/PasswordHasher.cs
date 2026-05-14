using System.Security.Cryptography;

namespace QuanLyKhoBanHang.BLL.Security;

public static class PasswordHasher
{
    private const int SaltBytes = 16;
    private const int HashBytes = 32;
    private const int Iterations = 100000;
    private const string VersionPrefix = "v1";
    private const char Separator = ':';

    public static string Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password must not be null or empty.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = ComputeHash(password, salt, Iterations);
        return $"{VersionPrefix}{Separator}{Iterations}{Separator}{Convert.ToBase64String(salt)}{Separator}{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password))
            return false;
        if (string.IsNullOrEmpty(storedHash))
            return false;

        try
        {
            var parts = storedHash.Split(Separator);
            if (parts.Length != 4 || parts[0] != VersionPrefix)
                return false;
            if (!int.TryParse(parts[1], out var iterations) || iterations <= 0)
                return false;

            var salt = Convert.FromBase64String(parts[2]);
            var storedHashBytes = Convert.FromBase64String(parts[3]);
            var computedHash = ComputeHash(password, salt, iterations);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
        }
        catch
        {
            return false;
        }
    }

    public static bool ValidatePasswordPolicy(string password, int minLength = 4)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        if (password.Length < minLength)
            return false;
        return true;
    }

    private static byte[] ComputeHash(string password, byte[] salt, int iterations)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(HashBytes);
    }
}
