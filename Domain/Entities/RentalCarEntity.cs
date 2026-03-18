namespace Domain.Entities
{
    public class RentalCarEntity
    {
        public required Guid Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required int Kilometers { get; set; }
    }
}
