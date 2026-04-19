using DAL.DAOs;
using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("hotel")]
    public class HotelController(IHotelDAO hotelDAO, IHotelRoomDAO hotelRoomDAO) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;
        private readonly IHotelRoomDAO _hotelRoomDAO = hotelRoomDAO;

        [HttpGet("all")]
        public ActionResult<IEnumerable<HotelEntity>> GetHotels() {

            try
            {
                IEnumerable<HotelEntity> hotels = _hotelDAO.SelectAll();

                if (hotels == null)
                {
                    return NotFound();
                }

                return Ok(hotels);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<HotelEntity>> GetFilteredHotels([FromQuery] HotelFilterRequestDTO filters)
        {
            try
            {
                IEnumerable<HotelEntity> hotels = _hotelDAO.SelectFiltered(filters);

                return Ok(hotels);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<HotelEntity> GetHotelById(Guid Id)
        {

            try
            {
                HotelEntity? hotel = _hotelDAO.SelectById(Id);

                if (hotel == null)
                {
                    return NotFound();
                }


                return Ok(hotel);
            }

            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("country/{countryName}")]
        public ActionResult<IEnumerable<HotelEntity>> GetHotelByConutry(string countryName)
        {

            try
            {
                IEnumerable<HotelEntity>? hotels = _hotelDAO.SelectByCountryName(countryName);

                if (hotels == null || !hotels.Any())
                {
                    return NotFound();
                }

                return Ok(hotels);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("contact/{email}")]
        public ActionResult<IEnumerable<HotelEntity>> GetHotelByEmail(string email)
        {

            try
            {
                IEnumerable<HotelEntity>? hotels = _hotelDAO.SelectByEmail(email);

                if (hotels == null || !hotels.Any())
                {
                    return NotFound();
                }

                return Ok(hotels);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("status/{hotelId}")]
        public ActionResult<int> CheckHotelStatus(Guid hotelId)
        {

            try
            {
                int status = _hotelDAO.CheckHotelStatus(hotelId);

                return Ok(status);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("{HotelEntity}")]
        public ActionResult InsertHotel([FromBody] HotelEntity hotel)
        {
            try
            {
                HotelEntity insertedHotel = _hotelDAO.Insert(hotel);

                if (insertedHotel == null) {
                    return StatusCode(500, "Insertion failed");
                }

                return Ok(hotel);
            }

            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("application/{HotelEntity}")]
        public ActionResult InsertHotelForApproval([FromBody] HotelEntity hotel)
        {
            try
            {
                hotel.Approved = false;

                HotelEntity insertedHotel = _hotelDAO.Insert(hotel);

                if (insertedHotel == null)
                {
                    return StatusCode(500, "Insertion failed");
                }

                return Ok(hotel);
            }

            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPatch("approve/{hotelId}")]
        public ActionResult ApproveHotel(Guid hotelId) {
            try
            {
                if (_hotelDAO.UpdateHotelStatusTrue(hotelId)) {
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();

                throw;
            }
        }

        [HttpPut("{HotelEntity}")]
        public ActionResult UpdateHotel([FromBody] HotelEntity hotel)
        {
            try
            {
                if (_hotelDAO.Update(hotel)) {

                    return Ok();
                }

                return BadRequest();
            }

            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteHotel(Guid hotelId) {
            try
            {
                if (_hotelDAO.Delete(hotelId)) {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{hotelId}/rooms")]
        public ActionResult<HotelRoomResponseDTO> GetHotelByHotelId(Guid hotelId)
        {
            try
            {
                return Ok(_hotelRoomDAO.SelectByHotelId(hotelId));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
