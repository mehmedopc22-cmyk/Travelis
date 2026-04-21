INSERT INTO HotelPayments
(
    Id,
    UserId,
    RoomId,
    CurrencyId,
    Amount,
    CreatedAt
)
VALUES
(
    @Id,
    @UserId,
    @RoomId,
    @CurrencyId,
    @Amount,
    @CreatedAt
)
