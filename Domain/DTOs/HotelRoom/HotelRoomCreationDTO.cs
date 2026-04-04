namespace Domain.DTOs
{
    public class HotelRoomCreationDTO
    {
        public Guid HotelId { get; set; } = Guid.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = -1;
        public string RoomNo { get; set; } = string.Empty;
        public int Floor { get; set; } = -1;
        public int BedCount { get; set; } = -1;
        public int Capacity { get; set; } = -1;
    }
}
