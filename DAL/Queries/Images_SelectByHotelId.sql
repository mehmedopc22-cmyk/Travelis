SELECT i.Id, i.Name
FROM [HotelImages] hi
INNER JOIN [Images] i ON i.Id = hi.ImageId
WHERE hi.HotelId = @HotelId
ORDER BY hi.Id DESC
