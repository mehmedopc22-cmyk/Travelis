namespace Domain.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; } = 0;
        public string AvatarURL { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public byte? MFAType { get; set; } = null;
        public string? TFASecret { get; set; } = null;
        public bool IsVerified { get; set; } = false;
        public byte Status { get; set; } = 0;
        public Guid RoleId { get; set; } = Guid.Empty;
        public DateTime? LastLoginAt { get; set; } = null;
        public DateTime? CreatedAt { get; set; } = null;
        public DateTime? UpdatedAt { get; set; } = null;
    }
}