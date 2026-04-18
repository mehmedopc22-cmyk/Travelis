using API.Helpers;
using DAL.DAOs;
using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("admin/users")]
    [Authorize(Roles = "GlobalAdmin")]
    public class AdminUserController(
        IUserDAO userDAO,
        IRoleDAO roleDAO,
        PasswordHasherService passwordHasher) : ControllerBase
    {
        private readonly IUserDAO _userDAO = userDAO;
        private readonly PasswordHasherService _passwordHasher = passwordHasher;
        private readonly IRoleDAO _roleDAO = roleDAO;

        [HttpGet]
        public ActionResult<IEnumerable<UserEntity>> GetUsers()
        {
            var users = _userDAO.SelectAll();
            return Ok(users);
        }

        [HttpPost]
        public ActionResult CreateUser([FromBody] AdminCreateUserDTO dto)
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                RoleId = dto.RoleId,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var inserted = _userDAO.Insert(user);
            return Ok(inserted);
        }

        [HttpPut("{userId:guid}")]
        public ActionResult UpdateUser(Guid userId, [FromBody] AdminUpdateUserDTO dto)
        {
            if (userId != dto.Id)
                return BadRequest();

            var user = _userDAO.SelectById(userId);
            if (user == null)
                return NotFound();

            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.RoleId = dto.RoleId;
            user.Status = dto.Status;
            user.UpdatedAt = DateTime.UtcNow;

            return _userDAO.Update(user) ? Ok() : BadRequest();
        }

        [HttpPatch("{userId:guid}/role")]
        public ActionResult ChangeRole(Guid userId, [FromBody] UpdateUserRoleDTO dto)
        {
            var user = _userDAO.SelectById(userId);
            if (user == null) return NotFound();

            user.RoleId = dto.RoleId;
            user.UpdatedAt = DateTime.UtcNow;

            return _userDAO.Update(user) ? Ok() : BadRequest();
        }

        [HttpPatch("{userId:guid}/status/{status}")]
        public ActionResult ChangeStatus(Guid userId, byte status)
        {
            return _userDAO.UpdateStatus(userId, status) ? Ok() : BadRequest();
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _roleDAO.SelectAll();

                if (roles == null || !roles.Any())
                {
                    return NotFound();
                }

                return Ok(roles);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
