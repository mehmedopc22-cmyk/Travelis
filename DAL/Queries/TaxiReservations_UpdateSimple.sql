UPDATE TaxiReservation
SET
    TaxiCompanyId = @TaxiCompanyId,
    PickupAddress = @PickupAddress,
    DestinationAddress = @DestinationAddress,
    Time = @Time,
    UpdatedAt = @UpdatedAt,
    Status = @Status
WHERE Id = @Id
