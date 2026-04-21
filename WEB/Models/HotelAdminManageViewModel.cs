using Domain.Entities;

namespace WEB.Models
{
    public class HotelAdminManageViewModel
    {
        public HotelEntity Hotel { get; set; } = new();
        public List<ImageEntity> Images { get; set; } = [];
    }
}
