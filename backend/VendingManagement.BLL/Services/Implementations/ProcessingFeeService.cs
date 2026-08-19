using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class ProcessingFeeService : IProcessingFeeService
    {
        private readonly VendingDbContext _context;

        public ProcessingFeeService(VendingDbContext context)
        {
            _context = context;
        }

        public async Task<ProcessingFee> GetActiveFeeAsync()
        {
            var activeFee = await _context.ProcessingFees
                .Where(f => f.IsActive)
                .FirstOrDefaultAsync();

            if (activeFee == null)
            {
                throw new InvalidOperationException("No active processing fee found.");
            }

            return activeFee;
        }

        public async Task<ProcessingFee> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate)
        {
            var currentActiveFees = await _context.ProcessingFees
                .Where(f => f.IsActive)
                .ToListAsync();

            foreach (var fee in currentActiveFees)
            {
                fee.IsActive = false;
            }

            var newFee = new ProcessingFee
            {
                FixedAmount = fixedAmount,
                PercentageRate = percentageRate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProcessingFees.Add(newFee);
            await _context.SaveChangesAsync();

            return newFee;
        }
    }
}
