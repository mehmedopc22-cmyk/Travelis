SELECT 
		
		res.[Id],
        res.[Description],
        res.[Price],
        res.[RoomNo],
        res.[Floor],
        res.[BedCount],
        res.[Capacity],
		
		hotel.[Id] AS HotelId,
		hotel.[Name]

        FROM [HotelRooms] res
        INNER JOIN [Hotels] hotel ON res.[hotelId] = hotel.[Id];
