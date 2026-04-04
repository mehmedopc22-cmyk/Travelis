namespace Domain.DTOs.HotelRoom
{
    public class HotelRoomPatchDTO
    {
        public string? Description { get; set; } = null;
        public decimal? Price { get; set; } = null;
        public string? RoomNo { get; set; } = null;
        public int? Floor { get; set; } = null;
        public int? BedCount { get; set; } = null;
        public int? Capacity { get; set; } = null;
    }
}
