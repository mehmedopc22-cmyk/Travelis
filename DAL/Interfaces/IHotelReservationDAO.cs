using Domain.DTOs;

namespace DAL.Interfaces
{
    public interface IHotelReservationDAO : IBaseDAO<HotelReservationResponseDTO>
    {
        HotelReservationResponseDTO InsertHotelReservation(HotelReservationCreationDTO creationData);
        IEnumerable<HotelReservationResponseDTO> SelectByUserId(Guid id);
        bool UpdateHotelReservation(Guid reservationId, HotelReservationPatchDTO newReservationData);
    }
}
