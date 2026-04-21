SELECT
    Id,
    HotelId,
    CommentId
FROM HotelComments
WHERE Id = @Id
