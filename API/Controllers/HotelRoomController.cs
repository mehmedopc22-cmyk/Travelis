using DAL.Interfaces;
using Domain.DTOs;
using Domain.DTOs.HotelRoom;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("hotel-rooms")]
    public class HotelRoomController(IHotelRoomDAO hotelRoomDAO) : ControllerBase
    {
        private readonly IHotelRoomDAO _hotelRoomDAO = hotelRoomDAO;

        [HttpGet]
        public ActionResult<IEnumerable<HotelRoomResponseDTO>> GetHotelRooms()
        {
            try
            {
                return Ok(_hotelRoomDAO.SelectAll());
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<HotelRoomResponseDTO> GetHotelRoomById(Guid id)
        {
            try
            {
                return Ok(_hotelRoomDAO.SelectById(id));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public ActionResult CreateHotelRoom([FromBody] HotelRoomCreationDTO hotelRoom)
        {
            try
            {
                bool created = _hotelRoomDAO.Create(hotelRoom);
                return created ? NoContent() : BadRequest("Failed. Please check the provided data."); ;
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteHotelRoom(Guid id)
        {
            try
            {
                bool deleted = _hotelRoomDAO.Delete(id);
                return deleted ? NoContent() : BadRequest();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPatch("{id}")]
        public ActionResult UpdateHotelRoom(Guid id, [FromBody] HotelRoomPatchDTO patchDTO)
        {
            try
            {
                bool updated = _hotelRoomDAO.UpdateHotelRoom(id, patchDTO);
                return updated ? NoContent() : BadRequest();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
