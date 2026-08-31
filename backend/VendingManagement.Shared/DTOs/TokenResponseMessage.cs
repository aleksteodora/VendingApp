namespace VendingManagement.Shared.DTOs
{
    public class TokenResponseMessage
    {
        public int TransactionId { get; set; }
        public string Token { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}