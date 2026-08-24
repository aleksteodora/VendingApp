using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService, ILogger<UserController> logger)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _customerService.GetAllAsync(pageNumber, pageSize);
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDataIn dataIn)
        {
            try
            {
                var result = await _customerService.CreateAsync(dataIn);
                return StatusCode((int)result.Status, result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create customer: duplicate meter serial number {MeterSerialNumber}.", dataIn.MeterSerialNumber);
                return BadRequest(new { message = "User with this meter serial number already exists." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDataIn dataIn)
        {
            try
            {
                var result = await _customerService.UpdateAsync(id, dataIn);
                return StatusCode((int)result.Status, result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update customer {CustomerId}: duplicate meter serial number {MeterSerialNumber}.", id, dataIn.MeterSerialNumber);
                return BadRequest(new { message = "User with this meter serial number already exists." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            return StatusCode((int)result.Status, result);
        }
    }
}