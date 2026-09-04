using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.WebApp.Security;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    [ApiKeyAuth]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;

        public TransactionController(ITransactionService transactionService, ICustomerService customerService)
        {
            _transactionService = transactionService;
            _customerService = customerService;
        }

        [HttpPost("buy-token")]
        public async Task<IActionResult> BuyToken([FromBody] TokenRequestDataIn dataIn)
        {
            var result = await _transactionService.ProcessTransactionAsync(dataIn);
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatus(Guid id)
        {
            var result = await _transactionService.GetTransactionStatusAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("webhook-url")]
        public async Task<IActionResult> UpdateWebhookUrl([FromBody] WebhookUrlDataIn dataIn)
        {
            var apiKey = Request.Headers["x-api-key"].ToString();
            var result = await _customerService.UpdateWebhookUrlByApiKeyAsync(apiKey, dataIn);
            return StatusCode((int)result.Status, result);
        }
    }
}