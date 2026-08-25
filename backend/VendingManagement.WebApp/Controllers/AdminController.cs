using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Policy = "SuperAdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _adminService.GetAllAsync();
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _adminService.GetByIdAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminDataIn dataIn)
        {
            var result = await _adminService.CreateAsync(dataIn);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminDataIn dataIn)
        {
            var result = await _adminService.UpdateAsync(id, dataIn);
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminService.DeleteAsync(id);
            return StatusCode((int)result.Status, result);
        }
    }
}