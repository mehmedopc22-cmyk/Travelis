using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;

namespace WEB.Controllers
{
    [Authorize(Policy = "Admins")]
    public class AdminHotelController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IActionResult> Pending()
        {
            var hotels = await Utils.CallApiAsync<List<HotelEntity>>(
                _configuration["DefaultApiUrl"] + "admin/hotels/pending",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            return View(hotels ?? new List<HotelEntity>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid hotelId)
        {
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/hotels/{hotelId}/approve",
                HttpMethod.Patch,
                currentHttpContext: HttpContext);

            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid hotelId)
        {
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/hotels/{hotelId}/reject",
                HttpMethod.Patch,
                currentHttpContext: HttpContext);

            return RedirectToAction(nameof(Pending));
        }
    }
}