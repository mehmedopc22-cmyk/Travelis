namespace Domain.DTOs
{
    public class CarResponseDTO
    {
        public Guid CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Kilometers { get; set; }
    }
}
