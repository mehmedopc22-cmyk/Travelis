using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class HotelController(IHotelDAO hotelDAO) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;

        [HttpGet("getAllHotels")]
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

        [HttpPost("getHotelById")]
        public ActionResult<HotelEntity> GetHotelById(Guid HotelId)
        {

            try
            {
                HotelEntity? hotel = _hotelDAO.SelectById(HotelId);

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

        [HttpPost("insertHotel")]
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

        [HttpPut("updateHotel")]
        public ActionResult updateHotel([FromBody] HotelEntity hotel)
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
    }
}
