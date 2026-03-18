namespace Domain.Entities
{
    public class HotelRoomEntity
    {
        public required Guid Id { get; set; }
        public required Guid HotelID { get; set; }
        public string Description { get; set; } = string.Empty;
        public required decimal Price { get; set; }
        public required string RoomNo { get; set; }
        public required int Floor { get; set; }
        public required int BedCount { get; set; }
        public required int Capacity { get; set; }

        public DateTimeOffset? CreatedAt { get; set; } = null;
        public DateTimeOffset? UpdatedAt { get; set; } = null;
    }
}
