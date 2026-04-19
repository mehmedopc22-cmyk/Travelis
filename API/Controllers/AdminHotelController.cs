using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("admin/hotels")]
    [Authorize(Roles = "GlobalAdmin")]
    public class AdminHotelController(IHotelDAO hotelDAO) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;

        [HttpGet("pending")]
        public ActionResult<IEnumerable<HotelEntity>> GetPendingHotels()
        {
            var hotels = _hotelDAO.SelectAll().Where(h => !h.Approved);
            return Ok(hotels);
        }

        [HttpPatch("{hotelId:guid}/approve")]
        public IActionResult ApproveHotel(Guid hotelId)
        {
            bool success = _hotelDAO.UpdateHotelStatusTrue(hotelId);
            return success ? Ok() : BadRequest();
        }

        [HttpPatch("{hotelId:guid}/reject")]
        public ActionResult RejectHotel(Guid hotelId)
        {
            try
            {
                if (_hotelDAO.UpdateHotelStatusRejected(hotelId))
                {
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
