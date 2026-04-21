SELECT
    Id,
    UserId,
    RoomId,
    CurrencyId,
    Amount,
    CreatedAt
FROM HotelPayments
WHERE UserId = @UserId
