using System;
using System.Security.Cryptography;
using System.Text;

namespace Book.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password, string salt);
        string GenerateSalt();
    }

    public class PasswordService : IPasswordService
    {
        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}

