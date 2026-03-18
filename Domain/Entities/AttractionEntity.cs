namespace Domain.Entities
{
    public class AttractionEntity
    {
        public Guid ID { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string PostalCode { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required int Status { get; set; }
        public required bool Approved { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } = null;
        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}
