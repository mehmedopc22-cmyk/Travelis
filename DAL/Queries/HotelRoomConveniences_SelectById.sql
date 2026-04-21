SELECT
    Id,
    HotelRoomId,
    ConvenienceId
FROM HotelRoomConveniences
WHERE Id = @Id
