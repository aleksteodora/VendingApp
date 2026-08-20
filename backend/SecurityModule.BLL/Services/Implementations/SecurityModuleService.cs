using SecurityModule.BLL.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.Shared.DTOs;

namespace SecurityModule.BLL.Services.Implementations
{
    public class SecurityModuleService : ISecurityModuleService
    {
        public ResponsePackage<string> GenerateRandomToken(TokenRequestDataIn dataIn)
        {
            StringBuilder result = new StringBuilder(20);

            for (int i = 0; i < 20; i++)
            {
                result.Append(RandomNumberGenerator.GetInt32(0, 10));
            }

            return new ResponsePackage<string> (result.ToString(), ResponseStatus.OK, "Token je uspešno generisan.");
        }
    }
}
