SELECT
    Id,
    HotelId,
    CommentId
FROM HotelComments
WHERE HotelId = @HotelId
