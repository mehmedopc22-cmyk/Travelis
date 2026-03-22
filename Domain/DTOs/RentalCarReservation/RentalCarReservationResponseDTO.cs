namespace Domain.DTOs
{
    public class RentalCarReservationResponseDTO
    {
        public Guid Id { get; set; }
        public UserResponseDTO User { get; set; }
        public CarResponseDTO Car { get; set; }
        public DateTime UseFrom { get; set; }
        public DateTime UseTo { get; set; }
    }
}
