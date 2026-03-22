namespace Domain.DTOs.RentalCarReservation
{
    public class RentalCarReservationCreationDTO
    {
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }
        public DateTime UseFrom { get; set; }
        public DateTime UseTo { get; set; }
    }
}
