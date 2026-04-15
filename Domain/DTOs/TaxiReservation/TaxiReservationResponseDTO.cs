namespace Domain.DTOs;

public class TaxiReservationResponseDTO
{
    public required Guid Id { get; set; }
    public required Guid TaxiCompanyID { get; set; }
    public required string TaxiCompanyName { get; set; }
    public string TaxiCompanyPhoneNumber { get; set; } = string.Empty;
    public required Guid UserID { get; set; }
    public required string UserName { get; set; }
    public required string PickupAddress { get; set; }
    public required string DestinationAddress { get; set; }
    public required DateTime Time { get; set; }
    public required int Status { get; set; }
    
    public string StatusText =>
        Status switch
        {
            0 => "Declined",
            1 => "Accepted",
            2 => "In progress",
            3 => "Completed",
            _ => "Unknown status"
        };
//     public DateTimeOffset? CreatedAt { get; set; } = null;
//     public DateTimeOffset? UpdatedAt { get; set; } = null;
}