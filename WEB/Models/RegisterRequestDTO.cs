namespace WEB.Models
{
    public class RegisterRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public Guid RoleId { get; set; }
    }
}