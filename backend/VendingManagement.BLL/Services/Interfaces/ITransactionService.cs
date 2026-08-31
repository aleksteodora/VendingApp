using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ResponsePackage<TransactionAcceptedDataOut>> ProcessTransactionAsync(TokenRequestDataIn dataIn);
        Task CompleteTransactionAsync(TokenRequestMessage message);
    }
}