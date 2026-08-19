using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("buy-token")]
        public async Task<IActionResult> BuyToken([FromBody] TokenRequestDataIn dataIn)
        {
            try
            {
                var result = await _transactionService.ProcessTransactionAsync(dataIn);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { 
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}