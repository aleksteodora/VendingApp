namespace VendingManagement.Shared.DTOs
{
    public class AdminLoginResponseDataOut
    {
        public string Token { get; set; }
        public AdminDataOut Admin { get; set; }
    }
}