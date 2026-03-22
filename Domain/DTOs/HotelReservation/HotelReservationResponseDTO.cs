using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class HotelReservationResponseDTO
    {
        public Guid Id { get; set; }
        public HotelResponseDTO Hotel { get; set; }
        public UserResponseDTO User { get; set; }
        public HotelRoomResponseDTO Room { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
