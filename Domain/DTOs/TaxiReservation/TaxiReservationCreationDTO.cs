namespace Domain.DTOs;

public class TaxiReservationCreationDTO
{
    public required Guid UserId { get; set; }
    public required Guid TaxiCompanyId { get; set; }
    public required string PickupAddress { get; set; }
    public required string DestinationAddress { get; set; }
    public required DateTime Time { get; set; }
    public required int Status { get; set; }
    
}


