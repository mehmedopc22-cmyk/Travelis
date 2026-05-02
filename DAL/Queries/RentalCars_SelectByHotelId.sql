SELECT
    rc.Id,
    rc.Brand,
    rc.Model,
    rc.Kilometers
FROM RentalCars rc
INNER JOIN HotelRentalCars hrc ON hrc.RentalCarId = rc.Id
WHERE hrc.HotelId = @HotelId
ORDER BY rc.Brand, rc.Model
