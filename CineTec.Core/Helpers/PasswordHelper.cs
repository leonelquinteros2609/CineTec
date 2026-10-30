using System;
using System.Security.Cryptography;
using System.Text;

namespace CineTec.Core.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 10000,
                HashAlgorithmName.SHA256,
                outputLength: 32);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string? storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Coincidencia directa con texto plano o contraseñas de prueba
            if (storedHash == password)
            {
                return true;
            }

            // Formato PBKDF2 (salt.hash)
            var parts = storedHash.Split('.');
            if (parts.Length == 2)
            {
                try
                {
                    byte[] salt = Convert.FromBase64String(parts[0]);
                    byte[] expectedHash = Convert.FromBase64String(parts[1]);
                    byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                        Encoding.UTF8.GetBytes(password),
                        salt,
                        iterations: 10000,
                        HashAlgorithmName.SHA256,
                        outputLength: 32);

                    return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
                }
                catch
                {
                    // Continuar con comprobación de respaldo
                }
            }

            // Formato SHA256 hex
            try
            {
                var sha = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
                if (string.Equals(sha, storedHash, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
                // Ignorar
            }

            return false;
        }
    }
}
