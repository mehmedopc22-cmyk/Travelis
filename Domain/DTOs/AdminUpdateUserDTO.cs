using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class AdminUpdateUserDTO
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public Guid RoleId { get; set; }
        public byte Status { get; set; }
    }
}
