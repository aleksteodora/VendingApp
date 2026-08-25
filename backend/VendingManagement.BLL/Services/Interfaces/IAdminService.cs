using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ResponsePackage<List<AdminDataOut>>> GetAllAsync();
        Task<ResponsePackage<AdminDataOut>> GetByIdAsync(int id);
        Task<ResponsePackage<AdminDataOut>> CreateAsync(AdminDataIn dataIn);
        Task<ResponsePackage<AdminDataOut>> UpdateAsync(int id, AdminDataIn dataIn);
        Task<ResponsePackageNoData> DeleteAsync(int id);
    }
}