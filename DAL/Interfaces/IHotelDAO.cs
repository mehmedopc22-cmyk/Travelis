using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IHotelDAO : IBaseDAO<HotelEntity>
    {
        IEnumerable<HotelEntity> SelectFiltered(HotelFilterRequestDTO filters);
        IEnumerable<HotelEntity> SelectByCountryName(string countryName);
        IEnumerable<HotelEntity> SelectByEmail(string hotelEmail);
        int CheckHotelStatus(Guid hotelId);
        bool UpdateHotelStatusTrue(Guid hotelId);
        bool UpdateHotelStatusRejected(Guid hotelId);
    }
}
