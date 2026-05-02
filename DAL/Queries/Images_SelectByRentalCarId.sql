SELECT i.Id, i.Name
FROM [RentalCarImages] rci
INNER JOIN [Images] i ON i.Id = rci.ImageId
WHERE rci.RentalCarId = @RentalCarId
ORDER BY rci.Id DESC
