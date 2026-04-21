INSERT INTO TaxiReservation
(
    UserId,
    TaxiCompanyId,
    PickupAddress,
    DestinationAddress,
    Time,
    Status
)
VALUES
(
    @UserId, 
    @TaxiCompanyId,
    @PickupAddress,
    @DestinationAddress,
    @Time,
    @Status
 )
