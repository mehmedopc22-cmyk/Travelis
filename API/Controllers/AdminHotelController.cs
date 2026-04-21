using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("admin/hotels")]
    [Authorize(Roles = "GlobalAdmin")]
    public class AdminHotelController(
        IHotelDAO hotelDAO,
        IUserDAO userDAO,
        IRoleDAO roleDAO,
        IHotelPersonalDAO hotelPersonalDAO) : ControllerBase
    {
        private readonly IHotelDAO _hotelDAO = hotelDAO;
        private readonly IUserDAO _userDAO = userDAO;
        private readonly IRoleDAO _roleDAO = roleDAO;
        private readonly IHotelPersonalDAO _hotelPersonalDAO = hotelPersonalDAO;

        [HttpGet("pending")]
        public ActionResult<IEnumerable<HotelEntity>> GetPendingHotels()
        {
            var hotels = _hotelDAO.SelectAll().Where(h => !h.Approved);
            return Ok(hotels);
        }

        [HttpGet("approved")]
        public ActionResult<IEnumerable<HotelEntity>> GetApprovedHotels()
        {
            var hotels = _hotelDAO.SelectAll().Where(h => h.Approved && h.Status == 1);
            return Ok(hotels);
        }

        [HttpGet("hotel-admins")]
        public ActionResult<IEnumerable<UserEntity>> GetHotelAdmins()
        {
            RoleEntity? hotelAdminRole = _roleDAO.SelectAll()
                .FirstOrDefault(IsHotelAdminRole);

            if (hotelAdminRole == null)
            {
                return Ok(Enumerable.Empty<UserEntity>());
            }

            return Ok(_userDAO.SelectByRoleId(hotelAdminRole.Id));
        }

        [HttpGet("admin-assignments")]
        public ActionResult<IEnumerable<HotelAdminAssignmentDTO>> GetHotelAdminAssignments()
        {
            List<HotelPersonalEntity> assignments = _hotelPersonalDAO.SelectAll().ToList();
            Dictionary<Guid, UserEntity> usersById = _userDAO.SelectAll().ToDictionary(user => user.Id);

            return Ok(assignments.Select(assignment =>
            {
                usersById.TryGetValue(assignment.UserId, out UserEntity? user);

                return new HotelAdminAssignmentDTO
                {
                    HotelId = assignment.HotelId,
                    UserId = assignment.UserId,
                    FirstName = user?.FirstName ?? string.Empty,
                    LastName = user?.LastName ?? string.Empty,
                    Email = user?.Email ?? string.Empty
                };
            }));
        }

        [HttpPatch("{hotelId:guid}/approve")]
        public IActionResult ApproveHotel(Guid hotelId)
        {
            bool success = _hotelDAO.UpdateHotelStatusTrue(hotelId);
            return success ? Ok() : BadRequest();
        }

        [HttpPost("{hotelId:guid}/admins")]
        public IActionResult AssignHotelAdmin(Guid hotelId, AssignHotelAdminRequestDTO request)
        {
            HotelEntity? hotel = _hotelDAO.SelectById(hotelId);

            if (hotel == null)
            {
                return NotFound("Hotel was not found.");
            }

            if (!hotel.Approved || hotel.Status != 1)
            {
                return BadRequest("Hotel must be approved before assigning hotel admins.");
            }

            UserEntity? user = _userDAO.SelectById(request.UserId);

            if (user == null)
            {
                return NotFound("User was not found.");
            }

            RoleEntity? userRole = _roleDAO.SelectAll().FirstOrDefault(role => role.Id == user.RoleId);

            if (userRole == null || !IsHotelAdminRole(userRole))
            {
                return BadRequest($"Only users with the HotelAdmin role can be assigned to a hotel. Current role: {userRole?.Name ?? "none"}.");
            }

            return _hotelPersonalDAO.Assign(hotelId, request.UserId)
                ? Ok()
                : BadRequest("Could not save the hotel admin assignment in HotelPersonal.");
        }

        [HttpDelete("{hotelId:guid}/admins/{userId:guid}")]
        public IActionResult RemoveHotelAdmin(Guid hotelId, Guid userId)
        {
            return _hotelPersonalDAO.Remove(hotelId, userId) ? NoContent() : NotFound();
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

        private static bool IsHotelAdminRole(RoleEntity role)
        {
            string normalizedName = NormalizeRoleName(role.Name);
            string normalizedDescription = NormalizeRoleName(role.Description);

            return normalizedName == "hoteladmin"
                || normalizedDescription == "hoteladmin"
                || (normalizedName.Contains("hotel") && normalizedName.Contains("admin"));
        }

        private static string NormalizeRoleName(string? value)
        {
            return new string((value ?? string.Empty)
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }
    }
}
