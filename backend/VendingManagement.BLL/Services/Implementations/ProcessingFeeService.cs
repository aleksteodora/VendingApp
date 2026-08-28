using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Caching;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace VendingManagement.BLL.Services.Implementations
{
    public class ProcessingFeeService : IProcessingFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessingFeeService> _logger;
        private readonly ICacheService _cacheService;

        private const string ActiveFeeCacheKey = "processing-fee:active";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public ProcessingFeeService(IUnitOfWork unitOfWork, ILogger<ProcessingFeeService> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<ResponsePackage<ProcessingFee>> GetActiveFeeAsync()
        {
            var cached = await _cacheService.GetAsync<ProcessingFee>(ActiveFeeCacheKey);

            if (cached != null)
            {
                _logger.LogInformation("Active processing fee retrieved from cache.");
                return new ResponsePackage<ProcessingFee>(
                    cached,
                    ResponseStatus.OK,
                    "Active processing fee retrieved successfully from cache.");
            }

            var activeFee = await _unitOfWork.ProcessingFeeRepository.GetActiveProcessingFeeAsync();

            if (activeFee == null)
            {
                _logger.LogWarning("No active processing fee found in the database.");
                return new ResponsePackage<ProcessingFee>(
                    ResponseStatus.NotFound,
                    "No active processing fee found in the data.");
            }

            await _cacheService.SetAsync(ActiveFeeCacheKey, activeFee, CacheDuration);
            _logger.LogInformation("Active processing fee retrieved from database and cached.");

            return new ResponsePackage<ProcessingFee>(
                activeFee,
                ResponseStatus.OK,
                "Active processing fee retrieved successfully from database.");
        }

        public async Task<ResponsePackage<ProcessingFee>> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate)
        {
            var currentActiveFees = await _unitOfWork.ProcessingFeeRepository.GetActiveProcessingFeesAsync();

            foreach (var fee in currentActiveFees)
            {
                fee.IsDeleted = true;
            }

            var newFee = new ProcessingFee
            {
                FixedAmount = fixedAmount,
                PercentageRate = percentageRate,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ProcessingFeeRepository.AddAsync(newFee);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync(ActiveFeeCacheKey);

            _logger.LogInformation("Processing fee changed: FixedAmount={FixedAmount}, PercentageRate={PercentageRate}. Cache invalidated.", fixedAmount, percentageRate);

            return new ResponsePackage<ProcessingFee>(
                newFee,
                ResponseStatus.OK,
                "Processing fee changed successfully.");
        }

        public async Task<ResponsePackage<List<ProcessingFee>>> GetHistoryAsync()
        {
            var allFees = await _unitOfWork.ProcessingFeeRepository.GetAllAsync();

            var ordered = allFees.OrderByDescending(f => f.CreatedAt).ToList();

            return new ResponsePackage<List<ProcessingFee>>(
                ordered,
                ResponseStatus.OK,
                "Processing fee history retrieved successfully.");
        }
    }
}