using VendingManagement.Shared.DTOs;

namespace VendingManagement.WebApp.Services
{
    public interface ISecurityModuleClient
    {
        Task<string> RequestTokenAsync(TokenRequestDataIn dataIn);
    }
}