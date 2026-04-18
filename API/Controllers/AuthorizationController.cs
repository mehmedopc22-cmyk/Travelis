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
                LastName = user.LastName,
                Role = user.RoleName,
                UserId = user.Id
            });
        }


        [HttpPost("register")]
        public ActionResult<LoginResponseDTO> Register(UserRegisterDTO user)
        {
            if (user == null)
                return BadRequest();

            string passwordHash = passwordHasher.Hash(user.PasswordHash);

            //За сега остава така това е за регистрация на нормален потрбител
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
    }
}
