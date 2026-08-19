using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TokenResponseDataOut> ProcessTransactionAsync(TokenRequestDataIn dataIn);
    }
}