using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IUserDAO : IBaseDAO<UserEntity>
    {
        UserLoginDTO? SelectByEmail(string email);
        bool EmailExists(string email);
        bool UpdateLastLogin(Guid userId);

        UserEntity? SelectUserByEmail(string email);
        IEnumerable<UserEntity> SelectByRoleId(Guid roleId);
        IEnumerable<UserEntity> SelectByStatus(byte status);
        bool UpdateStatus(Guid userId, byte status);
        bool UpdateVerification(Guid userId, bool isVerified);
    }
}