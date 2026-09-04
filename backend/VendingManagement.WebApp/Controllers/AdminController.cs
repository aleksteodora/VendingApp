using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;
using System.Security.Claims;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _adminService.GetAllAsync(pageNumber, pageSize);
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _adminService.GetByIdAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] AdminDataIn dataIn)
        {
            var result = await _adminService.CreateAsync(dataIn);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminDataIn dataIn)
        {
            var result = await _adminService.UpdateAsync(id, dataIn);
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminService.DeleteAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost("change-password")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDataIn dataIn)
        {
            var adminIdClaim = User.FindFirst("AdminId")?.Value;

            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var result = await _adminService.ChangePasswordAsync(adminId, dataIn);
            return StatusCode((int)result.Status, result);
        }
    }
}