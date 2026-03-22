using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class UserRegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Guid RoleId { get; set; } = Guid.Empty;
    }
}
