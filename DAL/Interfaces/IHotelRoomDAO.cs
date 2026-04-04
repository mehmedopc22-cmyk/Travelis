using Domain.DTOs;
using Domain.DTOs.HotelRoom;

namespace DAL.Interfaces
{
    public interface IHotelRoomDAO : IBaseDAO<HotelRoomResponseDTO>
    {
        bool Create(HotelRoomCreationDTO hotelRoomCreationDTO);
        IEnumerable<HotelRoomResponseDTO> SelectByHotelId(Guid id);
        bool UpdateHotelRoom(Guid hotelRoomId, HotelRoomPatchDTO patchData);
    }
}
