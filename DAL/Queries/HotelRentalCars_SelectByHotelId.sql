SELECT
    Id,
    HotelId,
    RentalCarId
FROM HotelRentalCars
WHERE HotelId = @HotelId
