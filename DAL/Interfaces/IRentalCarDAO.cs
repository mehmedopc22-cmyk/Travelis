using Domain.DTOs.RentalCar;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IRentalCarDAO : IBaseDAO<RentalCarEntity>
    {
        RentalCarEntity? CreateForHotel(Guid hotelId, RentalCarCreationDTO rentalCar);
        bool RemoveFromHotel(Guid hotelId, Guid rentalCarId);
        IEnumerable<RentalCarEntity> SelectByHotelId(Guid hotelId);
    }
}
