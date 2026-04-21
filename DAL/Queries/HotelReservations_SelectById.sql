SELECT 
    res.[Id],
    res.[CheckIn],
    res.[CheckOut],

    h.[Id] AS HotelId,
    h.[Name],
    h.[Country],
    h.[City],
    h.[Street],
    h.[PostalCode],
    h.[PhoneNumber],
    h.[Email],

    u.[Id] AS UserId,
    u.[Email],
    u.[FirstName],
    u.[LastName],

    rm.[Id] AS RoomId,
    rm.[Description],
    rm.[Price],
    rm.[RoomNo],
    rm.[Floor],
    rm.[BedCount],
    rm.[Capacity]

FROM [HotelReservation] res
INNER JOIN [Hotels] h ON res.[HotelID] = h.[Id]
INNER JOIN [Users] u ON res.[UserID] = u.[Id]
INNER JOIN [HotelRooms] rm ON res.[RoomID] = rm.[Id]
WHERE res.[Id] = @Id;
