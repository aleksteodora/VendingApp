using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.Shared.DTOs;

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
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDataIn dataIn)
        {
            try
            {
                var user = await _userService.CreateAsync(dataIn);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Korisnik sa ovim serijskim brojem brojila već postoji." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDataIn dataIn)
        {
            try
            {
                var user = await _userService.UpdateAsync(id, dataIn);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Korisnik sa ovim serijskim brojem brojila već postoji." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}