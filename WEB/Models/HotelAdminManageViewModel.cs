using Domain.Entities;

namespace WEB.Models
{
    public class HotelAdminManageViewModel
    {
        public HotelEntity Hotel { get; set; } = new();
        public List<ImageEntity> Images { get; set; } = [];
        public List<RentalCarEntity> RentalCars { get; set; } = [];
        public Dictionary<Guid, List<ImageEntity>> RentalCarImages { get; set; } = [];
    }
}
