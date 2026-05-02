using DAL.Interfaces;
using Domain.DTOs;
using Domain.DTOs.RentalCar;
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
        IImageDAO imageDAO,
        IRentalCarDAO rentalCarDAO) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;
        private readonly IHotelPersonalDAO _hotelPersonalDAO = hotelPersonalDAO;
        private readonly IImageDAO _imageDAO = imageDAO;
        private readonly IRentalCarDAO _rentalCarDAO = rentalCarDAO;

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

        [HttpGet("hotels/{hotelId:guid}/rental-cars")]
        public ActionResult<IEnumerable<RentalCarEntity>> GetHotelRentalCars(Guid hotelId)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            return Ok(_rentalCarDAO.SelectByHotelId(hotelId));
        }

        [HttpPost("hotels/{hotelId:guid}/rental-cars")]
        public ActionResult<RentalCarEntity> AddHotelRentalCar(Guid hotelId, RentalCarCreationDTO request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            string brand = request.Brand?.Trim() ?? string.Empty;
            string model = request.Model?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(brand))
            {
                return BadRequest("Brand is required.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                return BadRequest("Model is required.");
            }

            if (request.Kilometers < 0)
            {
                return BadRequest("Kilometers cannot be negative.");
            }

            RentalCarEntity? rentalCar = _rentalCarDAO.CreateForHotel(
                hotelId,
                new RentalCarCreationDTO
                {
                    Brand = brand,
                    Model = model,
                    Kilometers = request.Kilometers
                });

            return rentalCar == null ? BadRequest("Could not add rental car to the hotel.") : Ok(rentalCar);
        }

        [HttpPut("hotels/{hotelId:guid}/rental-cars/{rentalCarId:guid}")]
        public IActionResult UpdateHotelRentalCar(Guid hotelId, Guid rentalCarId, RentalCarCreationDTO request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            RentalCarEntity? rentalCar = SelectHotelRentalCar(hotelId, rentalCarId);

            if (rentalCar == null)
            {
                return NotFound();
            }

            string brand = request.Brand?.Trim() ?? string.Empty;
            string model = request.Model?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(brand))
            {
                return BadRequest("Brand is required.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                return BadRequest("Model is required.");
            }

            if (request.Kilometers < 0)
            {
                return BadRequest("Kilometers cannot be negative.");
            }

            rentalCar.Brand = brand;
            rentalCar.Model = model;
            rentalCar.Kilometers = request.Kilometers;

            return _rentalCarDAO.Update(rentalCar) ? Ok(rentalCar) : BadRequest("Could not update rental car.");
        }

        [HttpDelete("hotels/{hotelId:guid}/rental-cars/{rentalCarId:guid}")]
        public IActionResult RemoveHotelRentalCar(Guid hotelId, Guid rentalCarId)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            if (SelectHotelRentalCar(hotelId, rentalCarId) == null)
            {
                return NotFound();
            }

            return _rentalCarDAO.RemoveFromHotel(hotelId, rentalCarId)
                ? NoContent()
                : BadRequest("Could not remove rental car from the hotel.");
        }

        [HttpGet("hotels/{hotelId:guid}/rental-cars/{rentalCarId:guid}/images")]
        public ActionResult<IEnumerable<ImageEntity>> GetRentalCarImages(Guid hotelId, Guid rentalCarId)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            if (SelectHotelRentalCar(hotelId, rentalCarId) == null)
            {
                return NotFound();
            }

            return Ok(_imageDAO.SelectByRentalCarId(rentalCarId));
        }

        [HttpPost("hotels/{hotelId:guid}/rental-cars/{rentalCarId:guid}/images")]
        public ActionResult<ImageEntity> AddRentalCarImage(Guid hotelId, Guid rentalCarId, ImageUploadRequestDTO request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            if (SelectHotelRentalCar(hotelId, rentalCarId) == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.ImageName))
            {
                return BadRequest("Image name is required.");
            }

            ImageEntity image = _imageDAO.Insert(request.ImageName.Trim());

            if (!_imageDAO.LinkRentalCarImage(rentalCarId, image.Id))
            {
                return BadRequest();
            }

            return Ok(image);
        }

        [HttpPost("hotels/{hotelId:guid}/rental-cars/{rentalCarId:guid}/images/batch")]
        public ActionResult<IEnumerable<ImageEntity>> AddRentalCarImages(Guid hotelId, Guid rentalCarId, ImageUploadBatchRequestDTO request)
        {
            if (!IsAssignedToHotel(hotelId))
            {
                return Forbid();
            }

            if (SelectHotelRentalCar(hotelId, rentalCarId) == null)
            {
                return NotFound();
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

                if (!_imageDAO.LinkRentalCarImage(rentalCarId, image.Id))
                {
                    return BadRequest($"Could not link image {imageName} to the rental car.");
                }

                images.Add(image);
            }

            return Ok(images);
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

        private RentalCarEntity? SelectHotelRentalCar(Guid hotelId, Guid rentalCarId)
        {
            return _rentalCarDAO
                .SelectByHotelId(hotelId)
                .FirstOrDefault(car => car.Id == rentalCarId);
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdValue, out userId);
        }
    }
}
