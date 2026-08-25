using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDataIn dataIn)
        {
            var result = await _authService.LoginAsync(dataIn);
            return StatusCode((int)result.Status, result);
        }
    }
}