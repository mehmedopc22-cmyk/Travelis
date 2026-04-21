SELECT 
    res.[Id],
    res.[UseFrom],
    res.[UseTo],

    c.[Id] AS CarId,
    c.[Brand],
    c.[Model],
    c.[Kilometers],

    u.[Id] AS UserId,
    u.[Email],
    u.[FirstName],
    u.[LastName]

FROM [RentalCarReservation] res
INNER JOIN [RentalCars] c ON res.[CarID] = c.[Id]
INNER JOIN [Users] u ON res.[UserID] = u.[Id]

WHERE u.[Id] = @Id
