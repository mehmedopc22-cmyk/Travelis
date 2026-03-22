using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class HotelResponseDTO
    {
        public Guid HotelId { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
