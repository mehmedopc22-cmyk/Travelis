SELECT
    Id,
    HotelId,
    UserId,
    RoomId,
    CheckIn,
    CheckOut,
    CreatedAt,
    UpdatedAt
FROM HotelReservation
WHERE HotelId = @HotelId
