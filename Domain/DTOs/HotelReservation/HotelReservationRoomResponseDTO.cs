namespace Domain.DTOs
{
    public class HotelReservationRoomResponseDTO
    {
        public Guid HotelRoomId { get; set; } = Guid.Empty;
        public Guid HotelID { get; set; } = Guid.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = -1;
        public string RoomNo { get; set; } = string.Empty;
        public int Floor { get; set; } = -1;
        public int BedCount { get; set; } = -1;
        public int Capacity { get; set; } = -1;
    }
}
