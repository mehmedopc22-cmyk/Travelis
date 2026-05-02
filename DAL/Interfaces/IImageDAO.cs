using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IImageDAO
    {
        ImageEntity Insert(string imageName);
        ImageEntity? SelectById(Guid imageId);
        IEnumerable<ImageEntity> SelectByUserId(Guid userId);
        IEnumerable<ImageEntity> SelectByHotelId(Guid hotelId);
        IEnumerable<ImageEntity> SelectByRentalCarId(Guid rentalCarId);
        bool LinkUserImage(Guid userId, Guid imageId);
        bool LinkHotelImage(Guid hotelId, Guid imageId);
        bool LinkRentalCarImage(Guid rentalCarId, Guid imageId);
    }
}
