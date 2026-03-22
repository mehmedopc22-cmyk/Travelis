using DAL.Interfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("hotel-reservations")]
    public class HotelReservationController(IHotelReservationDAO hotelReservationDAO) : ControllerBase
    {
        private readonly IHotelReservationDAO _hotelReservationDAO = hotelReservationDAO;

        [HttpGet("{id}")]
        public ActionResult<HotelReservationResponseDTO> GetHotelReservationById(Guid id)
        {
            try
            {
                var hotelReservation = _hotelReservationDAO.SelectById(id);
                return hotelReservation != null ? Ok(hotelReservation) : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<HotelReservationResponseDTO>> GetHotelReservations([FromQuery] Guid? userId)
        {
            try
            {
                if(userId == null)
                {
                    var hotelReservations = _hotelReservationDAO.SelectAll();
                    if (hotelReservations == null)
                    {
                        return NotFound();
                    }

                    return Ok(hotelReservations);
                }

                var hotelReservation = _hotelReservationDAO.SelectByUserId(userId.Value);
                return hotelReservation != null ? Ok(hotelReservation) : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public ActionResult<HotelReservationResponseDTO> CreateHotelReservation([FromBody] HotelReservationCreationDTO creationData)
        {
            try
            {
                var createdHotelReservation = _hotelReservationDAO.InsertHotelReservation(creationData);
                if(createdHotelReservation == null)
                    return BadRequest("Failed to create hotel reservation. Please check the provided data.");

                return Ok(createdHotelReservation);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPatch("{id}")]
        public ActionResult UpdateHotelReservation(Guid id, [FromBody] HotelReservationPatchDTO patchData)
        {
            try
            {
                bool updated = _hotelReservationDAO.UpdateHotelReservation(id, patchData);
                return updated ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteHotelReservation(Guid id)
        {
            try
            {
                bool deleted = _hotelReservationDAO.Delete(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
