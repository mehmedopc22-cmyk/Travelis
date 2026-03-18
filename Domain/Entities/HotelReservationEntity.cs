namespace Domain.Entities
{
    public class HotelReservationEntity
    {
        public required Guid Id { get; set; }
        public required Guid HotelId { get; set; }
        public required Guid UserId { get; set; }
        public required Guid RoomId { get; set; }
        public required DateTimeOffset CheckIn { get; set; }
        public required DateTimeOffset CheckOut { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } = null;
        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}
