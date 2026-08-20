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
            var result = await _transactionService.ProcessTransactionAsync(dataIn);
            return StatusCode((int)result.Status, result);
        }
    }
}