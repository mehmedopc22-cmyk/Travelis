namespace WEB.Models
{
    public class LoginResponseViewModel
    {
        public string Token { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public Guid UserId { get; set; }
    }
}