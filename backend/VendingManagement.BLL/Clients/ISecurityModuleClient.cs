using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Clients
{
    public interface ISecurityModuleClient
    {
        Task<string> RequestTokenAsync(TokenRequestDataIn dataIn);
    }
}