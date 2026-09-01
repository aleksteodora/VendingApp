using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ResponsePackage<TransactionAcceptedDataOut>> ProcessTransactionAsync(TokenRequestDataIn dataIn);
        Task HandleTokenResponseAsync(TokenResponseMessage response);
        Task<ResponsePackage<TransactionStatusDataOut>> GetTransactionStatusAsync(int transactionId);
    }
}