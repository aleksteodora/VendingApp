using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponsePackage<AdminDataOut>> LoginAsync(AdminLoginDataIn dataIn);
    }
}