UPDATE HotelReservation
SET
    HotelId = @HotelId,
    UserId = @UserId,
    RoomId = @RoomId,
    CheckIn = @CheckIn,
    CheckOut = @CheckOut,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id
