using System;
using System.ComponentModel.DataAnnotations;

namespace WEB.Models
{
    public class BookingViewModel
    {
        // --- Секция 1 & 2: Локации и Дати ---
        [Required]
        public string PickUpLocation { get; set; }

        public string DropOffLocation { get; set; }

        [Required]
        public DateTime PickUpDate { get; set; }

        [Required]
        public DateTime DropOffDate { get; set; }

        // --- Новите полета, които липсваха (КРИТИЧНИ) ---
        public string DriverAge { get; set; }

        public string Residence { get; set; }

        // --- Спецификации на колата ---
        public string CarCategory { get; set; }

        public string Transmission { get; set; }

        // --- Секция 3: Екстри (Checkboxes) ---
        public bool HasChildSeat { get; set; }
        public bool HasGPS { get; set; }
        public bool HasAdditionalDriver { get; set; }
        public bool HasWinterPack { get; set; }
        public bool HasCrossBorder { get; set; }
        public bool HasWifi { get; set; }

        // --- Секция 4: Застраховка и Лични данни ---
        public string InsPackage { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        public string DriverName { get; set; }

        [Required(ErrorMessage = "Телефонът е задължителен")]
        public string DriverPhone { get; set; }

        [Required(ErrorMessage = "Номерът на книжката е задължителен")]
        public string DriverLicense { get; set; }

        public string DriverEmail { get; set; }
    }
}
