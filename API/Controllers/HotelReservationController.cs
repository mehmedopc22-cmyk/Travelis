using DAL.DAOs;
using DAL.Interfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class HotelReservationController(IHotelReservationDAO hotelREservationDAO) : ControllerBase
    {
        private readonly IHotelReservationDAO _hotelReservationDAO = hotelREservationDAO;

        [HttpGet("/hotel/reservations")]
        public ActionResult<IEnumerable<HotelReservationResponseDTO>> GetHotelReservations()
        {
            try
            {
                var hotelReservations = _hotelReservationDAO.SelectAll();
                if (hotelReservations == null)
                {
                    return NotFound();
                }

                return Ok(hotelReservations);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("/hotel/reservations/{id}")]
        public ActionResult<HotelReservationResponseDTO> GetHotelReservationById(Guid id)
        {
            try
            {
                var hotelReservation = _hotelReservationDAO.SelectById(id);
                return hotelReservation != null ? Ok(hotelReservation) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("/hotel/user/reservations")]
        public ActionResult<IEnumerable<HotelReservationResponseDTO>> GetHotelReservationByUserId(Guid userId)
        {
            try
            {
                var hotelReservation = _hotelReservationDAO.SelectByUserId(id);
                return hotelReservation != null ? Ok(hotelReservation) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("/hotel/reservations")]
        public ActionResult<HotelReservationResponseDTO> CreateHotelReservation([FromBody] HotelReservationCreationDTO creationData)
        {
            try
            {
                var createdHotelReservation = _hotelReservationDAO.InsertHotelReservation(creationData);
                return createdHotelReservation != null ? Ok(createdHotelReservation) : BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("/hotel/reservations/{id}")]
        public ActionResult DeleteHotelReservation(Guid id)
        {
            try
            {
                bool deleted = _hotelReservationDAO.Delete(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
