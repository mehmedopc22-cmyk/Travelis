namespace WEB.Models
{
    public class HotelRoomViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string RoomNo { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int BedCount { get; set; }
        public int Capacity { get; set; }
    }
}