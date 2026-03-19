using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IUserDAO : IBaseDAO<UserEntity>
    {
        UserLoginDTO? SelectByEmail(string email);
        UserEntity? SelectById(Guid id);
        bool EmailExists(string email);
        bool UpdateLastLogin(Guid userId);
        Guid Insert(UserEntity user);
    }
}