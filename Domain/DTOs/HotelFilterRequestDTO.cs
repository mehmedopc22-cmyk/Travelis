namespace Domain.DTOs
{
    public class HotelFilterRequestDTO
    {
        public string? Destination { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
    }
}
