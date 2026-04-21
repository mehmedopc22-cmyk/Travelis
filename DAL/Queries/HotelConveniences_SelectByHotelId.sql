SELECT
    hc.Id,
    hc.HotelId,
    hc.ConvenienceId
FROM HotelConveniences hc
WHERE hc.HotelId = @HotelId
