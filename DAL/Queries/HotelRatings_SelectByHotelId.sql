SELECT
    Id,
    HotelId,
    RatingId
FROM HotelRatings
WHERE HotelId = @HotelId
