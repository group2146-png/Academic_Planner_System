using System.Security.Cryptography;

namespace Academic__Planner
{
    /// <summary>
    /// Password hashing gamit ang PBKDF2 (built-in sa .NET, walang extra package).
    /// Format na naka-save sa database: iterations.salt.hash  (salt at hash ay Base64)
    /// </summary>
    internal static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            try
            {
                string[] parts = storedHash.Split('.');
                if (parts.Length != 3) return false;

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expected = Convert.FromBase64String(parts[2]);

                byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
                    password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }
    }
}
