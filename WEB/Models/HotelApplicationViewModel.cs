using System.ComponentModel.DataAnnotations;

namespace WEB.Models
{
    public class HotelApplicationViewModel
    {
        [Required]
        [Display(Name = "Име на обекта")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Държава")]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Град")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Улица")]
        public string Street { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Пощенски код")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = string.Empty;
    }
}