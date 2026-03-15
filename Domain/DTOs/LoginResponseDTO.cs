namespace Domain.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
    }
}