namespace Domain.DTOs.RentalCarReservation
{
    public class RentalCarReservationPatchDTO
    {
        public Guid? CarID { get; set; }
        public DateTime? UseFrom { get; set; }
        public DateTime? UseTo { get; set; }
    }
}
