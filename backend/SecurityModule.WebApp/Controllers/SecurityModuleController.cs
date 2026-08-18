using Microsoft.AspNetCore.Mvc;
using SecurityModule.BLL.Services.Interfaces;
using SecurityModule.WebApp.Security;
using VendingManagement.Shared.DTOs;

namespace SecurityModule.WebApp.Controllers
{
    [Route("api/security-module")]
    [ApiController]
    public class SecurityModuleController : ControllerBase
    {
        private readonly ISecurityModuleService _securityModuleService;
        public SecurityModuleController(ISecurityModuleService securityModuleService) 
        { 
            _securityModuleService = securityModuleService;
        }

        [HttpPost("credit")]
        [TypeFilter(typeof(AuthorizeApiKeyAttribute), Arguments = new object[] { "SecurityModuleApiKey", "x-api-key" })]
        public IActionResult GenerateRandomToken(TokenRequestDataIn dataIn)
        {
            return Ok(_securityModuleService.GenerateRandomToken(dataIn));
        }
    }
}
