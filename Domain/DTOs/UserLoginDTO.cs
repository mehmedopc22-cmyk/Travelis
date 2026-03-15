namespace Domain.DTOs
{
    public class UserLoginDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public bool IsVerified { get; set; }

        public byte Status { get; set; }
    }
}