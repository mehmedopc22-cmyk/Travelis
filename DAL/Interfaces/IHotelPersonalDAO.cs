using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IHotelPersonalDAO
    {
        IEnumerable<HotelPersonalEntity> SelectAll();
        IEnumerable<HotelPersonalEntity> SelectByHotelId(Guid hotelId);
        IEnumerable<HotelPersonalEntity> SelectByUserId(Guid userId);
        bool Exists(Guid hotelId, Guid userId);
        bool Assign(Guid hotelId, Guid userId);
        bool Remove(Guid hotelId, Guid userId);
    }
}
