namespace Domain.DTOs
{
    public class HotelAdminAssignmentDTO
    {
        public Guid HotelId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string DisplayName =>
            string.IsNullOrWhiteSpace($"{FirstName} {LastName}".Trim())
                ? Email
                : $"{FirstName} {LastName}".Trim();
    }
}
