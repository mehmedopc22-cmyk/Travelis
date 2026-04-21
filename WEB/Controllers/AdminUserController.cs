using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEB.Helpers;
using WEB.Models;

namespace WEB.Controllers
{
    [Authorize(Policy = "Admins")]
    public class AdminUserController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<IActionResult> Index()
        {
            var users = await Utils.CallApiAsync<List<UserEntity>>(
                _configuration["DefaultApiUrl"] + "admin/users",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            var roles = await Utils.CallApiAsync<List<RoleEntity>>(
                _configuration["DefaultApiUrl"] + "admin/users/roles",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            AdminUsersIndexViewModel model = new()
            {
                Users = users ?? new List<UserEntity>(),
                Roles = roles ?? new List<RoleEntity>()
            };

            return View(model);
        }

        public async Task<IActionResult> Roles()
        {
            var roles = await Utils.CallApiAsync<List<RoleEntity>>(
                _configuration["DefaultApiUrl"] + "admin/users/roles",
                HttpMethod.Get,
                currentHttpContext: HttpContext);

            return View(roles ?? new List<RoleEntity>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(AdminUpdateUserDTO dto)
        {
            Guid userId = dto.Id;
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/users/{userId}",
                HttpMethod.Put,
                HttpContext,
                dto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(Guid userId, Guid roleId)
        {
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/users/{userId}/role",
                HttpMethod.Patch,
                currentHttpContext: HttpContext,
                body: new UpdateUserRoleDTO
                {
                    RoleId = roleId
                });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid userId, byte status)
        {
            await Utils.CallApiAsync<object>(
                _configuration["DefaultApiUrl"] + $"admin/users/{userId}/status/{status}",
                HttpMethod.Patch,
                currentHttpContext: HttpContext);

            return RedirectToAction(nameof(Index));
        }
    }
}