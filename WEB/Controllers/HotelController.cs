using Domain.Entities;
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
                _configuration["DefaultApiUrl"] + "hotel/all",
                HttpMethod.Get
            );

            List<HotelCardViewModel> model = (hotelDtos ?? new List<HotelEntity>())
                .Select(h => new HotelCardViewModel
                {
                    Id = h.Id,
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

        [HttpGet]
        public IActionResult Apply()
        {
            return View(new HotelApplicationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(HotelApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            HotelEntity hotel = new()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Country = model.Country,
                City = model.City,
                Street = model.Street,
                PostalCode = model.PostalCode,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Approved = false,
                Status = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + "hotel/application/HotelEntity",
                HttpMethod.Post,
                body: hotel);

            TempData["SuccessMessage"] = "Заявката беше изпратена успешно и очаква одобрение.";

            return RedirectToAction(nameof(Apply));
        }
    }
}