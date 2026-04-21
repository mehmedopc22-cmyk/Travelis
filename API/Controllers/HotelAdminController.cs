using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("hotel-admin")]
    [Authorize(Roles = "HotelAdmin")]
    public class HotelAdminController(
        IHotelDAO hotelDAO,
        IHotelPersonalDAO hotelPersonalDAO,
        IImageDAO imageDAO) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;
        private readonly IHotelPersonalDAO _hotelPersonalDAO = hotelPersonalDAO;
        private readonly IImageDAO _imageDAO = imageDAO;

        [HttpGet("hotels")]
        public ActionResult<IEnumerable<HotelEntity>> GetAssignedHotels()
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            HashSet<Guid> hotelIds = _hotelPersonalDAO
                .SelectByUserId(userId)
                .Select(assignment => assignment.HotelId)
                .ToHashSet();

            IEnumerable<HotelEntity> hotels = _hotelDAO
                .SelectAll()
                .Where(hotel => hotelIds.Contains(hotel.Id));

            return Ok(hotels);
        }

        [HttpGet("hotels/{hotelId:guid}")]
        public ActionResult<HotelEntity> GetAssignedHotel(Guid hotelId)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            HotelEntity? hotel = _hotelDAO.SelectById(hotelId);
            return hotel == null ? NotFound() : Ok(hotel);
        }

        [HttpPut("hotels/{hotelId:guid}")]
        public IActionResult UpdateAssignedHotel(Guid hotelId, HotelEntity request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            HotelEntity? hotel = _hotelDAO.SelectById(hotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Name = request.Name;
            hotel.Country = request.Country;
            hotel.City = request.City;
            hotel.Street = request.Street;
            hotel.PostalCode = request.PostalCode;
            hotel.PhoneNumber = request.PhoneNumber;
            hotel.Email = request.Email;

            return _hotelDAO.Update(hotel) ? Ok() : BadRequest();
        }

        [HttpGet("hotels/{hotelId:guid}/images")]
        public ActionResult<IEnumerable<ImageEntity>> GetHotelImages(Guid hotelId)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            return Ok(_imageDAO.SelectByHotelId(hotelId));
        }

        [HttpPost("hotels/{hotelId:guid}/images")]
        public ActionResult<ImageEntity> AddHotelImage(Guid hotelId, ImageUploadRequestDTO request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(request.ImageName))
            {
                return BadRequest("Image name is required.");
            }

            ImageEntity image = _imageDAO.Insert(request.ImageName.Trim());

            if (!_imageDAO.LinkHotelImage(hotelId, image.Id))
            {
                return BadRequest();
            }

            return Ok(image);
        }

        [HttpPost("hotels/{hotelId:guid}/images/batch")]
        public ActionResult<IEnumerable<ImageEntity>> AddHotelImages(Guid hotelId, ImageUploadBatchRequestDTO request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            List<string> imageNames = (request.ImageNames ?? [])
                .Where(imageName => !string.IsNullOrWhiteSpace(imageName))
                .Select(imageName => imageName.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (imageNames.Count == 0)
            {
                return BadRequest("At least one image name is required.");
            }

            List<ImageEntity> images = [];

            foreach (string imageName in imageNames)
            {
                ImageEntity image = _imageDAO.Insert(imageName);

                if (!_imageDAO.LinkHotelImage(hotelId, image.Id))
                {
                    return BadRequest($"Could not link image {imageName} to the hotel.");
                }

                images.Add(image);
            }

            return Ok(images);
        }

        private bool IsAssignedToHotel(Guid hotelId)
        {
            return TryGetCurrentUserId(out Guid userId) && _hotelPersonalDAO.Exists(hotelId, userId);
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdValue, out userId);
        }
    }
}
