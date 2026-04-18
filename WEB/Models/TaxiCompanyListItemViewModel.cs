namespace WEB.Models
{
    public class TaxiCompanyListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Approved { get; set; }
        public int Status { get; set; }

        public string FullAddress => $"{Street}, {City} {PostalCode}, {Country}";
    }
}
