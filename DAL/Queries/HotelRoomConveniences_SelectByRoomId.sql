SELECT
    Id,
    HotelRoomId,
    ConvenienceId
FROM HotelRoomConveniences
WHERE HotelRoomId = @HotelRoomId
