using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IHotelReservationDAO : IBaseDAO<HotelReservationResponseDTO>
    {
        HotelReservationResponseDTO InsertHotelReservation(HotelReservationCreationDTO creationData);
    }
}
