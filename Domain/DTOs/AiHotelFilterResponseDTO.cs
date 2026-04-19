using Domain.Entities;

namespace Domain.DTOs
{
    public class AiHotelFilterResponseDTO
    {
        public HotelFilterRequestDTO AppliedFilters { get; set; } = new();
        public List<HotelEntity> Hotels { get; set; } = [];
        public string AiResponse { get; set; } = string.Empty;
    }
}
