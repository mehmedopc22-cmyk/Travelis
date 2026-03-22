using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
    }
}
