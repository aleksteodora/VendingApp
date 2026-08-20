using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDataIn dataIn)
        {
            try
            {
                var result = await _userService.CreateAsync(dataIn);
                return StatusCode((int)result.Status, result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return BadRequest(new { message = "User with this meter serial number already exists." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDataIn dataIn)
        {
            try
            {
                var result = await _userService.UpdateAsync(id, dataIn);
                return StatusCode((int)result.Status, result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return BadRequest(new { message = "Korisnik sa ovim serijskim brojem brojila već postoji." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            return StatusCode((int)result.Status, result);
        }
    }
}