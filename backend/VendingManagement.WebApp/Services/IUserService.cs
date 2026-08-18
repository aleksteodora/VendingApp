using VendingManagement.WebApp.DTOs;

namespace VendingManagement.WebApp.Services
{
    public interface IUserService
    {
        Task<List<UserDataOut>> GetAllAsync();
        Task<UserDataOut> GetByIdAsync(int id);
        Task<UserDataOut> CreateAsync(UserDataIn dataIn);
        Task<UserDataOut> UpdateAsync(int id, UserDataIn dataIn);
        Task DeleteAsync(int id);
    }
}