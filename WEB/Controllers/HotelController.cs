using Domain.Entities;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;
using WEB.Models;

namespace WEB.Controllers
{
    public class HotelController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        public async Task<IActionResult> IndexAsync()
        {
            var hotelDtos = await Utils.CallApiAsync<List<HotelEntity>>(
                   _configuration["DefaultApiUrl"] + "getAllHotels",
                   HttpMethod.Get
               );

            List<HotelCardViewModel> model = (hotelDtos ?? new List<HotelEntity>())
                .Select(h => new HotelCardViewModel
                {
                    Id = h.HotelId,
                    Name = h.Name,
                    Country = h.Country,
                    City = h.City,
                    Street = h.Street,
                    PostalCode = h.PostalCode,
                    PhoneNumber = h.PhoneNumber,
                    Email = h.Email,
                    Approved = h.Approved,
                    Status = h.Status,
                    CreatedAt = h.CreatedAt,
                    UpdatedAt = h.UpdatedAt
                })
                .ToList();

            return View(model);
        }

    }
}
