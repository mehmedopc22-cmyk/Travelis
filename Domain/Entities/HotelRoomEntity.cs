namespace Domain.Entities
{
    public class HotelRoomEntity
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid HotelID { get; set; } = Guid.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = -1;
        public string RoomNo { get; set; } = string.Empty;
        public int Floor { get; set; } = -1;
        public int BedCount { get; set; } = -1;
        public int Capacity { get; set; } = -1;

        public DateTime? CreatedAt { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
