using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponsePackage<PagedResult<UserDataOut>>> GetAllAsync(int pageNumber, int pageSize);
        Task<ResponsePackage<UserDataOut>> GetByIdAsync(int id);
        Task<ResponsePackage<UserDataOut>> CreateAsync(UserDataIn dataIn);
        Task<ResponsePackage<UserDataOut>> UpdateAsync(int id, UserDataIn dataIn);
        Task<ResponsePackageNoData> DeleteAsync(int id);
    }
}