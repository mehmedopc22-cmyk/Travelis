namespace Domain.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int? LoyaltyPoints { get; set; }

        public string? AvatarURL { get; set; }

        public string? PasswordHash { get; set; }

        public byte? MFAType { get; set; }

        public string? TFASecret { get; set; }

        public bool IsVerified { get; set; }

        public byte Status { get; set; }

        public Guid RoleId { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}