namespace Domain.DTOs
{
    public class HotelRoomHotelResponseDTO
    {
        public Guid HotelId { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
