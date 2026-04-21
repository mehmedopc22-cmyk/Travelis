SELECT
    Id,
    TaxiCompanyId,
    UserId,
    PickupAddress,
    DestinationAddress,
    Time,
    CreatedAt,
    UpdatedAt,
    Status
FROM TaxiReservation
WHERE UserId = @UserId
