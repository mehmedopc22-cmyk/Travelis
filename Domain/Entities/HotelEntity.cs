namespace Domain.Entities
{
    public class HotelEntity
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Status { get; set; } = -1;
        public bool Approved { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;

        public List<ImageEntity> Images { get; set; } = [];
    }
}
