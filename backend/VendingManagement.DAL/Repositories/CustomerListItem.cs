namespace VendingManagement.DAL.Repositories
{
    public class CustomerListItem
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string? MeterSerialNumber { get; set; }
    }
}