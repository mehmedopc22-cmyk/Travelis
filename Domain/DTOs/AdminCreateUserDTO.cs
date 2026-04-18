using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class AdminCreateUserDTO
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
        public required Guid RoleId { get; set; }
        public byte Status { get; set; } = 1;
    }
}
