using Domain.Entities;

namespace WEB.Models
{
    public class AdminUsersIndexViewModel
    {
        public List<UserEntity> Users { get; set; } = new();
        public List<RoleEntity> Roles { get; set; } = new();
    }
}
