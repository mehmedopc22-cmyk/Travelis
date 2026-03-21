namespace Domain.Entities
{
    public class HotelEntity
    {
        public Guid Id { get; set; } = Guid.Empty;
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string PostalCode { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required int Status { get; set; }
        public required bool Approved { get; set; }

        public DateTime? CreatedAt { get; set; } = new DateTime();
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
