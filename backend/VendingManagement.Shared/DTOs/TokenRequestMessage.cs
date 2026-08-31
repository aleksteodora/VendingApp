namespace VendingManagement.Shared.DTOs
{
    public class TokenRequestMessage
    {
        public int TransactionId { get; set; }
        public string MeterSerialNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
