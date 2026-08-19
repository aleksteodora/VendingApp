namespace VendingManagement.Shared.DTOs
{
    public class TokenResponseDataOut
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public decimal EnergyAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
    }
}