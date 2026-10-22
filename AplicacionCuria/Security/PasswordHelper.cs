using System.Security.Cryptography;

namespace AplicacionCuria.Security;

public class PasswordHelper
{
    private const int Iterations = 310000;
    private const int HashSize = 32;
    private const int SaltSize = 16;

    public static HashedPassword GenerateHash(string password)
    {
        byte[] salt = GenerateSalt();
        byte[] hash = GenerateHashWithPBKDF2(password, salt, Iterations);
        var hashedPassword = new HashedPassword
        {
            Password = Convert.ToBase64String(hash),
            Salt = Convert.ToBase64String(salt),
            Iterations = Iterations
        };
        return hashedPassword;
    }

    private static byte[] GenerateSalt()
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private static byte[] GenerateHashWithPBKDF2(string password, byte[] salt, int iterations)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(HashSize);
    }

    public static bool VerifyPassword(string password, string storedSaltBase64, string storedHashBase64, int iterations = Iterations)
    {
        byte[] storedSalt = Convert.FromBase64String(storedSaltBase64);
        byte[] storedHash = Convert.FromBase64String(storedHashBase64);
        byte[] computedHash = GenerateHashWithPBKDF2(password, storedSalt, iterations);
        return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
    }    

}
