using VendingManagement.Shared.Common;
using VendingManagement.Shared.DTOs;

namespace SecurityModule.BLL.Services.Interfaces
{
    public interface ISecurityModuleService
    {
        public ResponsePackage<string> GenerateRandomToken(TokenRequestDataIn dataIn);
    }
}
