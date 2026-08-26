using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponsePackage<List<AdminDataOut>>> GetAllAsync()
        {
            var admins = await _unitOfWork.AdminRepository.GetAllAsync();
            var items = admins.Where(a => a.Role != AdminRole.SuperAdmin).Select(MapToDataOut).ToList();

            return new ResponsePackage<List<AdminDataOut>>(
                items,
                ResponseStatus.OK,
                "Admins retrieved successfully.");
        }

        public async Task<ResponsePackage<AdminDataOut>> GetByIdAsync(int id)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id);

            if (admin == null)
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
            var existing = await _unitOfWork.AdminRepository.GetByEmailAsync(dataIn.Email);
            if (existing != null)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.BadRequest,
                    "An admin with this email already exists.");
            }

            var admin = new Admin
            {
                Email = dataIn.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dataIn.Password),
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

            if (admin == null)
            {
                return new ResponsePackage<AdminDataOut>(
                    ResponseStatus.NotFound,
                    "Admin not found.");
            }

            admin.Email = dataIn.Email;
            admin.FullName = dataIn.FullName;
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dataIn.Password);

            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<AdminDataOut>(
                MapToDataOut(admin),
                ResponseStatus.OK,
                "Admin updated successfully.");
        }

        public async Task<ResponsePackageNoData> DeleteAsync(int id)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id);

            if (admin == null)
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

            _unitOfWork.AdminRepository.Remove(admin);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackageNoData(
                ResponseStatus.OK,
                "Admin deleted successfully.");
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