using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.DAL.Repositories;

namespace VendingManagement.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public AdminService(IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<ResponsePackage<PagedResult<AdminDataOut>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 20;

            var (admins, totalCount) = await _unitOfWork.AdminRepository.GetPagedAsync(pageNumber, pageSize);

            var items = admins
                .Select(a => new AdminDataOut
                {
                    Id = a.Id,
                    Email = a.Email,
                    FullName = a.FullName
                })
                .ToList();

            var pagedResult = new PagedResult<AdminDataOut>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return new ResponsePackage<PagedResult<AdminDataOut>>(
                pagedResult,
                ResponseStatus.OK,
                "Admins retrieved successfully.");
        }

        public async Task<ResponsePackage<AdminDataOut>> GetByIdAsync(int id)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id);

            if (admin == null || admin.IsDeleted)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.NotFound,
                    "Admin not found.");
            }

            return new ResponsePackage<AdminDataOut>(
                MapToDataOut(admin),
                ResponseStatus.OK,
                "Admin retrieved successfully.");
        }

        public async Task<ResponsePackage<AdminDataOut>> CreateAsync(AdminDataIn dataIn)
        {
            if (string.IsNullOrWhiteSpace(dataIn.Password))
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.BadRequest,
                    "Password is required when creating a new admin.");
            }

            var existing = await _unitOfWork.AdminRepository.GetByEmailAsync(dataIn.Email);
            if (existing != null && !existing.IsDeleted)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.BadRequest,
                    "An admin with this email already exists.");
            }

            var admin = new Admin
            {
                Email = dataIn.Email,
                PasswordHash = _passwordService.HashPassword(dataIn.Password),
                FullName = dataIn.FullName,
                Role = AdminRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AdminRepository.AddAsync(admin);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<AdminDataOut>(
                MapToDataOut(admin),
                ResponseStatus.Created,
                "Admin created successfully.");
        }

        public async Task<ResponsePackage<AdminDataOut>> UpdateAsync(int id, AdminDataIn dataIn)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id);

            if (admin == null || admin.IsDeleted)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.NotFound,
                    "Admin not found.");
            }

            admin.Email = dataIn.Email;
            admin.FullName = dataIn.FullName;

            if (!string.IsNullOrWhiteSpace(dataIn.Password))
            {
                admin.PasswordHash = _passwordService.HashPassword(dataIn.Password);
            }

            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<AdminDataOut>(
                MapToDataOut(admin),
                ResponseStatus.OK,
                "Admin updated successfully.");
        }

        public async Task<ResponsePackageNoData> DeleteAsync(int id)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id);

            if (admin == null || admin.IsDeleted)
            {
                return new ResponsePackageNoData(
                    ResponseStatus.NotFound,
                    "Admin not found.");
            }

            if (admin.Role == AdminRole.SuperAdmin)
            {
                return new ResponsePackageNoData(
                    ResponseStatus.BadRequest,
                    "The super admin cannot be deleted.");
            }

            admin.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackageNoData(
                ResponseStatus.OK,
                "Admin deleted successfully.");
        }

        public async Task<ResponsePackageNoData> ChangePasswordAsync(int adminId, ChangePasswordDataIn dataIn)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(adminId);

            if (admin == null || admin.IsDeleted)
            {
                return new ResponsePackageNoData(
                    ResponseStatus.NotFound,
                    "Admin not found.");
            }

            bool currentPasswordValid = _passwordService.VerifyPassword(dataIn.CurrentPassword, admin.PasswordHash);

            if (!currentPasswordValid)
            {
                return new ResponsePackageNoData(
                    ResponseStatus.BadRequest,
                    "Current password is incorrect.");
            }

            admin.PasswordHash = _passwordService.HashPassword(dataIn.NewPassword);
            _unitOfWork.AdminRepository.Update(admin);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackageNoData(
                ResponseStatus.OK,
                "Password changed successfully.");
        }

        private static AdminDataOut MapToDataOut(Admin admin)
        {
            return new AdminDataOut
            {
                Id = admin.Id,
                Email = admin.Email,
                FullName = admin.FullName,
                Role = admin.Role
            };
        }
    }
}