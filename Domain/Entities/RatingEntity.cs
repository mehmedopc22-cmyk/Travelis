namespace Domain.Entities
{
    public class RatingEntity
    {
        public required Guid Id { get; set; }
        public required bool Disabled { get; set; }
        public required int Rating { get; set; }

        public DateTimeOffset? CreatedAt { get; set; } = null;
    }
}
