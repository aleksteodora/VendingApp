namespace VendingManagement.Shared.DTOs
{
    public class TransactionStatusDataOut
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string? Token { get; set; }
        public decimal EnergyAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
    }
}