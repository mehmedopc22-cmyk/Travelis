DELETE FROM HotelRatings
WHERE HotelId = @HotelId
  AND RatingId = @RatingId
