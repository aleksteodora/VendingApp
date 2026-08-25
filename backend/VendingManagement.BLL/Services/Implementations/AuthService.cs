using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponsePackage<AdminDataOut>> LoginAsync(AdminLoginDataIn dataIn)
        {
            var admin = await _unitOfWork.AdminRepository.GetByEmailAsync(dataIn.Email);

            if (admin == null)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.Unauthorized,
                    "Invalid email or password.");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dataIn.Password, admin.PasswordHash);

            if (!passwordValid)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.Unauthorized,
                    "Invalid email or password.");
            }

            var result = new AdminDataOut
            {
                Id = admin.Id,
                Email = admin.Email,
                FullName = admin.FullName,
                IsSuperAdmin = admin.IsSuperAdmin
            };

            return new ResponsePackage<AdminDataOut>(
                result,
                ResponseStatus.OK,
                "Login successful.");
        }
    }
}