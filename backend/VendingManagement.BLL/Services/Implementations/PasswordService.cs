using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public bool VerifyPassword(string plainPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
        }
    }
}