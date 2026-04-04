namespace Domain.DTOs
{
    public class HotelRoomResponseDTO
    {
        public Guid Id { get; set; } = Guid.Empty;
        public HotelRoomHotelResponseDTO Hotel { get; set; } = null;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = -1;
        public string RoomNo { get; set; } = string.Empty;
        public int Floor { get; set; } = -1;
        public int BedCount { get; set; } = -1;
        public int Capacity { get; set; } = -1;
    }
}
