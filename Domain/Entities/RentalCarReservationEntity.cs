namespace Domain.Entities
{
    public class RentalCarReservationEntity
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid CarId { get; set; }
        public required DateTime UseFrom { get; set; }
        public required DateTime UseTo { get; set; }

        public DateTimeOffset? CreatedAt { get; set; } = null;
        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}
