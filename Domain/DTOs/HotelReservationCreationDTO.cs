using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class HotelReservationCreationDTO
    {
        public Guid HotelId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
