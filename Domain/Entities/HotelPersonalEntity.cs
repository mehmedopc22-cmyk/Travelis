namespace Domain.Entities
{
    public class HotelPersonalEntity
    {
        public Guid HotelId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
    }
}
