using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IUserDAO : IBaseDAO<UserEntity>
    {
        UserLoginDTO? SelectByEmail(string email);
        bool EmailExists(string email);
        bool UpdateLastLogin(Guid userId);
    }
}