using DAL.DAOs;
using DAL.Interfaces;
using Domain.DTOs.RentalCarReservation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("rental-car-reservations")]
    [ApiController]
    public class RentalCarReservationController(IRentalCarReservationDAO rentalCarReservationDAO) : ControllerBase
    {
        private readonly IRentalCarReservationDAO _rentalCarReservationDAO = rentalCarReservationDAO;

        [HttpGet]
        public ActionResult<IEnumerable<RentalCarReservationDAO>> GetRentalCarReservations([FromQuery] Guid? userID)
        {
            try
            {
                if(userID == null)
                {
                    var hotelReservations = _rentalCarReservationDAO.SelectAll();
                    return Ok(hotelReservations);
                }

                var hotelReservation = _rentalCarReservationDAO.SelectUserCarReservations(userID.Value);
                return hotelReservation != null ? Ok(hotelReservation) : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<RentalCarReservationDAO>> GetRentalCarReservationsById(Guid id)
        {
            try
            {
                var hotelReservation = _rentalCarReservationDAO.SelectById(id);
                return hotelReservation != null ? Ok(hotelReservation) : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public ActionResult<RentalCarReservationDAO> CreateRentalCarReservation([FromBody] RentalCarReservationCreationDTO rentalCarInformation)
        {
            try
            {
                var createdHotelReservation = _rentalCarReservationDAO.InsertRentalCarReservation(rentalCarInformation);
                if (createdHotelReservation == null)
                    return BadRequest("Failed to create rental car reservation. Please check the provided data.");

                return Ok(createdHotelReservation);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPatch("{id}")]
        public ActionResult UpdateRentalCarReservation(Guid id, [FromBody] RentalCarReservationPatchDTO patchData)
        {
            try
            {
                bool updated = _rentalCarReservationDAO.UpdateRentalCarReservation(id, patchData);
                return updated ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRentalCarReservation(Guid id)
        {
            try
            {
                bool deleted = _rentalCarReservationDAO.Delete(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
