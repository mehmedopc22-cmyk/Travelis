using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class UpdateUserRoleDTO
    {
        public required Guid RoleId { get; set; }
    }
}
