using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WEB.Helpers;
using WEB.Models;

namespace WEB.Controllers
{
    public class HotelController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IActionResult> IndexAsync([FromQuery] HotelFilterViewModel filters)
        {
            string apiBaseUrl = (_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/');
            string queryString = BuildHotelFilterQueryString(filters);

            var hotelDtos = await Utils.CallApiAsync<List<HotelEntity>>(
                $"{apiBaseUrl}/hotel/filter{queryString}",
                HttpMethod.Get
            );

            List<HotelCardViewModel> model = MapHotelCards(hotelDtos ?? new List<HotelEntity>());

            return View(new HotelIndexViewModel
            {
                Filters = filters,
                Hotels = model
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AiFilter(HotelAiFilterViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(model.SelectedLocation))
            {
                TempData["AiFilterError"] = "Моля, въведи локация, за да може AI филтърът да работи.";
                return RedirectToAction("Index", new
                {
                    Destination = model.SelectedLocation
                });
            }

            if (string.IsNullOrWhiteSpace(model.Prompt))
            {
                TempData["AiFilterError"] = "Моля, опиши какъв хотел търсиш.";
                return RedirectToAction("Index", new
                {
                    Destination = model.SelectedLocation
                });
            }

            AiHotelFilterResponseDTO? response = await Utils.CallApiAsync<AiHotelFilterResponseDTO>(
                $"{(_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/')}/ai/hotels/filter",
                HttpMethod.Post,
                new AiHotelFilterRequestDTO
                {
                    SelectedLocation = model.SelectedLocation,
                    Prompt = model.Prompt
                },
                cancellationToken: cancellationToken);

            ViewData["AiFilterMessage"] = $"AI филтърът беше приложен за {model.SelectedLocation}.";

            return View("Index", new HotelIndexViewModel
            {
                Filters = new HotelFilterViewModel
                {
                    Destination = model.SelectedLocation,
                    SortBy = "recommended"
                },
                Hotels = MapHotelCards(response?.Hotels ?? new List<HotelEntity>()),
                AiResponse = response?.AiResponse
            });
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

        public async Task<IActionResult> Details(Guid id)
        {
            string apiBaseUrl = (_configuration["DefaultApiUrl"] ?? string.Empty).TrimEnd('/');
            
            HotelEntity? hotel = await Utils.CallApiAsync<HotelEntity>(
                $"{apiBaseUrl}/hotel/{id}",
                HttpMethod.Get
            );

            if (hotel == null)
            {
                return NotFound();
            }

            var rooms = await Utils.CallApiAsync<List<HotelRoomResponseDTO>>(
                $"{apiBaseUrl}/hotel/{id}/rooms",
                HttpMethod.Get
            ) ?? new List<HotelRoomResponseDTO>();

            HotelCardViewModel model = new()
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Country = hotel.Country,
                City = hotel.City,
                Street = hotel.Street,
                PostalCode = hotel.PostalCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                Approved = hotel.Approved,
                Status = hotel.Status,
                CreatedAt = hotel.CreatedAt,
                UpdatedAt = hotel.UpdatedAt,
                
                Rooms = rooms.Select(r => new HotelRoomViewModel
                {
                    Id = r.Id,
                    Description = r.Description ?? string.Empty,
                    Price = r.Price,
                    RoomNo = r.RoomNo ?? string.Empty,
                    Floor = r.Floor,
                    BedCount = r.BedCount,
                    Capacity = r.Capacity
                }).ToList()
            };

            return View(model);
        }

        private static string BuildHotelFilterQueryString(HotelFilterViewModel filters)
        {
            Dictionary<string, string?> query = new()
            {
                ["destination"] = filters.Destination,
                ["sortBy"] = filters.SortBy
            };

            if (filters.MinPrice.HasValue)
            {
                query["minPrice"] = filters.MinPrice.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (filters.MaxPrice.HasValue)
            {
                query["maxPrice"] = filters.MaxPrice.Value.ToString(CultureInfo.InvariantCulture);
            }

            string[] parts = query
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value!)}")
                .ToArray();

            return parts.Length == 0 ? string.Empty : $"?{string.Join("&", parts)}";
        }

        private static List<HotelCardViewModel> MapHotelCards(IEnumerable<HotelEntity> hotels)
        {
            return hotels
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
        }
    }
}