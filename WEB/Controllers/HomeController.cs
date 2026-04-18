using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Domain.Entities;
using WEB.Helpers;
using WEB.Models;

namespace WEB.Controllers
{
    public class HomeController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            List<PopularStayViewModel> popularStays = [];

            try
            {
                List<HotelEntity>? hotels = await Utils.CallApiAsync<List<HotelEntity>>(
                    _configuration["DefaultApiUrl"] + "hotel/all",
                    HttpMethod.Get,
                    cancellationToken: cancellationToken);

                popularStays = (hotels ?? [])
                    .Where(h => h.Approved && h.Status == 1)
                    .OrderByDescending(h => h.UpdatedAt ?? h.CreatedAt ?? DateTime.MinValue)
                    .ThenBy(h => h.Name)
                    .Take(3)
                    .Select((h, index) => new PopularStayViewModel
                    {
                        Name = h.Name,
                        Location = $"{h.City}, {h.Country}",
                        Description = $"{h.Street}, {h.PostalCode}",
                        ImageUrl = PopularStayViewModel.GetDefaultImageByIndex(index)
                    })
                    .ToList();
            }
            catch (HttpRequestException)
            {
                popularStays = [];
            }

            HomeIndexViewModel model = new()
            {
                PopularStays = popularStays
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
