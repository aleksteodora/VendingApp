namespace VendingManagement.Shared.DTOs
{
    public class TransactionAcceptedDataOut
    {
        public int TransactionId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}