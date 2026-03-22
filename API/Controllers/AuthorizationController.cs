using API.Helpers;
using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    public class AuthorizationController(IUserDAO userDAO, JWTService jwtService, PasswordHasherService passwordHasher) : ControllerBase
    {
        private readonly IUserDAO _userDAO = userDAO;
        private readonly JWTService _jwtService = jwtService;

        [HttpPost("login")]
        public ActionResult<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            UserLoginDTO? user = _userDAO.SelectByEmail(request.Email);

            if (user == null)
                return Unauthorized();

            bool valid = passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!valid)
                return Unauthorized();

            string token = _jwtService.Generate(user);

            return Ok(new LoginResponseDTO
            {
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }


        [HttpPost("register")]
        public ActionResult<LoginResponseDTO> Register(UserRegisterDTO user)
        {
            if (user == null)
                return BadRequest();

            string passwordHash = passwordHasher.Hash(user.PasswordHash);

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
    }
}
