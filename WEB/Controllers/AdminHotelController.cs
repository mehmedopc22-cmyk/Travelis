using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;
using WEB.Models;

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

        [Authorize(Roles = "GlobalAdmin")]
        public async Task<IActionResult> Assignments()
        {
            var hotels = await Utils.CallApiAsync<List<HotelEntity>>(
                _configuration["DefaultApiUrl"] + "admin/hotels/approved",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            var hotelAdmins = await Utils.CallApiAsync<List<UserEntity>>(
                _configuration["DefaultApiUrl"] + "admin/hotels/hotel-admins",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            var assignments = await Utils.CallApiAsync<List<HotelAdminAssignmentDTO>>(
                _configuration["DefaultApiUrl"] + "admin/hotels/admin-assignments",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            return View(new AdminHotelAssignmentsViewModel
            {
                Hotels = hotels ?? new List<HotelEntity>(),
                HotelAdmins = hotelAdmins ?? new List<UserEntity>(),
                Assignments = assignments ?? new List<HotelAdminAssignmentDTO>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid hotelId)
        {
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/hotels/{hotelId}/approve",
                HttpMethod.Patch,
                currentHttpContext: HttpContext);

            return User.IsInRole("GlobalAdmin")
                ? RedirectToAction(nameof(Assignments))
                : RedirectToAction(nameof(Pending));
        }

        [Authorize(Roles = "GlobalAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignAdmin(Guid hotelId, Guid userId)
        {
            try
            {
                await Utils.CallApiAsync<object>(
                    _configuration["DefaultApiUrl"] + $"admin/hotels/{hotelId}/admins",
                    HttpMethod.Post,
                    currentHttpContext: HttpContext,
                    body: new AssignHotelAdminRequestDTO
                    {
                        UserId = userId
                    });

                TempData["HotelAdminAssignmentMessage"] = "Администраторът беше назначен към хотела.";
            }
            catch (HttpRequestException ex)
            {
                TempData["HotelAdminAssignmentError"] = ex.Message;
            }

            return RedirectToAction(nameof(Assignments));
        }

        [Authorize(Roles = "GlobalAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(Guid hotelId, Guid userId)
        {
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/hotels/{hotelId}/admins/{userId}",
                HttpMethod.Delete,
                currentHttpContext: HttpContext);

            TempData["HotelAdminAssignmentMessage"] = "Администраторът беше премахнат от хотела.";
            return RedirectToAction(nameof(Assignments));
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
