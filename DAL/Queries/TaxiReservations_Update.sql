UPDATE TaxiReservation
SET
    TaxiCompanyId = @TaxiCompanyId,
    UserId = @UserId,
    PickupAddress = @PickupAddress,
    DestinationAddress = @DestinationAddress,
    Time = @Time,
    UpdatedAt = @UpdatedAt,
    Status = @Status
WHERE Id = @Id
