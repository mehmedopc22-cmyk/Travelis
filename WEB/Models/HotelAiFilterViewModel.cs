using System.ComponentModel.DataAnnotations;

namespace WEB.Models
{
    public class HotelAiFilterViewModel
    {
        [Required(ErrorMessage = "Моля, въведи локация, за да може AI филтърът да работи.")]
        public string? SelectedLocation { get; set; }

        [Required]
        public string Prompt { get; set; } = string.Empty;
    }
}
