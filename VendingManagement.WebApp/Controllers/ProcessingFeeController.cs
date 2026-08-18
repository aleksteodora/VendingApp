using Microsoft.AspNetCore.Mvc;
using VendingManagement.WebApp.DTOs;
using VendingManagement.WebApp.Services;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/processing-fee")]
    [ApiController]
    public class ProcessingFeeController : ControllerBase
    {
        private readonly IProcessingFeeService _processingFeeService;

        public ProcessingFeeController(IProcessingFeeService processingFeeService)
        {
            _processingFeeService = processingFeeService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var fee = await _processingFeeService.GetActiveFeeAsync();
            return Ok(fee);
        }

        [HttpPut("change")]
        public async Task<IActionResult> Change([FromBody] ProcessingFeeDataIn dataIn)
        {
            var fee = await _processingFeeService.ChangeFeeAsync(dataIn.FixedAmount, dataIn.PercentageRate);
            return Ok(fee);
        }
    }
}