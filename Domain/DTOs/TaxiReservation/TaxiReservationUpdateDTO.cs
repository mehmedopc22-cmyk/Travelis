namespace Domain.DTOs;

public class TaxiReservationUpdateDTO
{
    public Guid Id { get; set; }
    public Guid TaxiCompanyId { get; set; }
    public string PickupAddress { get; set; }
    public string DestinationAddress { get; set; }
    public DateTime Time { get; set; }

    //public DateTimeOffset? UpdatedAt { get; set; } = null;
}