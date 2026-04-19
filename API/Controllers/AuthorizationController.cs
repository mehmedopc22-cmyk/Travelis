using API.Helpers;
using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace API.Controllers
{

    [ApiController]
    public class AuthorizationController(
        IUserDAO userDAO,
        JWTService jwtService,
        PasswordHasherService passwordHasher,
        IEmailService emailService) : ControllerBase
    {
        private readonly IUserDAO _userDAO = userDAO;
        private readonly JWTService _jwtService = jwtService;
        private readonly PasswordHasherService _passwordHasher = passwordHasher;
        private readonly IEmailService _emailService = emailService;

        [HttpPost("login")]
        public ActionResult<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            UserLoginDTO? user = _userDAO.SelectByEmail(request.Email);

            if (user == null)
                return Unauthorized();

            bool valid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!valid)
                return Unauthorized();

            string token = _jwtService.Generate(user);

            return Ok(new LoginResponseDTO
            {
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.RoleName,
                UserId = user.Id
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request, CancellationToken cancellationToken)
        {
            UserEntity? user = _userDAO.SelectUserByEmail(request.Email);

            if (user == null)
            {
                return Ok();
            }

            string temporaryPassword = GenerateTemporaryPassword();
            string temporaryPasswordHash = _passwordHasher.Hash(temporaryPassword);

            bool updated = _userDAO.UpdatePasswordHash(user.Id, temporaryPasswordHash);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to reset password.");
            }

            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, temporaryPassword, cancellationToken);
            }
            catch
            {
                _userDAO.UpdatePasswordHash(user.Id, user.PasswordHash);
                throw;
            }

            return Ok();
        }

        [HttpPost("change-password")]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordRequestDTO request)
        {
            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out Guid userId))
            {
                return Unauthorized();
            }

            UserEntity? user = _userDAO.SelectById(userId);

            if (user == null)
            {
                return NotFound();
            }

            bool currentPasswordValid = _passwordHasher.Verify(request.CurrentPassword, user.PasswordHash);

            if (!currentPasswordValid)
            {
                return BadRequest("Current password is incorrect.");
            }

            string newPasswordHash = _passwordHasher.Hash(request.NewPassword);
            bool updated = _userDAO.UpdatePasswordHash(user.Id, newPasswordHash);

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to change password.");
            }

            return NoContent();
        }

        [HttpPost("register")]
        public ActionResult<LoginResponseDTO> Register(UserRegisterDTO user)
        {
            if (user == null)
                return BadRequest();

            string passwordHash = _passwordHasher.Hash(user.PasswordHash);

            // Default role for a regular registered user.
            user.RoleId = new Guid("5A519675-975E-4C0E-8409-A45442E2645C");

            UserEntity userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = passwordHash,
                RoleId = user.RoleId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _userDAO.Insert(userEntity);

            return Ok();
        }

        private static string GenerateTemporaryPassword()
        {
            const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@$?";
            char[] password = new char[12];

            for (int index = 0; index < password.Length; index++)
            {
                password[index] = characters[RandomNumberGenerator.GetInt32(characters.Length)];
            }

            return new string(password);
        }
    }
}
