using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("user")]
    public class UserController(IUserDAO userDAO) : ControllerBase
    {
        private readonly IUserDAO _userDAO = userDAO;

        [HttpGet("all")]
        public ActionResult<IEnumerable<UserEntity>> GetUsers()
        {
            try
            {
                IEnumerable<UserEntity> users = _userDAO.SelectAll();

                if (users == null || !users.Any())
                {
                    return NotFound();
                }

                return Ok(users);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{userId:guid}")]
        public ActionResult<UserEntity> GetUserById(Guid userId)
        {
            try
            {
                UserEntity? user = _userDAO.SelectById(userId);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("email/{email}")]
        public ActionResult<UserEntity> GetUserByEmail(string email)
        {
            try
            {
                UserEntity? user = _userDAO.SelectUserByEmail(email);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("role/{roleId:guid}")]
        public ActionResult<IEnumerable<UserEntity>> GetUsersByRoleId(Guid roleId)
        {
            try
            {
                IEnumerable<UserEntity> users = _userDAO.SelectByRoleId(roleId);

                if (users == null || !users.Any())
                {
                    return NotFound();
                }

                return Ok(users);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("status/{status}")]
        public ActionResult<IEnumerable<UserEntity>> GetUsersByStatus(byte status)
        {
            try
            {
                IEnumerable<UserEntity> users = _userDAO.SelectByStatus(status);

                if (users == null || !users.Any())
                {
                    return NotFound();
                }

                return Ok(users);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult InsertUser([FromBody] UserEntity user)
        {
            try
            {
                UserEntity insertedUser = _userDAO.Insert(user);

                if (insertedUser == null)
                {
                    return StatusCode(500, "Insertion failed");
                }

                return Ok(insertedUser);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public ActionResult UpdateUser([FromBody] UserEntity user)
        {
            try
            {
                if (_userDAO.Update(user))
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

        [HttpPatch("status/{userId:guid}/{status}")]
        public ActionResult UpdateUserStatus(Guid userId, byte status)
        {
            try
            {
                if (_userDAO.UpdateStatus(userId, status))
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

        [HttpPatch("verify/{userId:guid}")]
        public ActionResult VerifyUser(Guid userId)
        {
            try
            {
                if (_userDAO.UpdateVerification(userId, true))
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

        [HttpDelete("{userId:guid}")]
        public ActionResult DeleteUser(Guid userId)
        {
            try
            {
                if (_userDAO.Delete(userId))
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
    }
}