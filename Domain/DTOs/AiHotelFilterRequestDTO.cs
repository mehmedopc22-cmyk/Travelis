using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class AiHotelFilterRequestDTO
    {
        public string? SelectedLocation { get; set; }

        [Required]
        public string Prompt { get; set; } = string.Empty;
    }
}
