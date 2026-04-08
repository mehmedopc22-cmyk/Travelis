namespace Domain.DTOs;

public class TaxiReservationResponseDTO
{
    public required Guid Id { get; set; }
    public required Guid TaxiCompantyID { get; set; }
    public required Guid UserID { get; set; }
    public required string PickupAddress { get; set; }
    public required string DestinationAddress { get; set; }
    public required DateTime Time { get; set; }
    public required string UserName { get; set; }
    
//     public DateTimeOffset? CreatedAt { get; set; } = null;
//     public DateTimeOffset? UpdatedAt { get; set; } = null;
}