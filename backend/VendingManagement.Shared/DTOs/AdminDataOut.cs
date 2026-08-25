namespace VendingManagement.Shared.DTOs
{
    public class AdminDataOut
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}