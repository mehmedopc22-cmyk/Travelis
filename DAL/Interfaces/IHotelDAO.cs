using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IHotelDAO : IBaseDAO<HotelEntity>
    {
        public IEnumerable<HotelEntity> SelectByCountryName(string countryName);
        public IEnumerable<HotelEntity> SelectByEmail(string hotelEmail);
        public int CheckHotelStatus(Guid hotelId);
        public bool UpdateHotelStatusTrue(Guid hotelId);

    }
}
