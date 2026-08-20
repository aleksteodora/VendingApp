using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ResponsePackage<TokenResponseDataOut>> ProcessTransactionAsync(TokenRequestDataIn dataIn);
    }
}