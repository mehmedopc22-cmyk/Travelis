using Domain.DTOs;
using Domain.DTOs.RentalCarReservation;

namespace DAL.Interfaces
{
    public interface IRentalCarReservationDAO : IBaseDAO<RentalCarReservationResponseDTO>
    {
        IEnumerable<RentalCarReservationResponseDTO> SelectUserCarReservations(Guid id);
        RentalCarReservationResponseDTO InsertRentalCarReservation(RentalCarReservationCreationDTO creationData);
        bool UpdateRentalCarReservation(Guid reservationId, RentalCarReservationPatchDTO newReservationData);
    }
}
