namespace Domain.DTOs.RentalCar
{
    public class RentalCarCreationDTO
    {
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required int Kilometers { get; set; }
    }
}
