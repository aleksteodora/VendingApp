using Microsoft.EntityFrameworkCore;
using VendingManagement.Shared.DTOs;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Clients;

namespace VendingManagement.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly VendingDbContext _context;
        private readonly IProcessingFeeService _processingFeeService;
        private readonly ISecurityModuleClient _securityModuleClient;

        public TransactionService(
            VendingDbContext context,
            IProcessingFeeService processingFeeService,
            ISecurityModuleClient securityModuleClient)
        {
            _context = context;
            _processingFeeService = processingFeeService;
            _securityModuleClient = securityModuleClient;
        }

        public async Task<TokenResponseDataOut> ProcessTransactionAsync(TokenRequestDataIn dataIn)
        {
            // nadji brojilo po serijskom broju zajedno sa korisnikom
            var meter = await _context.Meters
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MeterSerialNumber == dataIn.MeterSerialNumber);

            if (meter == null)
            {
                throw new KeyNotFoundException("Meter with given serial number was not found.");
            }

            // uzmi trenutno aktivan processingfee
            var activeFee = await _processingFeeService.GetActiveFeeAsync();

            // racunamo trosak obrade i preostalu energiju
            decimal processingFeeAmount = activeFee.FixedAmount + (dataIn.Amount * activeFee.PercentageRate);
            decimal energyAmount = dataIn.Amount - processingFeeAmount;

            // transakcija pending pre poziva security modula
            var transaction = new Transaction
            {
                MeterId = meter.Id,
                Amount = dataIn.Amount,
                ProcessingFeeAmount = processingFeeAmount,
                EnergyAmount = energyAmount,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // pozovi security module da generise token
            string token;
            try
            {
                token = await _securityModuleClient.RequestTokenAsync(dataIn);
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                await _context.SaveChangesAsync();
                throw;
            }

            // uspesno, azuriraj transakciju sa tokenom i statusom completed
            transaction.Token = token;
            transaction.Status = TransactionStatus.Completed;
            await _context.SaveChangesAsync();

            // vrati racun korisniku
            return new TokenResponseDataOut
            {
                FullName = meter.User.FullName,
                Address = meter.User.Address,
                PhoneNumber = meter.User.PhoneNumber,
                Token = token,
                EnergyAmount = energyAmount,
                ProcessingFeeAmount = processingFeeAmount
            };

        }
    }
}