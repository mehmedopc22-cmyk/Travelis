UPDATE HotelRooms
SET
    HotelId = @HotelId,
    Description = @Description,
    Price = @Price,
    RoomNo = @RoomNo,
    Floor = @Floor,
    BedCount = @BedCount,
    Capacity = @Capacity,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id
