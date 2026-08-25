using Microsoft.AspNetCore.Mvc;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace VendingManagement.WebApp.Controllers
{
    [Route("api/processing-fee")]
    [ApiController]
    [Authorize]
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
            var result = await _processingFeeService.GetActiveFeeAsync();
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("change")]
        public async Task<IActionResult> Change([FromBody] ProcessingFeeDataIn dataIn)
        {
            var result = await _processingFeeService.ChangeFeeAsync(dataIn.FixedAmount, dataIn.PercentageRate);
            return StatusCode((int)result.Status, result);
        }
    }
}