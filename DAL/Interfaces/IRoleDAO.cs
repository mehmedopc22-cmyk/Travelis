using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IRoleDAO
    {
        IEnumerable<RoleEntity> SelectAll();
        RoleEntity? SelectById(Guid roleId);
    }
}