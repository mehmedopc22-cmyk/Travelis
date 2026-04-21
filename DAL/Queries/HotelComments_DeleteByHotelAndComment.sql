DELETE FROM HotelComments
WHERE HotelId = @HotelId
  AND CommentId = @CommentId
