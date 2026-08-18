using VendingManagement.Shared.DTOs;
using VendingManagement.WebApp.DTOs;

namespace VendingManagement.WebApp.Services
{
    public interface ITransactionService
    {
        Task<TokenResponseDataOut> ProcessTransactionAsync(TokenRequestDataIn dataIn);
    }
}