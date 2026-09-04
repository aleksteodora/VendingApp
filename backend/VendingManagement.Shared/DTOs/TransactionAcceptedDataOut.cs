namespace VendingManagement.Shared.DTOs
{
    public class TransactionAcceptedDataOut
    {
        public Guid TransactionId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}