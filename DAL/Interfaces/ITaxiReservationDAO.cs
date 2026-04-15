using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces;

public interface ITaxiReservationDAO : IBaseDAO<TaxiReservationEntity>
{
    IEnumerable<TaxiReservationEntity> SelectByUserId(Guid userId);
    IEnumerable<TaxiReservationResponseDTO> SelectAllWithUserName();
    IEnumerable<TaxiReservationResponseDTO> SelectByUserIdWithUserName(Guid userId);
    TaxiReservationCreationDTO InsertSimple(TaxiReservationCreationDTO taxiReservation);
    bool UpdateSimple(TaxiReservationUpdateDTO taxiReservation);

}