using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.BLL.Services.Interfaces;
using System.Data;

namespace VendingManagement.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _passwordService = passwordService;
        }

        public async Task<ResponsePackage<AdminLoginResponseDataOut>> LoginAsync(AdminLoginDataIn dataIn)
        {
            var admin = await _unitOfWork.AdminRepository.GetByEmailAsync(dataIn.Email);

            if (admin == null)
            {
                return new ResponsePackage<AdminLoginResponseDataOut>(
                    ResponseStatus.Unauthorized,
                    "Invalid email or password.");
            }

            bool passwordValid = _passwordService.VerifyPassword(dataIn.Password, admin.PasswordHash);

            if (!passwordValid)
            {
                return new ResponsePackage<AdminLoginResponseDataOut>(
                    ResponseStatus.Unauthorized,
                    "Invalid email or password.");
            }

            var token = GenerateJwtToken(admin.Id, admin.Email, admin.Role);

            var result = new AdminLoginResponseDataOut
            {
                Token = token,
                Admin = new AdminDataOut
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    FullName = admin.FullName,
                    Role = admin.Role
                }
            };

            return new ResponsePackage<AdminLoginResponseDataOut>(
                result,
                ResponseStatus.OK,
                "Login successful.");
        }

        private string GenerateJwtToken(int adminId, string email, AdminRole role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            //var secret = _configuration["Jwt:Secret"];

            var claims = new List<Claim>
            {
                new Claim("AdminId", adminId.ToString()),
                new Claim("Email", email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}