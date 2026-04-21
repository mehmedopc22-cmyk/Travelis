UPDATE RentalCarReservation
SET
    UserId = @UserId,
    CarId = @CarId,
    UseFrom = @UseFrom,
    UseTo = @UseTo,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id
