using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
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
                HotelEntity hotel = _hotelDAO.SelectById(HotelId);

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
    }
}
